using System;
using PocketFish.Domain.Catch;

namespace PocketFish.Domain.Collection
{
    /// <summary>
    /// Everything the collection knows about one species, derived from its catches.
    /// </summary>
    public sealed class SpeciesRecord
    {
        private static readonly int QualityCount = Enum.GetValues(typeof(Quality)).Length;

        private readonly int[] _qualityCounts = new int[QualityCount];
        private CatchRecord _best;
        private DateTime _firstCaughtUtc;

        /// <summary>
        /// Creates an empty record, which counts as undiscovered.
        /// </summary>
        /// <param name="species">The species this record belongs to.</param>
        public SpeciesRecord(FishSpecies species)
        {
            Species = species;
        }

        /// <summary>The species this record belongs to.</summary>
        public FishSpecies Species { get; }

        /// <summary>How many of this species have been caught.</summary>
        public int Count { get; private set; }

        /// <summary>True once at least one has been caught.</summary>
        public bool IsDiscovered => Count > 0;

        /// <summary>The best catch by <see cref="BestCatchRule"/>. Only valid when discovered.</summary>
        public CatchRecord BestCatch => _best;

        /// <summary>Quality of the best catch. Only valid when discovered.</summary>
        public Quality BestQuality => _best.Quality;

        /// <summary>When the first one was caught. Only valid when discovered.</summary>
        public DateTime FirstCaughtUtc => _firstCaughtUtc;

        /// <summary>
        /// Counts the catches of one quality.
        /// </summary>
        /// <param name="quality">The quality to count.</param>
        /// <returns>How many catches had that quality.</returns>
        public int CountOf(Quality quality) => _qualityCounts[(int)quality];

        /// <summary>
        /// Adds a catch of this species and updates the records.
        /// </summary>
        /// <param name="record">The catch to add. Its species id must match this record's species.</param>
        internal void Add(in CatchRecord record)
        {
            if(Count == 0)
            {
                _best = record;
                _firstCaughtUtc = record.CaughtUtc;
            }
            else
            {
                if(BestCatchRule.IsBetter(record, _best)) _best = record;
                if(record.CaughtUtc < _firstCaughtUtc) _firstCaughtUtc = record.CaughtUtc;
            }
            _qualityCounts[(int)record.Quality]++;
            Count++;
        }
    }
}
