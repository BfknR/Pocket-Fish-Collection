using System;

namespace PocketFish.Domain.Fishing
{
    /// <summary>
    /// The score each <see cref="TapGrade"/> is worth. Scores are averaged across a minigame's taps.
    /// </summary>
    public readonly struct TapGradeScores
    {
        /// <summary>
        /// Creates a set of scores.
        /// </summary>
        /// <param name="miss">Score for a Miss, including a timeout.</param>
        /// <param name="good">Score for Good.</param>
        /// <param name="great">Score for Great.</param>
        /// <param name="perfect">Score for Perfect.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when a score is NaN, infinite or negative.</exception>
        public TapGradeScores(float miss, float good, float great, float perfect)
        {
            Miss = Validate(miss, nameof(miss));
            Good = Validate(good, nameof(good));
            Great = Validate(great, nameof(great));
            Perfect = Validate(perfect, nameof(perfect));
        }

        /// <summary>Score for a Miss, including a timeout.</summary>
        public float Miss { get; }

        /// <summary>Score for Good.</summary>
        public float Good { get; }

        /// <summary>Score for Great.</summary>
        public float Great { get; }

        /// <summary>Score for Perfect.</summary>
        public float Perfect { get; }

        /// <summary>
        /// Gets the score for a grade.
        /// </summary>
        /// <param name="grade">The grade to score.</param>
        /// <returns>The configured score.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown for an unknown grade.</exception>
        public float ScoreFor(TapGrade grade)
        {
            switch (grade)
            {
                case TapGrade.Miss: return Miss;
                case TapGrade.Good: return Good;
                case TapGrade.Great: return Great;
                case TapGrade.Perfect: return Perfect;
                default: throw new ArgumentOutOfRangeException(nameof(grade), grade, null);
            }
        }

        private static float Validate(float score, string name)
        {
            if (float.IsNaN(score) || float.IsInfinity(score) || score < 0f)
                throw new ArgumentOutOfRangeException(name, score, "Score must be a finite, non-negative number.");
            return score;
        }
    }
}
