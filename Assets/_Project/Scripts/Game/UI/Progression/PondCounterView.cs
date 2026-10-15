using PocketFish.Domain.Fishing;
using PocketFish.Domain.Progression;
using PocketFish.Game.Data;
using PocketFish.Game.Persistence;
using TMPro;
using UnityEngine;

namespace PocketFish.Game.UI
{
    /// <summary>
    /// Presentation only. Shows how many fish are in the selected aquarium's pond and how long until the next one,
    /// such as "Pond 4/6" over "Next fish 0:07".
    /// The pond isn't loaded in the Aquarium scene, so the count is projected from the pond's save and
    /// the time since it was written, the same way the pond catches up when it opens.
    /// </summary>
    public sealed class PondCounterView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField, Min(0.05f)] private float _refreshSeconds = 0.25f;

        private PlayerProgress _progress;
        private AquariumCatalog _catalog;
        private float _nextRefresh;

        /// <summary>
        /// Starts showing the selected aquarium's pond.
        /// </summary>
        /// <param name="progress">The progress, for which aquarium is selected.</param>
        /// <param name="catalog">The aquariums and their ponds.</param>
        public void Bind(PlayerProgress progress, AquariumCatalog catalog)
        {
            _progress = progress;
            _catalog = catalog;
            _progress.Changed += Refresh;
            Refresh();
        }

        private void OnDestroy()
        {
            if (_progress != null) _progress.Changed -= Refresh;
        }

        /// <summary>Fish keep spawning while the player watches the tank, so recount often enough for the countdown to tick.</summary>
        private void Update()
        {
            if (_progress == null || Time.unscaledTime < _nextRefresh) return;
            Refresh();
        }

        private void Refresh()
        {
            _nextRefresh = Time.unscaledTime + _refreshSeconds;

            int index = _progress.SelectedIndex;
            PondDefinition pond = _catalog.Aquariums[index].Pond;
            PondSaveData save = PondSaveStore.Load(_catalog.Aquariums[index].Id, index == 0);

            // A pond that was never visited has no fish yet, and its timer starts when it is first opened.
            if (save == null)
            {
                _label.text = $"Pond 0/{pond.Capacity}\nOpen it to start the timer";
                return;
            }

            PondForecast.Result result = PondForecast.Project(
                save.FishIds.Count, save.TimerSeconds,
                PondTime.ElapsedSeconds(save.SavedUtcTicks, System.DateTime.UtcNow.Ticks),
                pond.Capacity, pond.SpawnIntervalSeconds);

            string status = result.SecondsToNextFish.HasValue
                ? $"Next fish {FormatTime(result.SecondsToNextFish.Value)}"
                : "Pond is full";
            _label.text = $"Pond {result.FishCount}/{pond.Capacity}\n{status}";
        }

        /// <summary>Formats seconds as m:ss, rounding up so the countdown never shows 0:00 before the fish arrives.</summary>
        private static string FormatTime(double seconds)
        {
            int total = Mathf.Max(0, Mathf.CeilToInt((float)seconds));
            return $"{total / 60}:{total % 60:00}";
        }
    }
}
