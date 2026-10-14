using System;

namespace PocketFish.Domain.Fishing
{
    /// <summary>
    /// Time helpers for the pond's real-time spawning.
    /// </summary>
    public static class PondTime
    {
        /// <summary>
        /// Seconds between the last save and now. Never negative, so a clock
        /// set backwards or a missing save (0) gives zero elapsed time.
        /// </summary>
        /// <param name="savedUtcTicks">UTC ticks of the last sync. Zero or less means no save exists.</param>
        /// <param name="nowUtcTicks">UTC ticks of the current time.</param>
        /// <returns>Elapsed seconds, or 0 when there is no valid interval.</returns>
        public static double ElapsedSeconds(long savedUtcTicks, long nowUtcTicks)
        {
            if(savedUtcTicks <= 0) return 0;

            long ticks = nowUtcTicks - savedUtcTicks;
            if(ticks <= 0) return 0;

            return TimeSpan.FromTicks(ticks).TotalSeconds;
        }
    }
}
