using System;
using System.Collections.Generic;

namespace PocketFish.Domain.Progression
{
    /// <summary>
    /// The player's coins and aquarium progression: which aquariums are owned, which one is selected
    /// and how far each item is upgraded. Aquariums are bought in order, and the next one unlocks
    /// only once every item of the latest one is at its top level.
    /// </summary>
    public sealed class PlayerProgress
    {
        private readonly IReadOnlyList<AquariumSpec> _aquariums;
        private readonly int[][] _levels;
        private int _ownedCount = 1;

        /// <summary>
        /// Creates fresh progress: no coins, the first aquarium owned and nothing upgraded.
        /// </summary>
        /// <param name="aquariums">Every aquarium, in the order they are bought. Must not be empty.</param>
        public PlayerProgress(IReadOnlyList<AquariumSpec> aquariums)
        {
            if (aquariums == null || aquariums.Count == 0) throw new ArgumentException("At least one aquarium is required.", nameof(aquariums));

            _aquariums = aquariums;
            _levels = new int[aquariums.Count][];
            for (int i = 0; i < aquariums.Count; i++) _levels[i] = new int[aquariums[i].Upgrades.Count];
        }

        /// <summary>Raised whenever coins, ownership, selection or a level changes.</summary>
        public event Action Changed;

        /// <summary>Coins the player has.</summary>
        public int Coins { get; private set; }

        /// <summary>Every aquarium, in order.</summary>
        public IReadOnlyList<AquariumSpec> Aquariums => _aquariums;

        /// <summary>How many aquariums are owned. They are always the first ones in order.</summary>
        public int OwnedCount => _ownedCount;

        /// <summary>Index of the aquarium the player is looking at and fishing in. Always an owned one.</summary>
        public int SelectedIndex { get; private set; }

        /// <summary>
        /// Gives the player coins. Zero and negative amounts are ignored.
        /// </summary>
        /// <param name="amount">Coins to add.</param>
        public void AddCoins(int amount)
        {
            if (amount <= 0) return;

            Coins += amount;
            Changed?.Invoke();
        }

        /// <summary>Checks whether an aquarium is owned.</summary>
        /// <param name="aquariumIndex">Index of the aquarium.</param>
        public bool IsOwned(int aquariumIndex) => aquariumIndex >= 0 && aquariumIndex < _ownedCount;

        /// <summary>
        /// Selects an owned aquarium.
        /// </summary>
        /// <param name="aquariumIndex">Index of the aquarium.</param>
        /// <returns>False if it isn't owned, in which case nothing changes.</returns>
        public bool Select(int aquariumIndex)
        {
            if (!IsOwned(aquariumIndex)) return false;
            if (aquariumIndex == SelectedIndex) return true;

            SelectedIndex = aquariumIndex;
            Changed?.Invoke();
            return true;
        }

        /// <summary>
        /// Gets how far an item is upgraded.
        /// </summary>
        /// <param name="aquariumIndex">Index of the aquarium.</param>
        /// <param name="upgradeIndex">Index of the item within the aquarium.</param>
        /// <returns>The current level, from 0. Zero for an unknown aquarium or item.</returns>
        public int LevelOf(int aquariumIndex, int upgradeIndex)
        {
            return IsValid(aquariumIndex, upgradeIndex) ? _levels[aquariumIndex][upgradeIndex] : 0;
        }

        /// <summary>
        /// Gets what the next level of an item costs.
        /// </summary>
        /// <param name="aquariumIndex">Index of the aquarium.</param>
        /// <param name="upgradeIndex">Index of the item within the aquarium.</param>
        /// <returns>The cost, or null if the item is at its top level or doesn't exist.</returns>
        public int? NextUpgradeCost(int aquariumIndex, int upgradeIndex)
        {
            if (!IsValid(aquariumIndex, upgradeIndex)) return null;

            UpgradeSpec upgrade = _aquariums[aquariumIndex].Upgrades[upgradeIndex];
            int level = _levels[aquariumIndex][upgradeIndex];
            return level >= upgrade.MaxLevel ? (int?)null : upgrade.LevelCosts[level];
        }

        /// <summary>
        /// Buys the next level of an item.
        /// </summary>
        /// <param name="aquariumIndex">Index of the aquarium.</param>
        /// <param name="upgradeIndex">Index of the item within the aquarium.</param>
        /// <returns>The outcome. Coins are spent and the level rises only on Success.</returns>
        public PurchaseResult TryUpgrade(int aquariumIndex, int upgradeIndex)
        {
            if (!IsValid(aquariumIndex, upgradeIndex)) return PurchaseResult.Unknown;
            if (!IsOwned(aquariumIndex)) return PurchaseResult.NotOwned;

            int? cost = NextUpgradeCost(aquariumIndex, upgradeIndex);
            if (cost == null) return PurchaseResult.AlreadyMaxLevel;
            if (Coins < cost.Value) return PurchaseResult.NotEnoughCoins;

            Coins -= cost.Value;
            _levels[aquariumIndex][upgradeIndex]++;
            Changed?.Invoke();
            return PurchaseResult.Success;
        }

        /// <summary>
        /// Checks whether every item of an aquarium is at its top level.
        /// </summary>
        /// <param name="aquariumIndex">Index of the aquarium.</param>
        /// <returns>True if all are maxed. False for an unknown aquarium.</returns>
        public bool IsFullyUpgraded(int aquariumIndex)
        {
            if (aquariumIndex < 0 || aquariumIndex >= _aquariums.Count) return false;

            IReadOnlyList<UpgradeSpec> upgrades = _aquariums[aquariumIndex].Upgrades;
            for (int i = 0; i < upgrades.Count; i++)
            {
                if (_levels[aquariumIndex][i] < upgrades[i].MaxLevel) return false;
            }
            return true;
        }

        /// <summary>True when the latest owned aquarium is fully upgraded and there is another one to buy.</summary>
        public bool CanBuyNextAquarium => _ownedCount < _aquariums.Count && IsFullyUpgraded(_ownedCount - 1);

        /// <summary>Price of the next aquarium, or null if there are no more.</summary>
        public int? NextAquariumCost => _ownedCount < _aquariums.Count ? _aquariums[_ownedCount].Cost : (int?)null;

        /// <summary>
        /// Buys the next aquarium and selects it.
        /// </summary>
        /// <returns>The outcome. Coins are spent and the aquarium is added only on Success.</returns>
        public PurchaseResult TryBuyNextAquarium()
        {
            if (!CanBuyNextAquarium) return PurchaseResult.NotAvailable;

            int cost = _aquariums[_ownedCount].Cost;
            if (Coins < cost) return PurchaseResult.NotEnoughCoins;

            Coins -= cost;
            _ownedCount++;
            SelectedIndex = _ownedCount - 1;
            Changed?.Invoke();
            return PurchaseResult.Success;
        }

        /// <summary>
        /// Loads saved state. Values are clamped to what is possible, so a damaged or outdated save
        /// can't produce an impossible state.
        /// </summary>
        /// <param name="coins">Saved coins.</param>
        /// <param name="ownedCount">Saved number of owned aquariums.</param>
        /// <param name="selectedIndex">Saved selected aquarium.</param>
        /// <param name="levels">Saved level of each item, per aquarium. Missing entries count as zero.</param>
        public void Restore(int coins, int ownedCount, int selectedIndex, IReadOnlyList<IReadOnlyList<int>> levels)
        {
            Coins = Math.Max(0, coins);
            _ownedCount = Math.Max(1, Math.Min(ownedCount, _aquariums.Count));
            SelectedIndex = Math.Max(0, Math.Min(selectedIndex, _ownedCount - 1));

            for (int a = 0; a < _levels.Length; a++)
            {
                for (int u = 0; u < _levels[a].Length; u++)
                {
                    int saved = levels != null && a < levels.Count && levels[a] != null && u < levels[a].Count ? levels[a][u] : 0;
                    _levels[a][u] = Math.Max(0, Math.Min(saved, _aquariums[a].Upgrades[u].MaxLevel));
                }
            }
            Changed?.Invoke();
        }

        private bool IsValid(int aquariumIndex, int upgradeIndex)
        {
            return aquariumIndex >= 0 && aquariumIndex < _aquariums.Count
                && upgradeIndex >= 0 && upgradeIndex < _aquariums[aquariumIndex].Upgrades.Count;
        }
    }
}
