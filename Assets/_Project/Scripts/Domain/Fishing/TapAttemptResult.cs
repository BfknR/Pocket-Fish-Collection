namespace PocketFish.Domain.Fishing
{
    /// <summary>
    /// The outcome of one tap in a multi-tap minigame.
    /// </summary>
    public readonly struct TapAttemptResult
    {
        /// <summary>
        /// Creates the result of one attempt.
        /// </summary>
        /// <param name="index">Zero-based position of the attempt in the sequence.</param>
        /// <param name="grade">Grade earned. A timed-out attempt is a Miss.</param>
        /// <param name="score">Score mapped from the grade.</param>
        public TapAttemptResult(int index, TapGrade grade, float score)
        {
            Index = index;
            Grade = grade;
            Score = score;
        }

        /// <summary>Zero-based position of the attempt in the sequence.</summary>
        public int Index { get; }

        /// <summary>Grade earned. A timed-out attempt is a Miss.</summary>
        public TapGrade Grade { get; }

        /// <summary>Score mapped from the grade.</summary>
        public float Score { get; }
    }
}
