using UnityEngine;
using UnityEngine.UI;

namespace PocketFish.Game.UI
{
    /// <summary>
    /// Presentation only. Keeps the detail art large at the top and shrinks it into a pinned thumbnail
    /// as the info scrolls up. Everything is driven by the scroll position, with no separate animation.
    /// </summary>
    public sealed class CollapsingArtHandler : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scroll;
        [SerializeField] private RectTransform _panel;
        [SerializeField] private RectTransform _art;
        [SerializeField] private Image _headerBar;
        [SerializeField] private LayoutElement _spacer;
        [SerializeField] private LayoutElement _contentMinHeight;

        [Header("Sizes")]
        [SerializeField, Range(0.2f, 0.6f)] private float _largeHeightFraction = 0.4f;
        [SerializeField, Min(40f)] private float _thumbnailSize = 160f;
        [SerializeField, Min(0f)] private float _largeTopInset = 120f;
        [SerializeField, Min(0f)] private float _margin = 24f;

        private ArtPose _large;
        private ArtPose _thumbnail;
        private float _collapseDistance;

        /// <summary>Starts following the scroll.</summary>
        private void OnEnable() => _scroll.onValueChanged.AddListener(OnScrolled);

        /// <summary>Stops following the scroll.</summary>
        private void OnDisable() => _scroll.onValueChanged.RemoveListener(OnScrolled);

        /// <summary>Recalculates the poses when the screen size changes.</summary>
        private void OnRectTransformDimensionsChange()
        {
            if (isActiveAndEnabled) Refresh();
        }

        /// <summary>
        /// Recalculates the poses from the panel size, resets the scroll to the top and shows the large art.
        /// </summary>
        public void ResetToTop()
        {
            _scroll.verticalNormalizedPosition = 1f;
            Refresh();
        }

        /// <summary>Recalculates the poses and applies the current scroll position.</summary>
        private void Refresh()
        {
            Rect panel = _panel.rect;
            float largeSize = Mathf.Min(panel.width - 2f * _margin, panel.height * _largeHeightFraction);

            _large = new ArtPose(largeSize, new Vector2(panel.width * 0.5f, -(_largeTopInset + largeSize * 0.5f)));
            _thumbnail = new ArtPose(_thumbnailSize,
                new Vector2(_margin + _thumbnailSize * 0.5f, -(_margin + _thumbnailSize * 0.5f)));
            _collapseDistance = largeSize - _thumbnailSize;

            _spacer.minHeight = _spacer.preferredHeight = _largeTopInset + largeSize + _margin;
            _headerBar.rectTransform.sizeDelta = new Vector2(0f, _thumbnailSize + 2f * _margin);
            _contentMinHeight.minHeight = _scroll.viewport.rect.height + _collapseDistance;

            Apply(CurrentProgress());
        }

        private void OnScrolled(Vector2 _) => Apply(CurrentProgress());

        private float CurrentProgress() =>
            CollapsingArtMath.Progress(_scroll.content.anchoredPosition.y, _collapseDistance);

        /// <summary>Moves and sizes the art, and fades the pinned bar in behind the thumbnail.</summary>
        private void Apply(float progress)
        {
            ArtPose pose = CollapsingArtMath.Evaluate(progress, _large, _thumbnail);
            _art.sizeDelta = new Vector2(pose.Size, pose.Size);
            _art.anchoredPosition = pose.Center;

            Color bar = _headerBar.color;
            bar.a = progress;
            _headerBar.color = bar;
        }
    }
}
