using System.Collections.Generic;

namespace PocketFish.Domain.Progression
{
    /// <summary>
    /// One aquarium in the progression: its id, the price to buy it and its upgradable items.
    /// </summary>
    public sealed class AquariumSpec
    {
        /// <summary>Creates an aquarium.</summary>
        /// <param name="id">Stable identifier, used in saves.</param>
        /// <param name="cost">Coins to buy it. Unused for the first aquarium, which is always owned.</param>
        /// <param name="upgrades">The upgradable items.</param>
        public AquariumSpec(string id, int cost, IReadOnlyList<UpgradeSpec> upgrades)
        {
            Id = id;
            Cost = cost;
            Upgrades = upgrades;
        }

        /// <summary>Stable identifier, used in saves.</summary>
        public string Id { get; }

        /// <summary>Coins to buy this aquarium.</summary>
        public int Cost { get; }

        /// <summary>The upgradable items.</summary>
        public IReadOnlyList<UpgradeSpec> Upgrades { get; }
    }
}
