namespace PocketFish.Domain.Fishing
{
    /// <summary>
    /// Difficulty for one minigame run. Speed comes from the fish, the time limit comes from tuning.
    /// </summary>
    public readonly struct MinigameConfig
    {
        /// <summary>
        /// Creates a minigame configuration.
        /// </summary>
        /// <param name="sweepSeconds">Seconds for the marker to cross the bar one way.</param>
        /// <param name="timeLimitSeconds">Seconds before a no-tap Miss.</param>
        /// <param name="goodHalfWidth">Half-width of the Good zone, normalized to the bar.</param>
        /// <param name="greatHalfWidth">Half-width of the Great zone, normalized to the bar.</param>
        /// <param name="perfectHalfWidth">Half-width of the Perfect zone, normalized to the bar.</param>
        public MinigameConfig(float sweepSeconds, float timeLimitSeconds,
            float goodHalfWidth, float greatHalfWidth, float perfectHalfWidth)
        {
            SweepSeconds = sweepSeconds;
            TimeLimitSeconds = timeLimitSeconds;
            GoodHalfWidth = goodHalfWidth;
            GreatHalfWidth = greatHalfWidth;
            PerfectHalfWidth = perfectHalfWidth;
        }

        /// <summary>Seconds for the marker to cross the bar one way. Smaller is faster.</summary>
        public float SweepSeconds { get; }

        /// <summary>Fixed time before a no-tap Miss. The same for every fish.</summary>
        public float TimeLimitSeconds { get; }

        /// <summary>Half-width of the Good zone, normalized to the bar. Must contain the Great zone.</summary>
        public float GoodHalfWidth { get; }

        /// <summary>Half-width of the Great zone, normalized to the bar. Must contain the Perfect zone.</summary>
        public float GreatHalfWidth { get; }

        /// <summary>Half-width of the Perfect zone, normalized to the bar.</summary>
        public float PerfectHalfWidth { get; }
    }
}
