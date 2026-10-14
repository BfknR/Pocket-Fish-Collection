using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PocketFish.Game.UI
{
    /// <summary>
    /// Draws a ring as coloured bands. The caller supplies the angles, so the theme controls the shape.
    /// Rebuilt only when bands change, not every frame.
    /// </summary>
    public sealed class TimingArcGraphic : Graphic
    {
        /// <summary>
        /// One coloured stretch of the arc, in positions from 0 (start) to 1 (end).
        /// </summary>
        public readonly struct Band
        {
            /// <summary>
            /// Creates a band.
            /// </summary>
            /// <param name="start">Where the band starts along the arc, 0 to 1.</param>
            /// <param name="end">Where the band ends along the arc, 0 to 1.</param>
            /// <param name="color">Fill colour of the band.</param>
            public Band(float start, float end, Color color)
            {
                Start = start;
                End = end;
                Color = color;
            }

            /// <summary>Where the band starts along the arc, 0 to 1.</summary>
            public float Start { get; }

            /// <summary>Where the band ends along the arc, 0 to 1.</summary>
            public float End { get; }

            /// <summary>Fill colour of the band.</summary>
            public Color Color { get; }
        }

        private const float DegreesPerSegment = 2f;

        [SerializeField, Range(0.1f, 0.95f)] private float _innerRadiusFraction = 0.75f;

        private readonly List<Band> _bands = new List<Band>();
        private float _startDegrees;
        private float _spanDegrees;

        /// <summary>
        /// Replaces the bands and the arc's angles, and queues a mesh rebuild.
        /// </summary>
        /// <param name="bands">Bands to draw. Later bands are drawn on top of earlier ones.</param>
        /// <param name="startDegrees">Angle of the arc's start, clockwise from straight up.</param>
        /// <param name="spanDegrees">Total angle the arc covers.</param>
        public void SetBands(IReadOnlyList<Band> bands, float startDegrees, float spanDegrees)
        {
            _bands.Clear();
            _bands.AddRange(bands);
            _startDegrees = startDegrees;
            _spanDegrees = spanDegrees;
            SetVerticesDirty();
        }

        /// <summary>Rebuilds the mesh. Called by Unity's UI system when the graphic is dirty.</summary>
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            Rect rect = rectTransform.rect;
            float outer = Mathf.Min(rect.width, rect.height) * 0.5f;
            float inner = outer * _innerRadiusFraction;

            foreach (Band band in _bands)
                AddBand(vh, band, inner, outer);
        }

        /// <summary>Adds the triangle strip for one band, between the inner and outer radius.</summary>
        private void AddBand(VertexHelper vh, Band band, float inner, float outer)
        {
            float startDeg = AngleAt(band.Start);
            float endDeg = AngleAt(band.End);
            int segments = Mathf.Max(1, Mathf.CeilToInt((endDeg - startDeg) / DegreesPerSegment));
            int baseIndex = vh.currentVertCount;

            for (int i = 0; i <= segments; i++)
            {
                float deg = Mathf.Lerp(startDeg, endDeg, (float)i / segments);
                Vector2 dir = Direction(deg);
                vh.AddVert(dir * outer, band.Color, Vector2.zero);
                vh.AddVert(dir * inner, band.Color, Vector2.zero);
            }

            for (int i = 0; i < segments; i++)
            {
                int outer0 = baseIndex + i * 2;
                int inner0 = outer0 + 1;
                int outer1 = outer0 + 2;
                int inner1 = outer0 + 3;
                vh.AddTriangle(outer0, inner0, outer1);
                vh.AddTriangle(outer1, inner0, inner1);
            }
        }

        /// <summary>Converts an arc position (0 to 1) to degrees clockwise from straight up.</summary>
        private float AngleAt(float position) =>
            _startDegrees + Mathf.Clamp01(position) * _spanDegrees;

        /// <summary>Unit vector for an angle measured clockwise from straight up.</summary>
        private static Vector2 Direction(float degreesClockwiseFromUp)
        {
            float rad = degreesClockwiseFromUp * Mathf.Deg2Rad;
            return new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
        }
    }
}
