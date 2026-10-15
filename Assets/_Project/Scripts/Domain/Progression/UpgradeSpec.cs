using System.Collections.Generic;

namespace PocketFish.Domain.Progression
{
    /// <summary>
    /// One upgradable item of an aquarium: its id and what each level costs.
    /// </summary>
    public sealed class UpgradeSpec
    {
        /// <summary>Creates an upgrade item.</summary>
        /// <param name="id">Stable identifier, used in saves.</param>
        /// <param name="levelCosts">Coins to buy each level, in order. The count is the max level.</param>
        public UpgradeSpec(string id, IReadOnlyList<int> levelCosts)
        {
            Id = id;
            LevelCosts = levelCosts;
        }

        /// <summary>Stable identifier, used in saves.</summary>
        public string Id { get; }

        /// <summary>Coins to buy each level. Index 0 is the cost of level 1.</summary>
        public IReadOnlyList<int> LevelCosts { get; }

        /// <summary>The highest level the item can reach.</summary>
        public int MaxLevel => LevelCosts.Count;
    }
}
