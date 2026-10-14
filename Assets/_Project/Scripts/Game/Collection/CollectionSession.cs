using System;
using System.Collections.Generic;
using PocketFish.Domain.Catch;
using PocketFish.Domain.Collection;
using PocketFish.Game.Persistence;
using UnityEngine;

namespace PocketFish.Game.Collection
{
    /// <summary>
    /// Owns the player's collection at runtime: rebuilds it from the saved catch log on creation,
    /// and records each new catch to both the collection and the log.
    /// </summary>
    public sealed class CollectionSession
    {
        private readonly CatchLogData _log;

        /// <summary>
        /// Builds the collection from a saved log. Entries that can't be read or whose species is unknown
        /// are skipped with a warning but stay in the log, so nothing is lost.
        /// </summary>
        /// <param name="allSpecies">Every species in the game, in display order.</param>
        /// <param name="log">The saved catch log.</param>
        public CollectionSession(IEnumerable<FishSpecies> allSpecies, CatchLogData log)
        {
            _log = log;
            Progress = new CollectionProgress(allSpecies);

            foreach (CatchLogEntry entry in _log.Entries)
            {
                if (!CatchLogConverter.TryToRecord(entry, out CatchRecord record))
                {
                    Debug.LogWarning("Catch log has an unreadable entry. Skipping it.");
                    continue;
                }
                if (!Progress.Add(record))
                {
                    Debug.LogWarning($"Catch log references unknown species '{record.SpeciesId}'. Skipping it.");
                }
            }
        }

        /// <summary>The collection derived from the catch log. Read-only for screens.</summary>
        public CollectionProgress Progress { get; }

        /// <summary>
        /// Records a new catch in the collection and saves the log.
        /// </summary>
        /// <param name="outcome">The resolved catch.</param>
        public void Record(CatchOutcome outcome)
        {
            var record = new CatchRecord(outcome.Species.Id, outcome.Quality, null, DateTime.UtcNow);
            if (!Progress.Add(record))
            {
                Debug.LogWarning($"Catch of unknown species '{record.SpeciesId}' was not recorded.");
                return;
            }

            _log.Entries.Add(CatchLogConverter.ToEntry(record));
            CatchLogStore.Save(_log);
        }
    }
}
