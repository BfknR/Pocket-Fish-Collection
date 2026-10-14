using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PocketFish.Game.UI
{
    /// <summary>
    /// Presentation only. One tile of the species grid. Shows a model and reports taps.
    /// </summary>
    public sealed class SpeciesTileView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _art;
        [SerializeField] private GameObject _unknownMarker;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _qualityText;
        [SerializeField] private TMP_Text _countText;

        private string _speciesId;

        /// <summary>Raised with the species id when the tile is tapped.</summary>
        public event Action<string> Tapped;

        /// <summary>Hooks up the tap.</summary>
        private void Awake()
        {
            _button.onClick.AddListener(OnClicked);
        }

        /// <summary>
        /// Shows a species. A discovered species shows its art, name, quality and count.
        /// An undiscovered one shows a black silhouette and a "?" marker.
        /// </summary>
        /// <param name="model">What to show.</param>
        public void Bind(SpeciesTileModel model)
        {
            _speciesId = model.SpeciesId;

            _art.sprite = model.Sprite;
            _art.enabled = model.Sprite != null;
            _art.color = SpeciesArtStyle.TintFor(model.IsDiscovered);

            _unknownMarker.SetActive(!model.IsDiscovered);
            _nameText.text = model.DisplayName;
            _qualityText.text = model.QualityText;
            _countText.text = model.CountText;
        }

        private void OnClicked() => Tapped?.Invoke(_speciesId);
    }
}
