using System;

namespace PocketFish.Domain.Catch
{
    /// <summary>
    /// Pure odds logic for catches. Takes an injected RNG so results are reproducible in tests.
    /// </summary>
    public static class CatchRoller
    {
        /// <summary>
        /// Picks an index from a list of weights, with each index chosen proportionally to its weight.
        /// </summary>
        /// <param name="weights">Relative weights. They don't need to sum to any particular value.</param>
        /// <param name="rng">Random source. Inject a seeded instance in tests.</param>
        /// <returns>The chosen index into <paramref name="weights"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the weights are null, empty, or sum to zero.</exception>
        public static int PickWeightedIndex(float[] weights, Random rng)
        {
            if (weights == null || weights.Length == 0)
                throw new ArgumentException("Weights are empty.", nameof(weights));

            float total = 0f;
            for (int i = 0; i < weights.Length; i++) total += weights[i];
            if (total <= 0f)
                throw new ArgumentException("Weights sum to zero.", nameof(weights));

            float roll = (float)rng.NextDouble() * total;
            for (int i = 0; i < weights.Length; i++)
            {
                roll -= weights[i];
                if (roll < 0f) return i;
            }
            return weights.Length - 1;
        }

        /// <summary>
        /// Rolls the quality of a catch using weighted odds.
        /// </summary>
        /// <param name="odds">Relative weights for each quality tier.</param>
        /// <param name="rng">Random source. Inject a seeded instance in tests.</param>
        /// <returns>The rolled <see cref="Quality"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the odds sum to zero.</exception>
        public static Quality PickQuality(QualityOdds odds, Random rng)
        {
            float total = odds.Normal + odds.Good + odds.Great + odds.Perfect;
            if (total <= 0f)
                throw new ArgumentException("Quality odds sum to zero.", nameof(odds));

            float roll = (float)rng.NextDouble() * total;
            if ((roll -= odds.Normal) < 0f) return Quality.Normal;
            if ((roll -= odds.Good) < 0f) return Quality.Good;
            if ((roll -= odds.Great) < 0f) return Quality.Great;
            if (roll - odds.Perfect < 0f) return Quality.Perfect;

            // Float rounding can leave the roll past the last bucket. Fall back to the highest quality
            // that actually has weight, never to one with zero weight.
            if (odds.Perfect > 0f) return Quality.Perfect;
            if (odds.Great > 0f) return Quality.Great;
            if (odds.Good > 0f) return Quality.Good;
            return Quality.Normal;
        }
    }
}
