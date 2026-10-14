using PocketFish.Domain.Fishing;

namespace PocketFish.Domain.Catch
{
    /// <summary>
    /// The result of one resolved catch: which species, how well the player tapped, and the rolled quality.
    /// </summary>
    public readonly struct CatchOutcome
    {
        /// <summary>
        /// Creates a catch result.
        /// </summary>
        /// <param name="species">The species that was caught.</param>
        /// <param name="grade">How accurate the player's tap was in the minigame.</param>
        /// <param name="quality">The quality rolled from the odds for that grade.</param>
        public CatchOutcome(FishSpecies species, TapGrade grade, Quality quality)
        {
            Species = species;
            Grade = grade;
            Quality = quality;
        }

        /// <summary>The species that was caught.</summary>
        public FishSpecies Species { get; }

        /// <summary>How accurate the player's tap was in the minigame.</summary>
        public TapGrade Grade { get; }

        /// <summary>The quality tier rolled for this catch.</summary>
        public Quality Quality { get; }
    }
}
