using System;
using PocketFish.Domain.Catch;
using PocketFish.Domain.Progression;
using UnityEngine;

namespace PocketFish.Game.Data
{
    /// <summary>
    /// How many coins catches are worth. A catch pays the base amount for its rarity
    /// times the multiplier for its quality.
    /// </summary>
    [CreateAssetMenu(menuName = "PocketFish/Economy Tuning", fileName = "EconomyTuning")]
    public sealed class EconomyTuning : ScriptableObject
    {
        [Serializable]
        private struct RarityCoins
        {
            [Min(0)] public int Common;
            [Min(0)] public int Uncommon;
            [Min(0)] public int Rare;
            [Min(0)] public int Legendary;
        }

        [Serializable]
        private struct QualityMultipliers
        {
            [Min(0f)] public float Normal;
            [Min(0f)] public float Good;
            [Min(0f)] public float Great;
            [Min(0f)] public float Perfect;
        }

        [Header("Base coins per catch, by rarity")]
        [SerializeField] private RarityCoins _baseCoins = new RarityCoins { Common = 10, Uncommon = 25, Rare = 60, Legendary = 150 };

        [Header("Multiplier by quality")]
        [SerializeField] private QualityMultipliers _qualityMultipliers = new QualityMultipliers { Normal = 1f, Good = 1.5f, Great = 2f, Perfect = 3f };

        /// <summary>Builds the engine-free reward table.</summary>
        public CoinRewardTable ToDomain()
        {
            // Indexed by the Rarity and Quality enums, so the order here must match them.
            var bases = new int[Enum.GetValues(typeof(Rarity)).Length];
            bases[(int)Rarity.Common] = _baseCoins.Common;
            bases[(int)Rarity.Uncommon] = _baseCoins.Uncommon;
            bases[(int)Rarity.Rare] = _baseCoins.Rare;
            bases[(int)Rarity.Legendary] = _baseCoins.Legendary;

            var multipliers = new float[Enum.GetValues(typeof(Quality)).Length];
            multipliers[(int)Quality.Normal] = _qualityMultipliers.Normal;
            multipliers[(int)Quality.Good] = _qualityMultipliers.Good;
            multipliers[(int)Quality.Great] = _qualityMultipliers.Great;
            multipliers[(int)Quality.Perfect] = _qualityMultipliers.Perfect;

            return new CoinRewardTable(bases, multipliers);
        }
    }
}
