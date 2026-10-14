using PocketFish.Domain.Catch;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PocketFish.Game.UI
{
    /// <summary>
    /// Presentation only. Shows the result of a catch. Receives data, never decides anything.
    /// </summary>
    public sealed class CatchRevealView : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private Image _fishImage;
        [SerializeField] private TMP_Text _speciesText;
        [SerializeField] private TMP_Text _qualityText;
        [SerializeField] private TMP_Text _gradeText;

        /// <summary>
        /// Fills in the result and shows the panel.
        /// </summary>
        /// <param name="outcome">The catch to display.</param>
        /// <param name="sprite">The fish's picture. The image is hidden when this is null.</param>
        public void Show(CatchOutcome outcome, Sprite sprite)
        {
            _fishImage.sprite = sprite;
            _fishImage.enabled = sprite != null;

            _speciesText.text = outcome.Species.DisplayName;
            _qualityText.text = outcome.Quality.ToString();
            _gradeText.text = outcome.Grade.ToString();

            _root.SetActive(true);
        }

        /// <summary>Hides the panel.</summary>
        public void Hide() => _root.SetActive(false);
    }
}
