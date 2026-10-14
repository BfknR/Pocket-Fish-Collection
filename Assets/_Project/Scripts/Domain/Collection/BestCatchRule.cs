namespace PocketFish.Domain.Collection
{
    /// <summary>
    /// Decides which of two catches of a species is the better one: higher quality first, then the earlier catch.
    /// </summary>
    public static class BestCatchRule
    {
        /// <summary>
        /// Checks whether a catch beats the current best. On equal quality the earlier catch wins, so a repeat never replaces the first. A full tie keeps the current best.
        /// </summary>
        /// <param name="candidate">The catch being considered.</param>
        /// <param name="current">The current best catch.</param>
        /// <returns>True if the candidate is strictly better.</returns>
        public static bool IsBetter(in CatchRecord candidate, in CatchRecord current)
        {
            if(candidate.Quality != current.Quality)
            {
                return candidate.Quality > current.Quality;
            }
            return candidate.CaughtUtc < current.CaughtUtc;
        }
    }
}
