namespace PocketFish.Domain.Fishing
{
    /// <summary>
    /// How accurate the player's tap was in the timing minigame. Selects which quality odds are used.
    /// </summary>
    public enum TapGrade
    {
        /// <summary>Tapped outside every target zone, or never tapped before the time limit.</summary>
        Miss,
        /// <summary>Tapped inside the outer target zone.</summary>
        Good,
        /// <summary>Tapped inside the middle target zone.</summary>
        Great,
        /// <summary>Tapped inside the innermost target zone.</summary>
        Perfect
    }
}
