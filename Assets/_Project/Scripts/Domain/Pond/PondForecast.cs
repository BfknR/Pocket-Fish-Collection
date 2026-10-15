using System.Collections.Generic;
using PocketFish.Domain.Catch;

namespace PocketFish.Domain.Fishing
{
    /// <summary>
    /// Works out how many fish a pond holds right now from its saved state, without loading the pond.
    /// Uses the same spawn rules as <see cref="PondPopulation"/>, so the number matches what the player finds.
    /// </summary>
    public static class PondForecast
    {
        /// <summary>The state of a saved pond after time has passed.</summary>
        public readonly struct Result
        {
            /// <summary>Creates a result.</summary>
            public Result(int fishCount, double? secondsToNextFish)
            {
                FishCount = fishCount;
                SecondsToNextFish = secondsToNextFish;
            }

            /// <summary>Fish in the pond, from 0 to the capacity.</summary>
            public int FishCount { get; }

            /// <summary>Seconds until the next fish spawns, or null if the pond is full.</summary>
            public double? SecondsToNextFish { get; }
        }

        /// <summary>
        /// Counts the fish a saved pond has after time has passed.
        /// </summary>
        /// <param name="savedFishCount">Fish in the pond when it was saved. Anything above the capacity is dropped.</param>
        /// <param name="savedTimerSeconds">The spawn timer when it was saved.</param>
        /// <param name="elapsedSeconds">Seconds since the save. Zero or less adds nothing.</param>
        /// <param name="capacity">The pond's capacity.</param>
        /// <param name="spawnIntervalSeconds">The pond's spawn interval. Must be above zero.</param>
        /// <returns>The number of fish in the pond now, from 0 to the capacity.</returns>
        public static int FishCount(int savedFishCount, double savedTimerSeconds, double elapsedSeconds, int capacity, double spawnIntervalSeconds)
        {
            return Project(savedFishCount, savedTimerSeconds, elapsedSeconds, capacity, spawnIntervalSeconds).FishCount;
        }

        /// <summary>
        /// Works out the fish count and the wait for the next fish, for a saved pond after time has passed.
        /// </summary>
        /// <param name="savedFishCount">Fish in the pond when it was saved. Anything above the capacity is dropped.</param>
        /// <param name="savedTimerSeconds">The spawn timer when it was saved.</param>
        /// <param name="elapsedSeconds">Seconds since the save. Zero or less adds nothing.</param>
        /// <param name="capacity">The pond's capacity.</param>
        /// <param name="spawnIntervalSeconds">The pond's spawn interval. Must be above zero.</param>
        public static Result Project(int savedFishCount, double savedTimerSeconds, double elapsedSeconds, int capacity, double spawnIntervalSeconds)
        {
            // The species doesn't matter for counting, so a placeholder stands in for each fish.
            var placeholder = new FishSpecies("forecast", "forecast", Rarity.Common);
            var pond = new PondPopulation(capacity, spawnIntervalSeconds, () => placeholder);

            var saved = new List<FishSpecies>();
            for (int i = 0; i < savedFishCount; i++) saved.Add(placeholder);

            pond.Restore(saved, savedTimerSeconds);
            pond.Advance(elapsedSeconds);

            double? next = pond.Count >= capacity ? (double?)null : spawnIntervalSeconds - pond.TimerSeconds;
            return new Result(pond.Count, next);
        }
    }
}
