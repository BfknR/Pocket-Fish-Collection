using System;
using System.Collections.Generic;
using PocketFish.Domain.Catch;
using PocketFish.Domain.Fishing;
using PocketFish.Game.Collection;
using PocketFish.Game.Data;
using PocketFish.Game.Persistence;
using PocketFish.Game.Progression;
using PocketFish.Domain.Progression;
using UnityEngine;

namespace PocketFish.Game.Fishing
{
    /// <summary>
    /// Owns the pond's fish: spawns them over real time, saves them, and restores them on launch.
    /// Which pond it is depends on the aquarium the player has selected.
    /// </summary>
    public sealed class PondSession : MonoBehaviour
    {
        [SerializeField] private AquariumCatalog _aquariums;
        [SerializeField] private SpeciesCatalog _catalog;
        [SerializeField] private CatchTuning _tuning;

        private PondDefinition _pond;
        private string _pondId;
        private bool _isFirstPond;
        private PlayerProgress _progress;
        private CoinRewardTable _rewards;

        private PondPopulation _population;
        private System.Random _random;
        private long _lastSyncTicks;
        private CollectionSession _collection;

        /// <summary>The game-wide catch odds and shadow sizes, shared by every pond.</summary>
        public CatchTuning Tuning => _tuning;

        /// <summary>The selected aquarium's pond: its fish, capacity and spawn rate.</summary>
        public PondDefinition Pond => _pond;

        /// <summary>The player's coins and aquarium progression.</summary>
        public PlayerProgress Progress => _progress;

        /// <summary>The live pond. Other components read and modify it through this.</summary>
        public PondPopulation Population => _population;

        /// <summary>The player's collection, rebuilt from the saved catch log.</summary>
        public CollectionSession Collection => _collection;

        /// <summary>Builds the collection and the pond, restores any save, then credits time spent away.</summary>
        private void Awake()
        {
            _random = new System.Random();
            _progress = ProgressSession.Get(_aquariums);
            _rewards = _aquariums.Economy.ToDomain();
            AquariumDefinition aquarium = _aquariums.Aquariums[_progress.SelectedIndex];
            _pond = aquarium.Pond;
            _pondId = aquarium.Id;
            _isFirstPond = _progress.SelectedIndex == 0;
            _collection = new CollectionSession(_catalog.ToDomainList(), CatchLogStore.Load());
            WarnAboutPondFishMissingFromCatalog();
            _population = new PondPopulation(_pond.Capacity, _pond.SpawnIntervalSeconds, RollSpecies);
            Restore();
            Sync();
        }

        /// <summary>Keeps the spawn timer running while the app is in the foreground.</summary>
        private void Update()
        {
            Sync();
        }

        /// <summary>Saves when the app goes to the background and catches up when it returns.</summary>
        private void OnApplicationPause(bool paused)
        {
            if (paused) Save();
            else Sync();
        }

        /// <summary>Saves the pond when leaving the scene.</summary>
        private void OnDestroy()
        {
            Save();
        }

        /// <summary>Saves the pond when the app quits.</summary>
        private void OnApplicationQuit()
        {
            Save();
        }

        /// <summary>
        /// Advances the pond by the wall-clock time since the last sync.
        /// Covers foreground time and time spent with the app in the background.
        /// </summary>
        public void Sync()
        {
            long now = DateTime.UtcNow.Ticks;
            _population.Advance(PondTime.ElapsedSeconds(_lastSyncTicks, now));
            _lastSyncTicks = now;
        }

        /// <summary>
        /// Writes the pond to disk. Call on every catch as well as on pause and quit.
        /// </summary>
        public void Save()
        {
            Sync();

            var data = new PondSaveData
            {
                TimerSeconds = _population.TimerSeconds,
                SavedUtcTicks = _lastSyncTicks,
            };
            foreach (PondFish fish in _population.Fish)
            {
                data.FishIds.Add(fish.Species.Id);
            }

            PondSaveStore.Save(data, _pondId);
        }

        /// <summary>
        /// Pays the player for a catch and saves their progress.
        /// </summary>
        /// <param name="outcome">The resolved catch.</param>
        /// <returns>The coins the catch was worth.</returns>
        public int AwardCoins(CatchOutcome outcome)
        {
            int coins = _rewards.For(outcome.Species.Rarity, outcome.Quality);
            _progress.AddCoins(coins);
            ProgressSession.Save();
            return coins;
        }

        /// <summary>
        /// Gets the definition for a pond fish, used for its sprite and minigame when it is tapped.
        /// </summary>
        /// <param name="fish">A fish currently in the pond.</param>
        /// <returns>The matching definition, or null if the pond table no longer contains the species.</returns>
        public FishDefinition DefinitionFor(PondFish fish)
        {
            return _pond.FindFish(fish.Species.Id);
        }

        /// <summary>
        /// Loads the saved pond into the population. Unknown fish ids are skipped with a warning.
        /// On a first launch there is no save, so the spawn timer starts now.
        /// </summary>
        private void Restore()
        {
            PondSaveData save = PondSaveStore.Load(_pondId, _isFirstPond);
            if (save == null)
            {
                _lastSyncTicks = 0; // first launch: the timer starts now
                return;
            }

            var species = new List<FishSpecies>();
            foreach (string id in save.FishIds)
            {
                FishDefinition definition = _pond.FindFish(id);
                if (definition == null)
                {
                    Debug.LogWarning($"Pond save references unknown fish '{id}'. Skipping it.");
                    continue;
                }
                species.Add(definition.ToDomain());
            }

            _population.Restore(species, save.TimerSeconds);
            _lastSyncTicks = save.SavedUtcTicks;
        }

        /// <summary>
        /// Warns about any fish in this pond's table that the catalog doesn't list.
        /// Catching such a fish couldn't be recorded in the Collection Book.
        /// </summary>
        private void WarnAboutPondFishMissingFromCatalog()
        {
            foreach (var entry in _pond.Fish)
            {
                if (entry.Fish != null && !_catalog.Contains(entry.Fish.Id))
                {
                    Debug.LogWarning($"Pond fish '{entry.Fish.Id}' is not in the species catalog, so its catches won't be recorded.");
                }
            }
        }

        /// <summary>Picks a species for a newly spawned fish. Passed to the population as its roll callback.</summary>
        private FishSpecies RollSpecies()
        {
            return RollFishDefinition().ToDomain();
        }

        /// <summary>
        /// Same weighted pick as the cast roll, but over the pond table.
        /// </summary>
        /// <returns>The chosen definition. Null only if the pond table is empty.</returns>
        private FishDefinition RollFishDefinition()
        {
            float total = 0f;
            foreach (var entry in _pond.Fish) total += entry.Weight;

            float pick = (float)_random.NextDouble() * total;
            FishDefinition last = null;
            foreach (var entry in _pond.Fish)
            {
                last = entry.Fish;
                pick -= entry.Weight;
                if (pick < 0f) return entry.Fish;
            }
            return last; // float rounding. Null only if the pond table is empty.
        }
    }
}
