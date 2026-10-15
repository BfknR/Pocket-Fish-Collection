using PocketFish.Domain.Progression;
using PocketFish.Game.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PocketFish.Game.UI
{
    /// <summary>
    /// Presentation only. Shows the selected aquarium's name with arrows to switch between the ones the player owns.
    /// </summary>
    public sealed class AquariumSelectorView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private Button _previousButton;
        [SerializeField] private Button _nextButton;

        private PlayerProgress _progress;
        private AquariumCatalog _catalog;

        /// <summary>
        /// Starts showing and controlling the selected aquarium.
        /// </summary>
        /// <param name="progress">The progress to watch and change.</param>
        /// <param name="catalog">Names for the aquariums.</param>
        public void Bind(PlayerProgress progress, AquariumCatalog catalog)
        {
            _progress = progress;
            _catalog = catalog;
            _progress.Changed += Refresh;
            _previousButton.onClick.AddListener(() => _progress.Select(_progress.SelectedIndex - 1));
            _nextButton.onClick.AddListener(() => _progress.Select(_progress.SelectedIndex + 1));
            Refresh();
        }

        private void OnDestroy()
        {
            if (_progress != null) _progress.Changed -= Refresh;
        }

        private void Refresh()
        {
            _nameLabel.text = _catalog.Aquariums[_progress.SelectedIndex].DisplayName;
            _previousButton.interactable = _progress.IsOwned(_progress.SelectedIndex - 1);
            _nextButton.interactable = _progress.IsOwned(_progress.SelectedIndex + 1);
        }
    }
}
