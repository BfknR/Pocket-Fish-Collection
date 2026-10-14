namespace PocketFish.Game.Data
{
    /// <summary>
    /// Size of a fish's shadow in the pond. Mapped to a scale by <see cref="CatchTuning.ShadowScaleFor"/>.
    /// </summary>
    public enum ShadowSize
    {
        /// <summary>Small shadow.</summary>
        Small,
        /// <summary>Medium shadow. Also the fallback for unknown values.</summary>
        Medium,
        /// <summary>Large shadow.</summary>
        Large
    }
}
