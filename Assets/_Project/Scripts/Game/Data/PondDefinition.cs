using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketFish.Game.Data
{
    /// <summary>
    /// One pond: which fish can spawn in it, how likely each is, how many fit and how fast they spawn.
    /// Game-wide catch balance (quality odds, shadow sizes) lives in <see cref="CatchTuning"/>, shared by every pond.
    /// </summary>
    [CreateAssetMenu(menuName = "PocketFish/Pond Definition", fileName = "Pond_New")]
    public sealed class PondDefinition : ScriptableObject
    {
        /// <summary>
        /// A fish that can appear in the pond, with its relative spawn weight.
        /// </summary>
        [Serializable]
        public struct Entry
        {
            /// <summary>The fish that can spawn.</summary>
            public FishDefinition Fish;

            /// <summary>Relative chance of spawning. Doesn't need to sum to any particular value.</summary>
            [Min(0f)] public float Weight;
        }

        [SerializeField] private Entry[] _fish;

        [Header("Pond")]
        [SerializeField, Min(1)] private int _capacity = 6;
        [SerializeField, Min(5f)] private float _spawnIntervalSeconds = 60f;

        /// <summary>The fish that can spawn in the pond, with their weights.</summary>
        public IReadOnlyList<Entry> Fish => _fish;

        /// <summary>Maximum number of fish in the pond at once.</summary>
        public int Capacity => _capacity;

        /// <summary>Seconds between fish spawns.</summary>
        public float SpawnIntervalSeconds => _spawnIntervalSeconds;

        /// <summary>
        /// Looks up a fish in this pond by its id.
        /// </summary>
        /// <param name="id">The <see cref="FishDefinition.Id"/> to find.</param>
        /// <returns>The matching definition, or null if this pond doesn't contain it.</returns>
        public FishDefinition FindFish(string id)
        {
            foreach (Entry entry in _fish)
            {
                if (entry.Fish != null && entry.Fish.Id == id) return entry.Fish;
            }
            return null;
        }
    }
}
