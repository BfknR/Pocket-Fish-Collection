using System;
using System.Collections.Generic;

namespace PocketFish.Game.Persistence
{
    /// <summary>
    /// The saved state of the pond, serialized to JSON by <see cref="PondSaveStore"/>.
    /// </summary>
    [Serializable]
    public sealed class PondSaveData
    {
        /// <summary>Species ids of the fish in the pond.</summary>
        public List<string> FishIds = new List<string>();

        /// <summary>Seconds elapsed toward the next fish spawn.</summary>
        public double TimerSeconds;

        /// <summary>UTC ticks of the last sync, used to credit time spent away.</summary>
        public long SavedUtcTicks;
    }
}
