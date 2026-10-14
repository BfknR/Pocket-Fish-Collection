using UnityEngine;

namespace PocketFish.Game.UI
{
    /// <summary>
    /// How species art is drawn, shared by the grid tiles and the detail screen.
    /// </summary>
    public static class SpeciesArtStyle
    {
        /// <summary>Tint that turns a species' sprite into a silhouette while it is undiscovered.</summary>
        public static readonly Color SilhouetteColor = Color.black;

        /// <summary>
        /// Gets the tint for a species' art.
        /// </summary>
        /// <param name="isDiscovered">Whether the species has been caught.</param>
        /// <returns>White for discovered art, the silhouette color otherwise.</returns>
        public static Color TintFor(bool isDiscovered) => isDiscovered ? Color.white : SilhouetteColor;
    }
}
