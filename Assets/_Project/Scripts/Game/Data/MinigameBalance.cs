using System;
using System.Collections.Generic;
using PocketFish.Domain.Catch;
using PocketFish.Domain.Fishing;
using UnityEngine;

namespace PocketFish.Game.Data
{
    /// <summary>
    /// All multi-tap minigame numbers live here: taps per rarity, difficulty by tap index,
    /// the score for each grade and the average-to-grade thresholds.
    /// </summary>
    [CreateAssetMenu(menuName = "PocketFish/Minigame Balance", fileName = "MinigameBalance")]
    public sealed class MinigameBalance : ScriptableObject
    {
        /// <summary>
        /// Difficulty of one tap position. The same for every fish.
        /// </summary>
        [Serializable]
        public struct TapTier
        {
            [Tooltip("Seconds for the arrow to cross the arc one way. Smaller is faster.")]
            [Min(0.2f)] public float SweepSeconds;
            [Range(0.005f, 0.5f)] public float PerfectHalfWidth;
            [Range(0.005f, 0.5f)] public float GreatHalfWidth;
            [Range(0.005f, 0.5f)] public float GoodHalfWidth;
            [Tooltip("Seconds before a no-tap Miss.")]
            [Min(0.5f)] public float TimeLimitSeconds;

            /// <summary>Converts the tier to the engine-free <see cref="MinigameConfig"/>.</summary>
            public MinigameConfig ToDomain() =>
                new MinigameConfig(SweepSeconds, TimeLimitSeconds, GoodHalfWidth, GreatHalfWidth, PerfectHalfWidth);
        }

        [Header("Taps per Rarity")]
        [SerializeField, Min(1)] private int _commonTaps = 1;
        [SerializeField, Min(1)] private int _uncommonTaps = 2;
        [SerializeField, Min(1)] private int _rareTaps = 3;
        [SerializeField, Min(1)] private int _legendaryTaps = 4;

        [Header("Difficulty by Tap Index (first entry is the first tap)")]
        [SerializeField] private TapTier[] _tiers =
        {
            new TapTier { SweepSeconds = 1.6f, PerfectHalfWidth = 0.04f,  GreatHalfWidth = 0.10f, GoodHalfWidth = 0.18f, TimeLimitSeconds = 4f },
            new TapTier { SweepSeconds = 1.3f, PerfectHalfWidth = 0.03f,  GreatHalfWidth = 0.08f, GoodHalfWidth = 0.15f, TimeLimitSeconds = 3.5f },
            new TapTier { SweepSeconds = 1.1f, PerfectHalfWidth = 0.02f,  GreatHalfWidth = 0.06f, GoodHalfWidth = 0.11f, TimeLimitSeconds = 3f },
            new TapTier { SweepSeconds = 0.9f, PerfectHalfWidth = 0.015f, GreatHalfWidth = 0.04f, GoodHalfWidth = 0.08f, TimeLimitSeconds = 2.5f },
        };

        [Header("Score per Tap Grade")]
        [SerializeField, Min(0f)] private float _missScore = 0f;
        [SerializeField, Min(0f)] private float _goodScore = 0.5f;
        [SerializeField, Min(0f)] private float _greatScore = 0.75f;
        [SerializeField, Min(0f)] private float _perfectScore = 1f;

        [Header("Average Score to Final Grade (minimum for each)")]
        [SerializeField, Range(0f, 1f)] private float _goodMin = 0.40f;
        [SerializeField, Range(0f, 1f)] private float _greatMin = 0.65f;
        [SerializeField, Range(0f, 1f)] private float _perfectMin = 0.85f;

        /// <summary>
        /// Gets how many taps a species of the given rarity needs.
        /// </summary>
        /// <param name="rarity">The species rarity.</param>
        /// <returns>The tap count, never more than the number of tiers.</returns>
        public int TapCountFor(Rarity rarity)
        {
            int taps = new RarityTapCounts(_commonTaps, _uncommonTaps, _rareTaps, _legendaryTaps).For(rarity);
            return Mathf.Min(taps, _tiers.Length);
        }

        /// <summary>
        /// Builds a minigame for a species of the given rarity. The caller starts it with Begin.
        /// </summary>
        /// <param name="rarity">The species rarity, which sets the tap count.</param>
        /// <returns>A new, not yet started minigame.</returns>
        public MultiTapMinigame CreateMinigame(Rarity rarity)
        {
            var tiers = new List<MinigameConfig>(_tiers.Length);
            foreach (TapTier tier in _tiers) tiers.Add(tier.ToDomain());

            return new MultiTapMinigame(
                TapCountFor(rarity),
                tiers,
                new TapGradeScores(_missScore, _goodScore, _greatScore, _perfectScore),
                new GradeThresholds(_goodMin, _greatMin, _perfectMin));
        }

        /// <summary>
        /// Editor-only check. Keeps zones nested, thresholds ascending and every tap count backed by a tier.
        /// </summary>
        private void OnValidate()
        {
            if (_tiers == null || _tiers.Length == 0)
            {
                Debug.LogError($"{name}: needs at least one tier.", this);
                return;
            }

            for (int i = 0; i < _tiers.Length; i++)
            {
                TapTier tier = _tiers[i];
                tier.GreatHalfWidth = Mathf.Max(tier.GreatHalfWidth, tier.PerfectHalfWidth);
                tier.GoodHalfWidth = Mathf.Max(tier.GoodHalfWidth, tier.GreatHalfWidth);
                _tiers[i] = tier;
            }

            const float gap = 0.01f;
            _greatMin = Mathf.Max(_greatMin, _goodMin + gap);
            _perfectMin = Mathf.Max(_perfectMin, _greatMin + gap);

            int maxTaps = Mathf.Max(_commonTaps, _uncommonTaps, _rareTaps, _legendaryTaps);
            if (maxTaps > _tiers.Length)
                Debug.LogWarning($"{name}: a rarity wants {maxTaps} taps but there are only {_tiers.Length} tiers. Add tiers.", this);
        }
    }
}
