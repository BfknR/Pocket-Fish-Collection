using PocketFish.Domain.Catch;
using PocketFish.Domain.Fishing;
using PocketFish.Game.Data;
using PocketFish.Game.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PocketFish.Game.Fishing
{
    /// <summary>
    /// Runs the fishing loop. Tapping a fish shadow starts the timing minigame,
    /// and when it ends the catch is rolled, the fish leaves the pond and the result is revealed.
    /// </summary>
    public sealed class PondController : MonoBehaviour
    {
        /// <summary>Whether the player is waiting to pick a fish or is in the minigame.</summary>
        private enum PondState { Idle, Fishing }

        [SerializeField] private CatchTuning _tuning;
        [SerializeField] private PondSession _session;
        [SerializeField] private Camera _camera;
        [SerializeField] private CatchRevealView _revealView;
        [SerializeField] private TimerBarView _timerBar;
        [SerializeField] private TimingArcView _arcView;

        private readonly System.Random _rng = new System.Random();
        private PondState _state = PondState.Idle;
        private IMinigame _minigame;
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

        /// <summary>
        /// Reads input each frame. While idle, a tap tries to pick a fish.
        /// While fishing, it feeds the tap and time to the minigame, updates the UI and resolves when done.
        /// </summary>
        private void Update()
        {
            bool pressed = Pointer.current != null && Pointer.current.press.wasPressedThisFrame;

            if (_state == PondState.Idle)
            {
                if (pressed) TryStartFromTap();
                return;
            }

            if (pressed) _minigame.Tap();
            _minigame.Tick(Time.deltaTime);
            _arcView.SetMarker(_minigame.MarkerPosition);
            _timerBar.SetProgress(_minigame.Progress);
            if (_minigame.IsFinished) Resolve();
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
            _activeSpecies = _activeDefinition.ToDomain(_tuning.MinigameTimeLimitSeconds);

            _minigame = new TapMinigame();
            _minigame.Begin(_activeSpecies.Minigame);

            _arcView.Show(_activeSpecies.Minigame);
            _timerBar.Show();
            _state = PondState.Fishing;
        }

        /// <summary>
        /// Finishes the attempt: rolls the quality from the tap grade, removes the fish from the pond,
        /// saves, and shows the result.
        /// </summary>
        private void Resolve()
        {
            TapGrade grade = _minigame.Result;
            Quality quality = CatchRoller.PickQuality(_tuning.OddsFor(grade), _rng);
            var outcome = new CatchOutcome(_activeSpecies, grade, quality);

            _arcView.Hide();
            _timerBar.Hide();

            // Every resolved attempt catches the fish, including a miss.
            _session.Population.Remove(_activeView.Fish.InstanceId);
            _session.Save();

            _state = PondState.Idle;
            _revealView.Show(outcome, _activeDefinition.Sprite);
        }
    }
}
