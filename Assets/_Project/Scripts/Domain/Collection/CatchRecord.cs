using System;
using PocketFish.Domain.Catch;

namespace PocketFish.Domain.Collection
{
    /// <summary>
    /// One caught fish as stored by the save system. Everything else in the collection is derived from these.
    /// </summary>
    public readonly struct CatchRecord
    {
        /// <summary>
        /// Creates a catch record.
        /// </summary>
        /// <param name="speciesId">Id of the species that was caught.</param>
        /// <param name="quality">The quality of the catch.</param>
        /// <param name="variantId">Id of the variant, or null for the regular look.</param>
        /// <param name="caughtUtc">When the fish was caught, in UTC.</param>
        public CatchRecord(string speciesId, Quality quality, string variantId, DateTime caughtUtc)
        {
            SpeciesId = speciesId;
            Quality = quality;
            VariantId = variantId;
            CaughtUtc = caughtUtc;
        }

        /// <summary>Id of the species that was caught.</summary>
        public string SpeciesId { get; }

        /// <summary>The quality of the catch.</summary>
        public Quality Quality { get; }

        /// <summary>Id of the variant, or null for the regular look.</summary>
        public string VariantId { get; }

        /// <summary>When the fish was caught, in UTC.</summary>
        public DateTime CaughtUtc { get; }
    }
}
