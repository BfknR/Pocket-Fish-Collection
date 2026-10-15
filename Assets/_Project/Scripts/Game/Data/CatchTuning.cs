using System;
using PocketFish.Domain.Catch;
using PocketFish.Domain.Fishing;
using UnityEngine;

namespace PocketFish.Game.Data
{
    /// <summary>
    /// All catch odds live here, so balancing never touches code. Also holds the shadow scales.
    /// One asset shared by every pond. What differs per pond lives in <see cref="PondDefinition"/>.
    /// </summary>
    [CreateAssetMenu(menuName = "PocketFish/Catch Tuning", fileName = "CatchTuning")]
    public sealed class CatchTuning : ScriptableObject
    {
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

        [Header("Quality Odds by Tap Grade")]
        [SerializeField] private OddsRow _missOdds;
        [SerializeField] private OddsRow _goodOdds;
        [SerializeField] private OddsRow _greatOdds;
        [SerializeField] private OddsRow _perfectOdds;

        [Header("Shadow Sizes")]
        [SerializeField, Min(0.1f)] private float _smallShadowScale = 0.6f;
        [SerializeField, Min(0.1f)] private float _mediumShadowScale = 1f;
        [SerializeField, Min(0.1f)] private float _largeShadowScale = 1.6f;

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
        /// Editor-only check. Every odds row needs at least one weight above zero, or the roll would fail.
        /// </summary>
        private void OnValidate()
        {
            WarnIfEmpty(_missOdds, nameof(_missOdds));
            WarnIfEmpty(_goodOdds, nameof(_goodOdds));
            WarnIfEmpty(_greatOdds, nameof(_greatOdds));
            WarnIfEmpty(_perfectOdds, nameof(_perfectOdds));
        }

        private void WarnIfEmpty(OddsRow row, string rowName)
        {
            if (row.Normal + row.Good + row.Great + row.Perfect <= 0f)
                Debug.LogError($"{name}: {rowName} has no weight above zero.", this);
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
