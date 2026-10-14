using System;
using System.Collections.Generic;
using PocketFish.Domain.Catch;
using PocketFish.Domain.Fishing;
using UnityEngine;

namespace PocketFish.Game.Data
{
    /// <summary>
    /// All catch odds live here, so balancing never touches code.
    /// Also holds the pond settings, the minigame time limit and the shadow scales.
    /// </summary>
    [CreateAssetMenu(menuName = "PocketFish/Catch Tuning", fileName = "CatchTuning")]
    public sealed class CatchTuning : ScriptableObject
    {
        /// <summary>
        /// A fish that can appear in the pond, with its relative spawn weight.
        /// </summary>
        [Serializable]
        public struct PondEntry
        {
            /// <summary>The fish that can spawn.</summary>
            public FishDefinition Fish;

            /// <summary>Relative chance of spawning. Doesn't need to sum to any particular value.</summary>
            [Min(0f)] public float Weight;
        }

        /// <summary>
        /// Editable quality weights for one tap grade.
        /// </summary>
        [Serializable]
        private struct OddsRow
        {
            [Min(0f)] public float Normal;
            [Min(0f)] public float Good;
            [Min(0f)] public float Great;
            [Min(0f)] public float Perfect;

            /// <summary>Converts the row to the engine-free <see cref="QualityOdds"/>.</summary>
            public QualityOdds ToDomain() => new QualityOdds(Normal, Good, Great, Perfect);
        }

        [SerializeField] private PondEntry[] _pond;

        [Header("Quality Odds by Tap Grade")]
        [SerializeField] private OddsRow _missOdds;
        [SerializeField] private OddsRow _goodOdds;
        [SerializeField] private OddsRow _greatOdds;
        [SerializeField] private OddsRow _perfectOdds;

        /// <summary>The fish that can spawn in the pond, with their weights.</summary>
        public IReadOnlyList<PondEntry> Pond => _pond;

        [Header("Minigame")]
        [Tooltip("Fixed time before a no-tap Miss. The same for every fish.")]
        [SerializeField, Min(0.5f)] private float _minigameTimeLimitSeconds = 3f;

        [Header("Pond")]
        [SerializeField, Min(1)] private int _pondCapacity = 6;
        [SerializeField, Min(5f)] private float _spawnIntervalSeconds = 60f;

        [Header("Shadow Sizes")]
        [SerializeField, Min(0.1f)] private float _smallShadowScale = 0.6f;
        [SerializeField, Min(0.1f)] private float _mediumShadowScale = 1f;
        [SerializeField, Min(0.1f)] private float _largeShadowScale = 1.6f;

        /// <summary>Seconds before a no-tap Miss. The same for every fish.</summary>
        public float MinigameTimeLimitSeconds => _minigameTimeLimitSeconds;

        /// <summary>Maximum number of fish in the pond at once.</summary>
        public int PondCapacity => _pondCapacity;

        /// <summary>Seconds between fish spawns.</summary>
        public float SpawnIntervalSeconds => _spawnIntervalSeconds;

        /// <summary>The fish that can spawn in the pond, with their weights.</summary>
        public IReadOnlyList<PondEntry> PondEntries => _pond;

        /// <summary>
        /// Looks up a fish in the pond table by its id.
        /// </summary>
        /// <param name="id">The <see cref="FishDefinition.Id"/> to find.</param>
        /// <returns>The matching definition, or null if the pond table doesn't contain it.</returns>
        public FishDefinition FindFish(string id)
        {
            foreach (var entry in _pond)
            {
                if (entry.Fish != null && entry.Fish.Id == id)
                    return entry.Fish;
            }
            return null;
        }

        /// <summary>
        /// Gets the quality odds that belong to a tap grade.
        /// </summary>
        /// <param name="grade">How accurate the player's tap was.</param>
        /// <returns>The odds configured for that grade.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown for a grade with no configured odds.</exception>
        public QualityOdds OddsFor(TapGrade grade)
        {
            switch (grade)
            {
                case TapGrade.Miss: return _missOdds.ToDomain();
                case TapGrade.Good: return _goodOdds.ToDomain();
                case TapGrade.Great: return _greatOdds.ToDomain();
                case TapGrade.Perfect: return _perfectOdds.ToDomain();
                default: throw new ArgumentOutOfRangeException(nameof(grade), grade, null);
            }
        }

        /// <summary>
        /// Gets the visual scale for a shadow size.
        /// </summary>
        /// <param name="size">The shadow size.</param>
        /// <returns>The configured scale. Unknown sizes fall back to the medium scale.</returns>
        public float ShadowScaleFor(ShadowSize size)
        {
            switch (size)
            {
                case ShadowSize.Small: return _smallShadowScale;
                case ShadowSize.Large: return _largeShadowScale;
                default: return _mediumShadowScale;
            }
        }
    }
}
