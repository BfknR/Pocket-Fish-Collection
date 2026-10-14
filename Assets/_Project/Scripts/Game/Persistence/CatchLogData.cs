using System;
using System.Collections.Generic;

namespace PocketFish.Game.Persistence
{
    /// <summary>
    /// One saved catch. Plain fields so JsonUtility can serialize it.
    /// </summary>
    [Serializable]
    public sealed class CatchLogEntry
    {
        /// <summary>Id of the species that was caught.</summary>
        public string SpeciesId;

        /// <summary>The quality, stored as its enum number.</summary>
        public int Quality;

        /// <summary>Id of the variant, or null for the regular look.</summary>
        public string VariantId;

        /// <summary>UTC ticks of when the fish was caught.</summary>
        public long CaughtUtcTicks;
    }

    /// <summary>
    /// Every catch the player has made, serialized to JSON by <see cref="CatchLogStore"/>.
    /// </summary>
    [Serializable]
    public sealed class CatchLogData
    {
        /// <summary>Format version, for future migrations.</summary>
        public int Version = 1;

        /// <summary>All catches, oldest first.</summary>
        public List<CatchLogEntry> Entries = new List<CatchLogEntry>();
    }
}
