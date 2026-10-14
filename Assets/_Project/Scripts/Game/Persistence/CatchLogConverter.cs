using System;
using PocketFish.Domain.Catch;
using PocketFish.Domain.Collection;

namespace PocketFish.Game.Persistence
{
    /// <summary>
    /// Converts between saved <see cref="CatchLogEntry"/> values and domain <see cref="CatchRecord"/> values.
    /// </summary>
    public static class CatchLogConverter
    {
        /// <summary>
        /// Builds the saved form of a catch.
        /// </summary>
        /// <param name="record">The catch to save.</param>
        /// <returns>The entry to store in the log.</returns>
        public static CatchLogEntry ToEntry(in CatchRecord record)
        {
            return new CatchLogEntry
            {
                SpeciesId = record.SpeciesId,
                Quality = (int)record.Quality,
                VariantId = record.VariantId,
                CaughtUtcTicks = record.CaughtUtc.Ticks,
            };
        }

        /// <summary>
        /// Reads a saved entry back into a catch.
        /// </summary>
        /// <param name="entry">The saved entry.</param>
        /// <param name="record">The catch, valid only when this returns true.</param>
        /// <returns>False if the entry is missing, has no species id, or has an unknown quality or time.</returns>
        public static bool TryToRecord(CatchLogEntry entry, out CatchRecord record)
        {
            record = default;
            if (entry == null || string.IsNullOrEmpty(entry.SpeciesId)) return false;
            if (!Enum.IsDefined(typeof(Quality), entry.Quality)) return false;
            if (entry.CaughtUtcTicks < DateTime.MinValue.Ticks || entry.CaughtUtcTicks > DateTime.MaxValue.Ticks) return false;

            record = new CatchRecord(
                entry.SpeciesId,
                (Quality)entry.Quality,
                entry.VariantId,
                new DateTime(entry.CaughtUtcTicks, DateTimeKind.Utc));
            return true;
        }
    }
}
