using UnityEngine;
using UnityEngine.UI;

namespace PocketFish.Game.UI
{
    /// <summary>
    /// Presentation only. Shows how much of the minigame's time limit is left.
    /// </summary>
    public sealed class TimerBarView : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private Image _fill;

        /// <summary>Shows the bar, full.</summary>
        public void Show()
        {
            _root.SetActive(true);
            SetProgress(0f);
        }

        /// <summary>Hides the bar.</summary>
        public void Hide() => _root.SetActive(false);

        /// <summary>
        /// Updates the bar. The fill drains as progress increases.
        /// </summary>
        /// <param name="progress">0 at the start of the minigame, 1 at the time limit.</param>
        public void SetProgress(float progress) => _fill.fillAmount = Mathf.Clamp01(1f - progress);
    }
}
