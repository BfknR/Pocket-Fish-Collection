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
        public QualityOdds(float normal, float good, float great, float perfect)
        {
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
    }
}
