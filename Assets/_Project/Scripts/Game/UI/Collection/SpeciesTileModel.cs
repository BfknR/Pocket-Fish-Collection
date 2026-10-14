using PocketFish.Domain.Catch;
using PocketFish.Domain.Collection;
using UnityEngine;

namespace PocketFish.Game.UI
{
    /// <summary>
    /// Everything a grid tile shows, already decided. The tile view displays it and decides nothing.
    /// An undiscovered species has no name, no badge and no count, so the view can't leak them.
    /// </summary>
    public readonly struct SpeciesTileModel
    {
        private SpeciesTileModel(string speciesId, Sprite sprite, bool isDiscovered, string displayName,
            string qualityText, string countText, Rarity rarity)
        {
            SpeciesId = speciesId;
            Sprite = sprite;
            IsDiscovered = isDiscovered;
            DisplayName = displayName;
            QualityText = qualityText;
            CountText = countText;
            Rarity = rarity;
        }

        /// <summary>Id of the species, passed back when the tile is tapped.</summary>
        public string SpeciesId { get; }

        /// <summary>The fish's art. Drawn as a silhouette when the species is undiscovered.</summary>
        public Sprite Sprite { get; }

        /// <summary>True once the species has been caught.</summary>
        public bool IsDiscovered { get; }

        /// <summary>The species name, or empty while undiscovered.</summary>
        public string DisplayName { get; }

        /// <summary>The best quality caught, or empty while undiscovered.</summary>
        public string QualityText { get; }

        /// <summary>The caught count such as "x 3", or empty while undiscovered.</summary>
        public string CountText { get; }

        /// <summary>How rare the species is. Shown even while undiscovered.</summary>
        public Rarity Rarity { get; }

        /// <summary>
        /// Builds the tile for a species.
        /// </summary>
        /// <param name="record">The species' record in the collection.</param>
        /// <param name="sprite">The art to show, already resolved to the placeholder if the species has none.</param>
        /// <returns>The model to bind to a tile.</returns>
        public static SpeciesTileModel From(SpeciesRecord record, Sprite sprite)
        {
            FishSpecies species = record.Species;
            if (!record.IsDiscovered)
            {
                return new SpeciesTileModel(species.Id, sprite, false, string.Empty, string.Empty, string.Empty, species.Rarity);
            }

            return new SpeciesTileModel(species.Id, sprite, true, species.DisplayName,
                record.BestQuality.ToString(), $"x {record.Count}", species.Rarity);
        }
    }
}
