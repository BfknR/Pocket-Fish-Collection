using System;
using System.Collections.Generic;
using PocketFish.Domain.Collection;
using PocketFish.Game.Data;
using UnityEngine;
using UnityEngine.UI;

namespace PocketFish.Game.UI
{
    /// <summary>
    /// Presentation only. A 3-column grid with one tile per species. Tile size comes from the width
    /// of the viewport, so tiles stay square on any screen.
    /// </summary>
    public sealed class SpeciesGridView : MonoBehaviour
    {
        private const int Columns = 3;

        [SerializeField] private RectTransform _viewport;
        [SerializeField] private GridLayoutGroup _grid;
        [SerializeField] private SpeciesTileView _tilePrefab;

        private readonly List<SpeciesTileView> _tiles = new List<SpeciesTileView>();

        /// <summary>Raised with the species id when a tile is tapped.</summary>
        public event Action<string> SpeciesSelected;

        /// <summary>
        /// Shows the collection. Tiles are created on the first call and reused after that.
        /// </summary>
        /// <param name="progress">The collection to show, in display order.</param>
        /// <param name="catalog">Gives each species its art, or the placeholder if it has none.</param>
        public void Bind(CollectionProgress progress, SpeciesCatalog catalog)
        {
            if (_tiles.Count != progress.SpeciesTotal) Rebuild(progress.SpeciesTotal);

            for (int i = 0; i < _tiles.Count; i++)
            {
                SpeciesRecord record = progress.Species[i];
                Sprite sprite = catalog.SpriteFor(catalog.Find(record.Species.Id));
                _tiles[i].Bind(SpeciesTileModel.From(record, sprite));
            }

            ApplyCellSize();
        }

        /// <summary>Keeps tiles square when the screen size or orientation changes.</summary>
        private void OnRectTransformDimensionsChange()
        {
            if (_grid != null) ApplyCellSize();
        }

        /// <summary>Destroys the old tiles and creates the requested number.</summary>
        private void Rebuild(int count)
        {
            foreach (SpeciesTileView tile in _tiles)
            {
                tile.Tapped -= OnTileTapped;
                Destroy(tile.gameObject);
            }
            _tiles.Clear();

            for (int i = 0; i < count; i++)
            {
                SpeciesTileView tile = Instantiate(_tilePrefab, _grid.transform);
                tile.Tapped += OnTileTapped;
                _tiles.Add(tile);
            }
        }

        /// <summary>Sizes the square cells so exactly three fit across the viewport.</summary>
        private void ApplyCellSize()
        {
            float usable = _viewport.rect.width - _grid.padding.horizontal - _grid.spacing.x * (Columns - 1);
            if (usable <= 0f) return;

            float size = Mathf.Floor(usable / Columns);
            _grid.cellSize = new Vector2(size, size);
        }

        private void OnTileTapped(string speciesId) => SpeciesSelected?.Invoke(speciesId);
    }
}
