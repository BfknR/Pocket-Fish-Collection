using System.Collections.Generic;
using PocketFish.Domain.Catch;

namespace PocketFish.Domain.Collection
{
    /// <summary>
    /// Derives per-species records and profile totals from catch records. Stores nothing twice:
    /// rebuild it from the saved catches, or feed it each new catch.
    /// </summary>
    public sealed class CollectionProgress
    {
        private readonly List<SpeciesRecord> _records = new List<SpeciesRecord>();
        private readonly Dictionary<string, SpeciesRecord> _byId = new Dictionary<string, SpeciesRecord>();

        /// <summary>
        /// Creates an empty collection.
        /// </summary>
        /// <param name="allSpecies">Every species in the game, in display order.</param>
        public CollectionProgress(IEnumerable<FishSpecies> allSpecies)
        {
            foreach(FishSpecies species in allSpecies)
            {
                var record = new SpeciesRecord(species);
                _records.Add(record);
                _byId[species.Id] = record;
            }
        }

        /// <summary>One record per species, in display order.</summary>
        public IReadOnlyList<SpeciesRecord> Species => _records;

        /// <summary>Number of species in the game.</summary>
        public int SpeciesTotal => _records.Count;

        /// <summary>Number of species caught at least once.</summary>
        public int SpeciesDiscovered { get; private set; }

        /// <summary>Total catches across all species.</summary>
        public int TotalCatches { get; private set; }

        /// <summary>Best quality across all catches, or null if nothing was caught.</summary>
        public Quality? BestQuality { get; private set; }

        /// <summary>Share of species discovered, from 0 to 1. Zero when the game has no species.</summary>
        public float Completion => SpeciesTotal == 0 ? 0f : (float)SpeciesDiscovered / SpeciesTotal;

        /// <summary>
        /// Looks up the record of a species.
        /// </summary>
        /// <param name="speciesId">Id of the species.</param>
        /// <param name="record">The record, or null if the id is unknown.</param>
        /// <returns>True if the species exists.</returns>
        public bool TryGetRecord(string speciesId, out SpeciesRecord record)
        {
            if(speciesId == null)
            {
                record = null;
                return false;
            }
            return _byId.TryGetValue(speciesId, out record);
        }

        /// <summary>
        /// Adds a catch. A catch of an unknown species is rejected and counts for nothing.
        /// </summary>
        /// <param name="catchRecord">The catch to add.</param>
        /// <returns>True if it was counted, false if the species id is unknown.</returns>
        public bool Add(in CatchRecord catchRecord)
        {
            if(!TryGetRecord(catchRecord.SpeciesId, out SpeciesRecord record)) return false;

            if(!record.IsDiscovered) SpeciesDiscovered++;
            record.Add(catchRecord);
            TotalCatches++;
            if(BestQuality == null || catchRecord.Quality > BestQuality) BestQuality = catchRecord.Quality;
            return true;
        }
    }
}
