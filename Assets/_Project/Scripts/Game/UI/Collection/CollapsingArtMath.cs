using UnityEngine;

namespace PocketFish.Game.UI
{
    /// <summary>
    /// Where and how big the detail art is: a square given by its center and side length.
    /// </summary>
    public readonly struct ArtPose
    {
        /// <summary>
        /// Creates a pose.
        /// </summary>
        /// <param name="size">Side length of the square art.</param>
        /// <param name="center">Center of the art, in the panel's coordinates.</param>
        public ArtPose(float size, Vector2 center)
        {
            Size = size;
            Center = center;
        }

        /// <summary>Side length of the square art.</summary>
        public float Size { get; }

        /// <summary>Center of the art, in the panel's coordinates.</summary>
        public Vector2 Center { get; }
    }

    /// <summary>
    /// Turns scroll distance into the art's size and position. No Unity objects, no allocations.
    /// </summary>
    public static class CollapsingArtMath
    {
        /// <summary>
        /// Converts how far the info has scrolled into collapse progress.
        /// </summary>
        /// <param name="scrollOffset">Distance scrolled down from the top. Negative values from overscroll count as zero.</param>
        /// <param name="collapseDistance">Scroll distance at which the art is fully collapsed.</param>
        /// <returns>Progress from 0 (large) to 1 (thumbnail). One if the distance is zero or less.</returns>
        public static float Progress(float scrollOffset, float collapseDistance)
        {
            if (collapseDistance <= 0f) return 1f;
            return Mathf.Clamp01(scrollOffset / collapseDistance);
        }

        /// <summary>
        /// Blends between the large and thumbnail poses.
        /// </summary>
        /// <param name="progress">Collapse progress from 0 to 1.</param>
        /// <param name="large">The pose at progress 0.</param>
        /// <param name="thumbnail">The pose at progress 1.</param>
        /// <returns>The pose for this progress.</returns>
        public static ArtPose Evaluate(float progress, in ArtPose large, in ArtPose thumbnail)
        {
            float t = Mathf.Clamp01(progress);
            return new ArtPose(
                Mathf.Lerp(large.Size, thumbnail.Size, t),
                Vector2.Lerp(large.Center, thumbnail.Center, t));
        }
    }
}
