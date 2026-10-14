namespace PocketFish.Domain.Catch
{
    /// <summary>
    /// Quality tier of a caught fish. Rolled from the odds that belong to the player's tap grade.
    /// </summary>
    public enum Quality
    {
        /// <summary>The base quality.</summary>
        Normal,
        /// <summary>Better than normal.</summary>
        Good,
        /// <summary>Better than good.</summary>
        Great,
        /// <summary>The best quality.</summary>
        Perfect
    }
}
