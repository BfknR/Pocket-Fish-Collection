using System;

namespace PocketFish.Domain.Fishing
{
    /// <summary>
    /// Timing minigame. The marker bounces across the bar, one pass per SweepSeconds.
    /// The minigame ends at the first tap, or at the fixed time limit as a Miss.
    /// </summary>
    public sealed class TapMinigame : IMinigame
    {
        private const float TargetCenter = 0.5f;

        private MinigameConfig _config;
        private float _elapsed;

        /// <inheritdoc/>
        public bool IsFinished { get; private set; }

        /// <inheritdoc/>
        public TapGrade Result { get; private set; }

        /// <summary>0 at the start, 1 at the time limit.</summary>
        public float Progress => IsFinished ? 1f : Math.Min(_elapsed / _config.TimeLimitSeconds, 1f);

        /// <summary>Marker position across the bar, 0 to 1. Frozen once the minigame ends.</summary>
        public float MarkerPosition
        {
            get
            {
                float cycle = (_elapsed / _config.SweepSeconds) % 2f; // 0 to 2 per round trip
                return cycle <= 1f ? cycle : 2f - cycle;
            }
        }

        /// <inheritdoc/>
        public void Begin(MinigameConfig config)
        {
            _config = config;
            _elapsed = 0f;
            IsFinished = false;
            Result = TapGrade.Miss;
        }

        /// <summary>
        /// Advances the marker. Finishes the minigame as a Miss when the time limit is reached.
        /// </summary>
        /// <param name="deltaSeconds">Seconds since the previous tick.</param>
        public void Tick(float deltaSeconds)
        {
            if (IsFinished) return;

            _elapsed += deltaSeconds;
            if (_elapsed >= _config.TimeLimitSeconds) Finish(TapGrade.Miss);
        }

        /// <summary>
        /// Grades the tap by how close the marker is to the target center and ends the minigame.
        /// </summary>
        public void Tap()
        {
            if (IsFinished) return;

            float distance = Math.Abs(MarkerPosition - TargetCenter);
            Finish(GradeFor(distance));
        }

        /// <summary>Maps a distance from the target center to a grade, checking the narrowest zone first.</summary>
        private TapGrade GradeFor(float distanceFromCenter)
        {
            if (distanceFromCenter <= _config.PerfectHalfWidth) return TapGrade.Perfect;
            if (distanceFromCenter <= _config.GreatHalfWidth) return TapGrade.Great;
            if (distanceFromCenter <= _config.GoodHalfWidth) return TapGrade.Good;
            return TapGrade.Miss;
        }

        /// <summary>Stores the result and marks the minigame as ended.</summary>
        private void Finish(TapGrade grade)
        {
            Result = grade;
            IsFinished = true;
        }
    }
}
