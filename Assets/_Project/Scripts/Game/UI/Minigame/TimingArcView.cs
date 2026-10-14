using System.Collections.Generic;
using PocketFish.Domain.Fishing;
using UnityEngine;

namespace PocketFish.Game.UI
{
    /// <summary>
    /// Presentation only. Owns the look of the timing arc. Edit colours and shape on the prefab's Inspector.
    /// </summary>
    public sealed class TimingArcView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _root;
        [SerializeField] private TimingArcGraphic _arc;
        [SerializeField] private RectTransform _arrow;

        [Header("Colours")]
        [SerializeField] private Color _missColor = Color.black;
        [SerializeField] private Color _goodColor = new Color(1f, 0.55f, 0.1f);
        [SerializeField] private Color _greatColor = new Color(1f, 0.85f, 0.1f);
        [SerializeField] private Color _perfectColor = new Color(0.25f, 0.85f, 0.25f);

        [Header("Shape")]
        [Tooltip("Angle of the left end, clockwise from straight up. -90 is the left side.")]
        [SerializeField, Range(-180f, 180f)] private float _arcStartDegrees = -90f;

        [Tooltip("Total angle of the arc. 180 is a semicircle, 270 is three-quarters.")]
        [SerializeField, Range(10f, 360f)] private float _arcSpanDegrees = 180f;

        private readonly List<TimingArcGraphic.Band> _bands = new List<TimingArcGraphic.Band>(4);

        /// <summary>
        /// Draws the target zones for a minigame, resets the arrow and shows the arc.
        /// </summary>
        /// <param name="config">The minigame whose zone widths are drawn.</param>
        public void Show(MinigameConfig config)
        {
            BuildBands(config);
            SetMarker(0f);
            _root.SetActive(true);
        }

        /// <summary>Hides the arc.</summary>
        public void Hide() => _root.SetActive(false);

        /// <summary>
        /// Rotates the arrow to a position along the arc.
        /// </summary>
        /// <param name="position">0 to 1 across the arc. Values outside the range are clamped.</param>
        public void SetMarker(float position)
        {
            // Unity rotates counter-clockwise for positive angles, so negate the clockwise angle.
            float degrees = AngleFor(position);
            _arrow.localRotation = Quaternion.Euler(0f, 0f, -degrees);
        }

        /// <summary>Converts an arc position (0 to 1) to degrees clockwise from straight up.</summary>
        private float AngleFor(float position) =>
            _arcStartDegrees + Mathf.Clamp01(position) * _arcSpanDegrees;

        /// <summary>Builds the colored bands for the miss background and each target zone, then sends them to the arc.</summary>
        private void BuildBands(MinigameConfig config)
        {
            _bands.Clear();

            // Drawn outer to inner, so the narrower zones sit on top of the wider ones.
            _bands.Add(new TimingArcGraphic.Band(0f, 1f, _missColor));
            AddZone(config.GoodHalfWidth, _goodColor);
            AddZone(config.GreatHalfWidth, _greatColor);
            AddZone(config.PerfectHalfWidth, _perfectColor);

            _arc.SetBands(_bands, _arcStartDegrees, _arcSpanDegrees);
        }

        /// <summary>Adds a band centered on the arc, extending half-width to each side.</summary>
        private void AddZone(float halfWidth, Color color)
        {
            _bands.Add(new TimingArcGraphic.Band(0.5f - halfWidth, 0.5f + halfWidth, color));
        }
    }
}
