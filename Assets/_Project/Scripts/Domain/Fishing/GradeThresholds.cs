using System;

namespace PocketFish.Domain.Fishing
{
    /// <summary>
    /// Minimum average scores for each final grade. Anything below the Good minimum is a Miss.
    /// </summary>
    public readonly struct GradeThresholds
    {
        /// <summary>
        /// Creates a set of thresholds.
        /// </summary>
        /// <param name="goodMin">Lowest average that earns Good.</param>
        /// <param name="greatMin">Lowest average that earns Great.</param>
        /// <param name="perfectMin">Lowest average that earns Perfect.</param>
        /// <exception cref="ArgumentException">Thrown when the minimums are not strictly ascending.</exception>
        public GradeThresholds(float goodMin, float greatMin, float perfectMin)
        {
            if (!(goodMin < greatMin && greatMin < perfectMin))
                throw new ArgumentException("Thresholds must be strictly ascending: good < great < perfect.");

            GoodMin = goodMin;
            GreatMin = greatMin;
            PerfectMin = perfectMin;
        }

        /// <summary>Lowest average that earns Good.</summary>
        public float GoodMin { get; }

        /// <summary>Lowest average that earns Great.</summary>
        public float GreatMin { get; }

        /// <summary>Lowest average that earns Perfect.</summary>
        public float PerfectMin { get; }

        /// <summary>
        /// Maps an average score to a grade. Each minimum is inclusive.
        /// </summary>
        /// <param name="average">The unrounded average score.</param>
        /// <returns>The final grade.</returns>
        public TapGrade GradeFor(float average)
        {
            if (average >= PerfectMin) return TapGrade.Perfect;
            if (average >= GreatMin) return TapGrade.Great;
            if (average >= GoodMin) return TapGrade.Good;
            return TapGrade.Miss;
        }
    }
}
