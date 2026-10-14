using System;
using System.Collections.Generic;
using PocketFish.Domain.Catch;

namespace PocketFish.Domain.Fishing
{
    /// <summary>
    /// One fish currently in the pond: a species plus an id that is unique within this population.
    /// </summary>
    public readonly struct PondFish
    {
        /// <summary>
        /// Creates a pond fish.
        /// </summary>
        /// <param name="instaceId">Id unique within the pond. Not stable across saves.</param>
        /// <param name="species">The species of this fish.</param>
        public PondFish(int instaceId, FishSpecies species)
        {
            InstanceId = instaceId;
            Species = species;
        }

        /// <summary>Id unique within the pond. Not stable across saves.</summary>
        public int InstanceId { get; }

        /// <summary>The species of this fish.</summary>
        public FishSpecies Species { get; }
    }

    /// <summary>
    /// The fish in the pond. A timer adds one fish per spawn interval until the pond is full.
    /// Holds no engine code, so it can be unit tested.
    /// </summary>
    public sealed class PondPopulation
    {
        private readonly int _capacity;
        private readonly double _spawnIntervalSeconds;
        private readonly Func<FishSpecies> _rollSpecies;
        private readonly List<PondFish> _fish = new List<PondFish>();
        private int _nextInstanceId = 1;
        private double _timerSeconds;

        /// <summary>
        /// Creates an empty pond.
        /// </summary>
        /// <param name="capacity">Maximum number of fish in the pond.</param>
        /// <param name="spawnIntervalSeconds">Seconds between spawns.</param>
        /// <param name="rollSpecies">Called each time a fish spawns, to pick its species.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when capacity is negative or the interval is not positive.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="rollSpecies"/> is null.</exception>
        public PondPopulation(int capacity, double spawnIntervalSeconds, Func<FishSpecies> rollSpecies)
        {
            if(capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));
            if(spawnIntervalSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(spawnIntervalSeconds));
            _capacity = capacity;
            _spawnIntervalSeconds = spawnIntervalSeconds;
            _rollSpecies = rollSpecies ?? throw new ArgumentNullException(nameof(rollSpecies));
        }

        /// <summary>The fish currently in the pond.</summary>
        public IReadOnlyList<PondFish> Fish => _fish;

        /// <summary>How many fish are in the pond.</summary>
        public int Count => _fish.Count;

        /// <summary>Seconds elapsed toward the next spawn. Saved by persistence so a reopen continues the timer.</summary>
        public double TimerSeconds => _timerSeconds;

        /// <summary>
        /// Rebuilds the pond from saved data. Use on load. Instance ids are reassigned.
        /// Fish beyond the capacity are dropped, and the timer is clamped to the spawn interval.
        /// </summary>
        /// <param name="savedSpecies">Species of the saved fish.</param>
        /// <param name="timerSeconds">Saved spawn timer.</param>
        public void Restore(IEnumerable<FishSpecies> savedSpecies, double timerSeconds)
        {
            _fish.Clear();
            foreach (FishSpecies species in savedSpecies)
            {
                if(_fish.Count >= _capacity) break;
                _fish.Add(new PondFish(_nextInstanceId++, species));
            }
            _timerSeconds = Math.Max(0, Math.Min(timerSeconds, _spawnIntervalSeconds));
        }

        /// <summary>
        /// Moves the spawn timer forward and adds any fish that are due, up to the capacity.
        /// Used live (each frame or tick) and on load (time away). Same rules for both.
        /// </summary>
        /// <param name="elapsedSeconds">Seconds to add. Zero or less is ignored.</param>
        public void Advance(double elapsedSeconds)
        {
            if(elapsedSeconds <= 0) return;

            _timerSeconds += elapsedSeconds;
            int due = (int)(_timerSeconds / _spawnIntervalSeconds);
            _timerSeconds %= _spawnIntervalSeconds;

            int spawns = Math.Min(due, _capacity - _fish.Count);
            for(int i = 0; i < spawns; i++)
            {
                _fish.Add(new PondFish(_nextInstanceId++, _rollSpecies()));
            }
        }

        /// <summary>
        /// Removes a fish from the pond. Called when a fish is caught.
        /// </summary>
        /// <param name="instanceId">The <see cref="PondFish.InstanceId"/> to remove.</param>
        /// <returns>True if the fish was removed, false if the id is not in the pond.</returns>
        public bool Remove(int instanceId)
        {
            for(int i = 0; i < _fish.Count; i++)
            {
                if(_fish[i].InstanceId == instanceId)
                {
                    _fish.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
    }
}
