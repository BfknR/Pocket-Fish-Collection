using PocketFish.Domain.Fishing;

namespace PocketFish.Domain.Catch
{
    /// <summary>
    /// Plain data snapshot of a species, built from FishDefinition in the Game layer.
    /// </summary>
    public readonly struct FishSpecies
    {
        /// <summary>
        /// Creates a species snapshot.
        /// </summary>
        /// <param name="id">Stable identifier, used when saving and loading the pond.</param>
        /// <param name="displayName">Name shown to the player.</param>
        /// <param name="rarity">How rare the species is.</param>
        /// <param name="minigame">Difficulty of the catch minigame for this species.</param>
        public FishSpecies(string id, string displayName, Rarity rarity, MinigameConfig minigame)
        {
            Id = id;
            DisplayName = displayName;
            Rarity = rarity;
            Minigame = minigame;
        }

        /// <summary>Stable identifier, used when saving and loading the pond.</summary>
        public string Id { get; }

        /// <summary>Name shown to the player.</summary>
        public string DisplayName { get; }

        /// <summary>How rare the species is.</summary>
        public Rarity Rarity { get; }

        /// <summary>Difficulty of the catch minigame for this species.</summary>
        public MinigameConfig Minigame { get; }
    }
}
