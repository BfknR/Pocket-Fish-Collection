using System;

namespace PocketFish.Domain.Catch
{
    /// <summary>
    /// Relative weights for each <see cref="Quality"/> tier. They don't need to sum to 100.
    /// </summary>
    public readonly struct QualityOdds
    {
        /// <summary>
        /// Creates a set of quality weights.
        /// </summary>
        /// <param name="normal">Weight of <see cref="Quality.Normal"/>.</param>
        /// <param name="good">Weight of <see cref="Quality.Good"/>.</param>
        /// <param name="great">Weight of <see cref="Quality.Great"/>.</param>
        /// <param name="perfect">Weight of <see cref="Quality.Perfect"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when a weight is NaN, infinite or negative, or when every weight is zero.
        /// </exception>
        public QualityOdds(float normal, float good, float great, float perfect)
        {
            Check(normal, nameof(normal));
            Check(good, nameof(good));
            Check(great, nameof(great));
            Check(perfect, nameof(perfect));
            if (normal + good + great + perfect <= 0f)
                throw new ArgumentOutOfRangeException(nameof(normal), "At least one weight must be above zero.");

            Normal = normal;
            Good = good;
            Great = great;
            Perfect = perfect;
        }

        /// <summary>Weight of <see cref="Quality.Normal"/>.</summary>
        public float Normal { get; }

        /// <summary>Weight of <see cref="Quality.Good"/>.</summary>
        public float Good { get; }

        /// <summary>Weight of <see cref="Quality.Great"/>.</summary>
        public float Great { get; }

        /// <summary>Weight of <see cref="Quality.Perfect"/>.</summary>
        public float Perfect { get; }

        private static void Check(float weight, string name)
        {
            if (float.IsNaN(weight) || float.IsInfinity(weight) || weight < 0f)
                throw new ArgumentOutOfRangeException(name, weight, "Weight must be a finite, non-negative number.");
        }
    }
}
