using PocketFish.Domain.Catch;
using PocketFish.Domain.Fishing;
using PocketFish.Game.Data;
using PocketFish.Game.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PocketFish.Game.Fishing
{
    /// <summary>
    /// Runs the fishing loop. Tapping a fish shadow starts the multi-tap minigame,
    /// and when its last tap is done the catch is rolled, the fish leaves the pond and the result is revealed.
    /// </summary>
    public sealed class PondController : MonoBehaviour
    {
        /// <summary>Whether the player is waiting to pick a fish or is in the minigame.</summary>
        private enum PondState { Idle, Fishing }

        [SerializeField] private MinigameBalance _balance;
        [SerializeField] private PondSession _session;
        [SerializeField] private Camera _camera;
        [SerializeField] private CatchRevealView _revealView;
        [SerializeField] private RectTransform _aquariumButton;
        [SerializeField] private CoinCounterView _coins;
        [SerializeField] private TimerBarView _timerBar;
        [SerializeField] private TimingArcView _arcView;

        private readonly System.Random _rng = new System.Random();
        private PondState _state = PondState.Idle;
        private MultiTapMinigame _minigame;
        private int _shownTapIndex;
        private FishView _activeView;
        private FishDefinition _activeDefinition;
        private FishSpecies _activeSpecies;

        /// <summary>Hides all fishing UI until a fish is tapped.</summary>
        private void Awake()
        {
            _revealView.Hide();
            _timerBar.Hide();
            _arcView.Hide();
        }

        /// <summary>Shows the coin counter. Done in Start because the session builds the progress in its Awake.</summary>
        private void Start()
        {
            _coins.Bind(_session.Progress);
        }

        /// <summary>
        /// Reads input each frame. While idle, a tap tries to pick a fish.
        /// While fishing, it feeds the tap and time to the minigame, updates the UI and resolves when done.
        /// </summary>
        private void Update()
        {
            bool pressed = Pointer.current != null && Pointer.current.press.wasPressedThisFrame;
            // Pressing the Aquarium button must not also start fishing.
            if (pressed && IsOverAquariumButton(Pointer.current.position.ReadValue())) pressed = false;

            if (_state == PondState.Idle)
            {
                if (pressed) TryStartFromTap();
                return;
            }

            if (pressed) _minigame.Tap();
            _minigame.Tick(Time.deltaTime);
            if (_minigame.IsFinished)
            {
                Resolve();
                return;
            }

            if (_minigame.CurrentIndex != _shownTapIndex) ShowTap();
            _arcView.SetMarker(_minigame.MarkerPosition);
            _timerBar.SetProgress(_minigame.Progress);
        }

        /// <summary>Checks whether a screen press landed on the Aquarium button. The canvas is screen space overlay.</summary>
        private bool IsOverAquariumButton(Vector2 screenPoint)
        {
            return _aquariumButton.gameObject.activeInHierarchy
                && RectTransformUtility.RectangleContainsScreenPoint(_aquariumButton, screenPoint, null);
        }

        /// <summary>Checks whether the tap landed on a fish and starts fishing if it did.</summary>
        private void TryStartFromTap()
        {
            Vector2 screen = Pointer.current.position.ReadValue();
            Vector3 world = _camera.ScreenToWorldPoint(
                new Vector3(screen.x, screen.y, -_camera.transform.position.z));

            Collider2D hit = Physics2D.OverlapPoint(world);
            if (hit == null) return;

            FishView view = hit.GetComponent<FishView>();
            if (view != null) StartFishing(view);
        }

        /// <summary>
        /// Begins the minigame for the tapped fish and shows the minigame UI.
        /// </summary>
        /// <param name="view">The fish that was tapped.</param>
        private void StartFishing(FishView view)
        {
            _revealView.Hide();

            _activeView = view;
            _activeDefinition = _session.DefinitionFor(view.Fish);
            _activeSpecies = _activeDefinition.ToDomain();

            _minigame = _balance.CreateMinigame(_activeSpecies.Rarity);
            _minigame.Begin();

            ShowTap();
            _state = PondState.Fishing;
            _aquariumButton.gameObject.SetActive(false);
        }

        /// <summary>Draws the zones and resets the timer for the tap now in play.</summary>
        private void ShowTap()
        {
            _shownTapIndex = _minigame.CurrentIndex;
            _arcView.Show(_minigame.CurrentTier);
            _timerBar.Show();
        }

        /// <summary>
        /// Finishes the attempt: rolls the quality from the final grade, removes the fish from the pond,
        /// saves, and shows the result.
        /// </summary>
        private void Resolve()
        {
            MinigameResult result = _minigame.Result;
            TapGrade grade = result.FinalGrade;
            Quality quality = CatchRoller.PickQuality(_session.Tuning.OddsFor(grade), _rng);
            var outcome = new CatchOutcome(_activeSpecies, grade, quality);

            _arcView.Hide();
            _timerBar.Hide();

            // Every resolved attempt catches the fish, including a miss.
            _session.Collection.Record(outcome);
            _session.AwardCoins(outcome);
            _session.Population.Remove(_activeView.Fish.InstanceId);
            _session.Save();

            _state = PondState.Idle;
            _aquariumButton.gameObject.SetActive(true);
            _revealView.Show(outcome, _activeDefinition.Sprite, result.AverageScore);
        }
    }
}
