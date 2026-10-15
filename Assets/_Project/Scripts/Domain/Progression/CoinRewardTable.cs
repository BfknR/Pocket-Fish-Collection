using System;
using System.Collections.Generic;
using PocketFish.Domain.Catch;

namespace PocketFish.Domain.Progression
{
    /// <summary>
    /// How many coins a catch is worth: a base amount for the species' rarity,
    /// multiplied by a factor for the catch's quality.
    /// </summary>
    public sealed class CoinRewardTable
    {
        private readonly IReadOnlyList<int> _baseByRarity;
        private readonly IReadOnlyList<float> _multiplierByQuality;

        /// <summary>Creates a reward table.</summary>
        /// <param name="baseByRarity">Base coins, indexed by <see cref="Rarity"/>.</param>
        /// <param name="multiplierByQuality">Multiplier, indexed by <see cref="Quality"/>.</param>
        public CoinRewardTable(IReadOnlyList<int> baseByRarity, IReadOnlyList<float> multiplierByQuality)
        {
            _baseByRarity = baseByRarity;
            _multiplierByQuality = multiplierByQuality;
        }

        /// <summary>
        /// Works out the coins for a catch.
        /// </summary>
        /// <param name="rarity">Rarity of the species.</param>
        /// <param name="quality">Quality of the catch.</param>
        /// <returns>Rounded coins, never negative. A rarity with no entry pays nothing, a quality with no entry counts as ×1.</returns>
        public int For(Rarity rarity, Quality quality)
        {
            int rarityIndex = (int)rarity;
            int qualityIndex = (int)quality;
            if (rarityIndex < 0 || rarityIndex >= _baseByRarity.Count) return 0;

            float multiplier = qualityIndex >= 0 && qualityIndex < _multiplierByQuality.Count ? _multiplierByQuality[qualityIndex] : 1f;
            return Math.Max(0, (int)Math.Round(_baseByRarity[rarityIndex] * multiplier, MidpointRounding.AwayFromZero));
        }
    }
}
