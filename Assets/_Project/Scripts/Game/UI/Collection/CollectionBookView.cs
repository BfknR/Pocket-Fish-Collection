using PocketFish.Domain.Collection;
using PocketFish.Game.Data;
using PocketFish.Game.Collection;
using UnityEngine;

namespace PocketFish.Game.UI
{
    /// <summary>
    /// Presentation only. The Collection Book screen: opens over the aquarium, shows the profile header and
    /// species grid, and opens the detail screen for a tapped species.
    /// </summary>
    public sealed class CollectionBookView : MonoBehaviour
    {
        private const string PlayerName = "Player";

        [SerializeField] private GameObject _root;
        [SerializeField] private ProfileHeaderView _header;
        [SerializeField] private SpeciesGridView _grid;
        [SerializeField] private SpeciesDetailView _detail;

        private CollectionSession _collection;
        private SpeciesCatalog _catalog;

        /// <summary>True while the book, or the detail screen over it, is showing.</summary>
        public bool IsOpen => _root.activeSelf;

        /// <summary>
        /// Gives the book the collection and catalog it shows. Call once when the scene starts.
        /// </summary>
        /// <param name="collection">The player's collection.</param>
        /// <param name="catalog">Every species in the game.</param>
        public void Bind(CollectionSession collection, SpeciesCatalog catalog)
        {
            _collection = collection;
            _catalog = catalog;
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
            var progress = _collection.Progress;
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
            if (!_collection.Progress.TryGetRecord(speciesId, out SpeciesRecord record)) return;

            FishDefinition definition = _catalog.Find(speciesId);
            string description = definition != null ? definition.Description : string.Empty;
            _detail.Show(SpeciesDetailModel.From(record, _catalog.SpriteFor(definition), description));
        }
    }
}
