using System;
using System.Collections.Generic;

namespace PocketFish.Domain.Fishing
{
    /// <summary>
    /// The final result of a multi-tap minigame. Kept in memory only.
    /// </summary>
    public sealed class MinigameResult
    {
        private readonly TapAttemptResult[] _attempts;

        /// <summary>
        /// Creates a result.
        /// </summary>
        /// <param name="attempts">Every attempt, in the order they were played.</param>
        /// <param name="averageScore">Arithmetic mean of the attempt scores, unrounded.</param>
        /// <param name="finalGrade">Grade the average maps to.</param>
        public MinigameResult(IReadOnlyList<TapAttemptResult> attempts, float averageScore, TapGrade finalGrade)
        {
            if (attempts == null) throw new ArgumentNullException(nameof(attempts));

            _attempts = new TapAttemptResult[attempts.Count];
            for (int i = 0; i < _attempts.Length; i++) _attempts[i] = attempts[i];

            AverageScore = averageScore;
            FinalGrade = finalGrade;
        }

        /// <summary>Every attempt, in the order they were played.</summary>
        public IReadOnlyList<TapAttemptResult> Attempts => _attempts;

        /// <summary>Arithmetic mean of the attempt scores, unrounded.</summary>
        public float AverageScore { get; }

        /// <summary>Grade the average maps to. Selects the quality odds.</summary>
        public TapGrade FinalGrade { get; }
    }
}
