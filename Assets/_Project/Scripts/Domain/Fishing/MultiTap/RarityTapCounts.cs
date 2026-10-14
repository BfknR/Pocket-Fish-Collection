using System;
using PocketFish.Domain.Catch;

namespace PocketFish.Domain.Fishing
{
    /// <summary>
    /// How many taps the minigame has for each <see cref="Rarity"/>.
    /// </summary>
    public readonly struct RarityTapCounts
    {
        /// <summary>
        /// Creates a tap count table.
        /// </summary>
        /// <param name="common">Taps for Common fish.</param>
        /// <param name="uncommon">Taps for Uncommon fish.</param>
        /// <param name="rare">Taps for Rare fish.</param>
        /// <param name="legendary">Taps for Legendary fish.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when a count is below 1.</exception>
        public RarityTapCounts(int common, int uncommon, int rare, int legendary)
        {
            Common = Validate(common, nameof(common));
            Uncommon = Validate(uncommon, nameof(uncommon));
            Rare = Validate(rare, nameof(rare));
            Legendary = Validate(legendary, nameof(legendary));
        }

        /// <summary>Taps for Common fish.</summary>
        public int Common { get; }

        /// <summary>Taps for Uncommon fish.</summary>
        public int Uncommon { get; }

        /// <summary>Taps for Rare fish.</summary>
        public int Rare { get; }

        /// <summary>Taps for Legendary fish.</summary>
        public int Legendary { get; }

        /// <summary>
        /// Gets the tap count for a rarity.
        /// </summary>
        /// <param name="rarity">The species rarity.</param>
        /// <returns>The number of taps.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown for an unknown rarity.</exception>
        public int For(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Common: return Common;
                case Rarity.Uncommon: return Uncommon;
                case Rarity.Rare: return Rare;
                case Rarity.Legendary: return Legendary;
                default: throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null);
            }
        }

        private static int Validate(int count, string name)
        {
            if (count < 1) throw new ArgumentOutOfRangeException(name, count, "A minigame needs at least one tap.");
            return count;
        }
    }
}
