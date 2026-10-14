using PocketFish.Domain.Collection;
using PocketFish.Game.Data;
using PocketFish.Game.Fishing;
using UnityEngine;

namespace PocketFish.Game.UI
{
    /// <summary>
    /// Presentation only. The Collection Book screen: opens over the pond, shows the profile header and
    /// species grid, and opens the detail screen for a tapped species.
    /// </summary>
    public sealed class CollectionBookView : MonoBehaviour
    {
        private const string PlayerName = "Player";

        [SerializeField] private GameObject _root;
        [SerializeField] private RectTransform _entryButton;
        [SerializeField] private ProfileHeaderView _header;
        [SerializeField] private SpeciesGridView _grid;
        [SerializeField] private SpeciesDetailView _detail;
        [SerializeField] private PondSession _session;
        [SerializeField] private SpeciesCatalog _catalog;

        /// <summary>True while the book, or the detail screen over it, is showing.</summary>
        public bool IsOpen => _root.activeSelf;

        /// <summary>
        /// Shows or hides the button that opens the book, such as while a minigame is running.
        /// </summary>
        /// <param name="available">True to show the button.</param>
        public void SetEntryAvailable(bool available) => _entryButton.gameObject.SetActive(available);

        /// <summary>
        /// Checks whether a screen press landed on the entry button, so the pond can ignore it.
        /// The canvas is screen space overlay, so no camera is needed.
        /// </summary>
        /// <param name="screenPoint">The press position in screen pixels.</param>
        /// <returns>True if the button is showing and the point is inside it.</returns>
        public bool IsOverEntryButton(Vector2 screenPoint)
        {
            return _entryButton.gameObject.activeInHierarchy
                && RectTransformUtility.RectangleContainsScreenPoint(_entryButton, screenPoint, null);
        }

        /// <summary>Starts hidden and listens for taps on the grid.</summary>
        private void Awake()
        {
            _root.SetActive(false);
            _grid.SpeciesSelected += OpenDetail;
        }

        private void OnDestroy()
        {
            if (_grid != null) _grid.SpeciesSelected -= OpenDetail;
        }

        /// <summary>Refreshes the header and grid from the current collection and shows the screen.</summary>
        public void Open()
        {
            var progress = _session.Collection.Progress;
            _header.Bind(ProfileHeaderModel.From(progress, PlayerName));
            _grid.Bind(progress, _catalog);
            _root.SetActive(true);
        }

        /// <summary>Hides the screen.</summary>
        public void Close() => _root.SetActive(false);

        /// <summary>
        /// Shows the detail screen for a species. The grid stays as it is underneath,
        /// so closing the detail returns to the same scroll position.
        /// </summary>
        /// <param name="speciesId">Id of the tapped species.</param>
        private void OpenDetail(string speciesId)
        {
            if (!_session.Collection.Progress.TryGetRecord(speciesId, out SpeciesRecord record)) return;

            FishDefinition definition = _catalog.Find(speciesId);
            string description = definition != null ? definition.Description : string.Empty;
            _detail.Show(SpeciesDetailModel.From(record, _catalog.SpriteFor(definition), description));
        }
    }
}
