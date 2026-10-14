namespace PocketFish.Domain.Fishing
{
    /// <summary>
    /// A catch minigame. Time is injected through <see cref="Tick"/>, never read from the engine,
    /// so implementations stay testable.
    /// </summary>
    public interface IMinigame
    {
        /// <summary>True once the minigame has ended, by a tap or by hitting the time limit.</summary>
        bool IsFinished { get; }

        /// <summary>0 at the start, 1 at the time limit. Drives the timer bar.</summary>
        float Progress { get; }

        /// <summary>0 at the left end, 1 at the right end. Drives the arrow. Frozen once the minigame ends.</summary>
        float MarkerPosition { get; }

        /// <summary>The grade earned. Only valid once <see cref="IsFinished"/> is true.</summary>
        TapGrade Result { get; }

        /// <summary>Resets the minigame and starts a new run with the given difficulty.</summary>
        /// <param name="config">Difficulty for this run.</param>
        void Begin(MinigameConfig config);

        /// <summary>Advances the minigame by the given amount of time.</summary>
        /// <param name="deltaSeconds">Seconds since the previous tick.</param>
        void Tick(float deltaSeconds);

        /// <summary>Registers a player tap. Ignored once the minigame is finished.</summary>
        void Tap();
    }
}
