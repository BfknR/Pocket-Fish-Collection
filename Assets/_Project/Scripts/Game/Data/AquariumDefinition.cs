using System;
using System.Collections.Generic;
using PocketFish.Domain.Progression;
using UnityEngine;

namespace PocketFish.Game.Data
{
    /// <summary>
    /// Designer-editable data for one aquarium: its name, price, the pond it unlocks
    /// and its upgradable items.
    /// </summary>
    [CreateAssetMenu(menuName = "PocketFish/Aquarium Definition", fileName = "Aquarium_New")]
    public sealed class AquariumDefinition : ScriptableObject
    {
        /// <summary>Every aquarium is meant to have this many upgradable items.</summary>
        public const int ExpectedUpgradeCount = 4;

        /// <summary>Every item is meant to have this many levels.</summary>
        public const int ExpectedLevelCount = 5;

        /// <summary>One upgradable item and what each of its levels costs.</summary>
        [Serializable]
        public struct UpgradeItem
        {
            /// <summary>Stable identifier, used in saves.</summary>
            public string Id;

            /// <summary>Name shown in the upgrade list.</summary>
            public string DisplayName;

            /// <summary>Coins to buy each level, in order. Index 0 is level 1.</summary>
            public int[] LevelCosts;
        }

        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField, Min(0)] private int _cost;

        [Header("Pond")]
        [Tooltip("The pond this aquarium gives access to: its fish, capacity and spawn rate.")]
        [SerializeField] private PondDefinition _pond;

        [Header("Upgrades")]
        [SerializeField] private UpgradeItem[] _upgrades;

        /// <summary>Stable identifier, used in saves. Don't change it once players may have saves.</summary>
        public string Id => _id;

        /// <summary>Name shown to the player.</summary>
        public string DisplayName => _displayName;

        /// <summary>Coins to buy this aquarium. Ignored for the first one, which is always owned.</summary>
        public int Cost => _cost;

        /// <summary>The pond this aquarium gives access to.</summary>
        public PondDefinition Pond => _pond;

        /// <summary>The upgradable items.</summary>
        public IReadOnlyList<UpgradeItem> Upgrades => _upgrades;

        /// <summary>Builds the engine-free description used by the progression rules.</summary>
        public AquariumSpec ToDomain()
        {
            var upgrades = new List<UpgradeSpec>();
            foreach (UpgradeItem item in _upgrades)
            {
                upgrades.Add(new UpgradeSpec(item.Id, item.LevelCosts ?? new int[0]));
            }
            return new AquariumSpec(_id, _cost, upgrades);
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_id)) Debug.LogWarning($"{name}: no id set.", this);
            if (_pond == null) Debug.LogWarning($"{name}: no pond assigned.", this);
            if (_upgrades == null) return;

            if (_upgrades.Length != ExpectedUpgradeCount)
            {
                Debug.LogWarning($"{name}: has {_upgrades.Length} upgrade items, expected {ExpectedUpgradeCount}.", this);
            }
            foreach (UpgradeItem item in _upgrades)
            {
                if (item.LevelCosts == null || item.LevelCosts.Length != ExpectedLevelCount)
                {
                    Debug.LogWarning($"{name}: upgrade '{item.Id}' should have {ExpectedLevelCount} level costs.", this);
                }
            }
        }
    }
}
