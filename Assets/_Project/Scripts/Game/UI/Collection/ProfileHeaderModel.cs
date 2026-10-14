using PocketFish.Domain.Collection;

namespace PocketFish.Game.UI
{
    /// <summary>
    /// Everything the profile header shows, already formatted. The header view displays it and decides nothing.
    /// </summary>
    public readonly struct ProfileHeaderModel
    {
        private const string NoQuality = "-";

        private ProfileHeaderModel(string playerName, string speciesText, string catchesText, string bestQualityText)
        {
            PlayerName = playerName;
            SpeciesText = speciesText;
            CatchesText = catchesText;
            BestQualityText = bestQualityText;
        }

        /// <summary>The player's name.</summary>
        public string PlayerName { get; }

        /// <summary>Species caught out of all species, such as "3 / 6".</summary>
        public string SpeciesText { get; }

        /// <summary>Total catches.</summary>
        public string CatchesText { get; }

        /// <summary>The best quality ever caught, or "-" if nothing was caught.</summary>
        public string BestQualityText { get; }

        /// <summary>
        /// Builds the header for a collection.
        /// </summary>
        /// <param name="progress">The collection to summarize.</param>
        /// <param name="playerName">The name to show.</param>
        /// <returns>The model to bind to the header.</returns>
        public static ProfileHeaderModel From(CollectionProgress progress, string playerName)
        {
            return new ProfileHeaderModel(
                playerName,
                $"{progress.SpeciesDiscovered} / {progress.SpeciesTotal}",
                progress.TotalCatches.ToString(),
                progress.BestQuality.HasValue ? progress.BestQuality.Value.ToString() : NoQuality);
        }
    }
}
