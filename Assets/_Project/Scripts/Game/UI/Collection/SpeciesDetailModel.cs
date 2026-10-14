using System;
using System.Text;
using PocketFish.Domain.Catch;
using PocketFish.Domain.Collection;
using UnityEngine;

namespace PocketFish.Game.UI
{
    /// <summary>
    /// Everything the species detail screen shows, already decided and formatted.
    /// An undiscovered species has no name, description, counts or records, so the view can't leak them.
    /// </summary>
    public readonly struct SpeciesDetailModel
    {
        /// <summary>Line shown in place of the info while a species is undiscovered.</summary>
        public const string LockedMessage = "Catch this species to reveal its info";

        private const string NoValue = "-";

        private SpeciesDetailModel(Sprite sprite, bool isDiscovered, string displayName, string rarityText,
            string description, string caughtText, string qualityCountsText, string bestQualityText,
            string firstCaughtText, string completionText)
        {
            Sprite = sprite;
            IsDiscovered = isDiscovered;
            DisplayName = displayName;
            RarityText = rarityText;
            Description = description;
            CaughtText = caughtText;
            QualityCountsText = qualityCountsText;
            BestQualityText = bestQualityText;
            FirstCaughtText = firstCaughtText;
            CompletionText = completionText;
        }

        /// <summary>The fish's art. Drawn as a silhouette when the species is undiscovered.</summary>
        public Sprite Sprite { get; }

        /// <summary>True once the species has been caught.</summary>
        public bool IsDiscovered { get; }

        /// <summary>The species name, or empty while undiscovered.</summary>
        public string DisplayName { get; }

        /// <summary>How rare the species is. Shown even while undiscovered.</summary>
        public string RarityText { get; }

        /// <summary>The species description, or empty while undiscovered.</summary>
        public string Description { get; }

        /// <summary>Total caught, such as "Caught: 3". Empty while undiscovered.</summary>
        public string CaughtText { get; }

        /// <summary>One line per quality with its count. Empty while undiscovered.</summary>
        public string QualityCountsText { get; }

        /// <summary>Best quality caught, such as "Best quality: Great". Empty while undiscovered.</summary>
        public string BestQualityText { get; }

        /// <summary>Date of the first catch. Empty while undiscovered.</summary>
        public string FirstCaughtText { get; }

        /// <summary>Qualities obtained out of all qualities, such as "Qualities: 2 / 4". Empty while undiscovered.</summary>
        public string CompletionText { get; }

        /// <summary>
        /// Builds the detail screen for a species.
        /// </summary>
        /// <param name="record">The species' record in the collection.</param>
        /// <param name="sprite">The art to show, already resolved to the placeholder if the species has none.</param>
        /// <param name="description">The species description from its definition.</param>
        /// <returns>The model to bind to the detail screen.</returns>
        public static SpeciesDetailModel From(SpeciesRecord record, Sprite sprite, string description)
        {
            FishSpecies species = record.Species;
            string rarity = species.Rarity.ToString();
            if (!record.IsDiscovered)
            {
                return new SpeciesDetailModel(sprite, false, string.Empty, rarity, string.Empty,
                    string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
            }

            Array qualities = Enum.GetValues(typeof(Quality));
            var counts = new StringBuilder();
            int obtained = 0;
            foreach (Quality quality in qualities)
            {
                int count = record.CountOf(quality);
                if (count > 0) obtained++;
                if (counts.Length > 0) counts.Append('\n');
                counts.Append(quality).Append(": ").Append(count);
            }

            return new SpeciesDetailModel(
                sprite,
                true,
                species.DisplayName,
                rarity,
                description,
                $"Caught: {record.Count}",
                counts.ToString(),
                $"Best quality: {record.BestQuality}",
                $"First caught: {record.FirstCaughtUtc.ToLocalTime().ToShortDateString()}",
                $"Qualities: {obtained} / {qualities.Length}");
        }
    }
}
