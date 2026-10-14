using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PocketFish.Game.UI
{
    /// <summary>
    /// Presentation only. The species detail screen: large art that collapses on scroll, with the info underneath.
    /// </summary>
    public sealed class SpeciesDetailView : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private Button _closeButton;
        [SerializeField] private CollapsingArtHandler _artHandler;
        [SerializeField] private Image _art;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _rarityText;
        [SerializeField] private GameObject _discoveredInfo;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _caughtText;
        [SerializeField] private TMP_Text _qualityCountsText;
        [SerializeField] private TMP_Text _bestQualityText;
        [SerializeField] private TMP_Text _firstCaughtText;
        [SerializeField] private TMP_Text _completionText;
        [SerializeField] private GameObject _lockedInfo;
        [SerializeField] private TMP_Text _lockedText;

        /// <summary>Raised when the screen is closed.</summary>
        public event Action Closed;

        /// <summary>Starts hidden and hooks up the Close button.</summary>
        private void Awake()
        {
            _closeButton.onClick.AddListener(Hide);
            _root.SetActive(false);
        }

        /// <summary>
        /// Fills in the screen, scrolls it to the top and shows it.
        /// </summary>
        /// <param name="model">What to show.</param>
        public void Show(SpeciesDetailModel model)
        {
            _art.sprite = model.Sprite;
            _art.enabled = model.Sprite != null;
            _art.color = SpeciesArtStyle.TintFor(model.IsDiscovered);

            _nameText.text = model.DisplayName;
            _rarityText.text = model.RarityText;

            _discoveredInfo.SetActive(model.IsDiscovered);
            _lockedInfo.SetActive(!model.IsDiscovered);
            _lockedText.text = SpeciesDetailModel.LockedMessage;

            _descriptionText.text = model.Description;
            _caughtText.text = model.CaughtText;
            _qualityCountsText.text = model.QualityCountsText;
            _bestQualityText.text = model.BestQualityText;
            _firstCaughtText.text = model.FirstCaughtText;
            _completionText.text = model.CompletionText;

            _root.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_root.transform);
            _artHandler.ResetToTop();
        }

        /// <summary>Hides the screen and tells listeners.</summary>
        public void Hide()
        {
            _root.SetActive(false);
            Closed?.Invoke();
        }
    }
}
