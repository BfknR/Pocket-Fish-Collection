using System;
using System.Collections.Generic;

namespace PocketFish.Domain.Fishing
{
    /// <summary>
    /// Runs a sequence of <see cref="TapMinigame"/> attempts and averages their scores.
    /// Each attempt uses the tier for its position in the sequence, whatever the fish.
    /// The marker is not reset between taps: the next attempt carries on from where the last one left it.
    /// Time is injected through <see cref="Tick"/>.
    /// </summary>
    public sealed class MultiTapMinigame
    {
        private readonly MinigameConfig[] _tiers;
        private readonly int _tapCount;
        private readonly TapGradeScores _scores;
        private readonly GradeThresholds _thresholds;
        private readonly TapMinigame _current = new TapMinigame();
        private readonly List<TapAttemptResult> _attempts = new List<TapAttemptResult>();

        /// <summary>
        /// Creates a multi-tap minigame. Call <see cref="Begin"/> to start it.
        /// </summary>
        /// <param name="tapCount">Number of taps, from the species rarity.</param>
        /// <param name="tiers">Difficulty by tap position. Needs at least <paramref name="tapCount"/> entries.</param>
        /// <param name="scores">Score for each grade.</param>
        /// <param name="thresholds">Average score to final grade boundaries.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the tap count is below 1.</exception>
        /// <exception cref="ArgumentException">Thrown when there are fewer tiers than taps.</exception>
        public MultiTapMinigame(int tapCount, IReadOnlyList<MinigameConfig> tiers,
            TapGradeScores scores, GradeThresholds thresholds)
        {
            if (tiers == null) throw new ArgumentNullException(nameof(tiers));
            if (tapCount < 1) throw new ArgumentOutOfRangeException(nameof(tapCount), tapCount, "Needs at least one tap.");
            if (tiers.Count < tapCount) throw new ArgumentException("Fewer tiers than taps.", nameof(tiers));

            _tapCount = tapCount;
            _tiers = new MinigameConfig[tapCount];
            for (int i = 0; i < tapCount; i++) _tiers[i] = tiers[i];
            _scores = scores;
            _thresholds = thresholds;
        }

        /// <summary>True once every attempt has been played.</summary>
        public bool IsFinished => _attempts.Count >= _tapCount;

        /// <summary>Zero-based index of the attempt in play. Equals the tap count once finished.</summary>
        public int CurrentIndex => _attempts.Count;

        /// <summary>Marker position of the attempt in play, 0 to 1. For display only.</summary>
        public float MarkerPosition => _current.MarkerPosition;

        /// <summary>Time progress of the attempt in play, 0 to 1. For display only.</summary>
        public float Progress => _current.Progress;

        /// <summary>Difficulty of the attempt in play, so the view can draw its zones. Not valid once finished.</summary>
        public MinigameConfig CurrentTier => _tiers[Math.Min(_attempts.Count, _tapCount - 1)];

        /// <summary>The final result. Only valid once <see cref="IsFinished"/> is true.</summary>
        public MinigameResult Result { get; private set; }

        /// <summary>Resets and starts the first attempt.</summary>
        public void Begin()
        {
            _attempts.Clear();
            Result = null;
            _current.Begin(_tiers[0]);
        }

        /// <summary>
        /// Advances the attempt in play. A timeout records a Miss and starts the next attempt.
        /// </summary>
        /// <param name="deltaSeconds">Seconds since the previous tick.</param>
        public void Tick(float deltaSeconds)
        {
            if (IsFinished) return;

            _current.Tick(deltaSeconds);
            if (_current.IsFinished) RecordAttempt();
        }

        /// <summary>Registers a tap on the attempt in play. Ignored once finished.</summary>
        public void Tap()
        {
            if (IsFinished) return;

            _current.Tap();
            RecordAttempt();
        }

        /// <summary>Stores the finished attempt, then starts the next one or builds the result.</summary>
        private void RecordAttempt()
        {
            TapGrade grade = _current.Result;
            _attempts.Add(new TapAttemptResult(_attempts.Count, grade, _scores.ScoreFor(grade)));

            if (_attempts.Count < _tapCount)
            {
                // The marker carries on from where this tap left it, at the next tier's speed.
                _current.Begin(_tiers[_attempts.Count], _current.Phase);
                return;
            }

            float sum = 0f;
            for (int i = 0; i < _attempts.Count; i++) sum += _attempts[i].Score;
            float average = sum / _attempts.Count;
            Result = new MinigameResult(_attempts, average, _thresholds.GradeFor(average));
        }
    }
}
