using System.Collections.Generic;
using PocketFish.Domain.Progression;
using PocketFish.Game.Collection;
using PocketFish.Game.Data;
using PocketFish.Game.Persistence;
using PocketFish.Game.Progression;
using PocketFish.Game.UI;
using UnityEngine;

namespace PocketFish.Game.Aquarium
{
    /// <summary>
    /// The lobby scene the game opens on. Loads the player's collection and progress, shows the selected
    /// aquarium's fish swimming in the tank, and wires up the coin counter, aquarium selector and upgrade panel.
    /// The Go Fish, Collection and Upgrades buttons are wired in the scene.
    /// </summary>
    public sealed class AquariumController : MonoBehaviour
    {
        /// <summary>Space kept between the fish and the edges of the screen, in world units.</summary>
        private const float EdgeMargin = 1f;

        [SerializeField] private SpeciesCatalog _catalog;
        [SerializeField] private AquariumCatalog _aquariums;
        [SerializeField] private CollectionBookView _book;
        [SerializeField] private CoinCounterView _coins;
        [SerializeField] private AquariumSelectorView _selector;
        [SerializeField] private PondCounterView _pondCounter;
        [SerializeField] private UpgradePanelView _upgrades;
        [SerializeField] private Camera _camera;
        [Tooltip("Where the swimming fish are created. Falls back to this object if empty.")]
        [SerializeField] private Transform _fishParent;

        [Header("Tank")]
        [SerializeField, Min(0)] private int _fishCount = 8;
        [Tooltip("Min and max swimming speed in world units per second. Each fish picks its own.")]
        [SerializeField] private Vector2 _speedRange = new Vector2(0.5f, 1.4f);

        [Header("Depth")]
        [SerializeField, Range(0.1f, 1f)] private float _deepestScale = 0.45f;
        [SerializeField] private Color _deepestTint = new Color(0.45f, 0.65f, 0.85f);
        [SerializeField] private Vector2 _depthSpeedRange = new Vector2(0.03f, 0.1f);

        private readonly List<GameObject> _fish = new List<GameObject>();
        private PlayerProgress _progress;
        private int _populatedFor = -1;

        private void Awake()
        {
            var collection = new CollectionSession(_catalog.ToDomainList(), CatchLogStore.Load());
            _book.Bind(collection, _catalog);

            _progress = ProgressSession.Get(_aquariums);
            _coins.Bind(_progress);
            _selector.Bind(_progress, _aquariums);
            _pondCounter.Bind(_progress, _aquariums);
            _upgrades.Bind(_progress, _aquariums);

            _progress.Changed += OnProgressChanged;
            Populate();
        }

        private void OnDestroy()
        {
            if (_progress != null) _progress.Changed -= OnProgressChanged;
        }

        /// <summary>Swaps the tank's fish when the player switches aquarium. Saves the choice.</summary>
        private void OnProgressChanged()
        {
            if (_progress.SelectedIndex == _populatedFor) return;

            Populate();
            ProgressSession.Save();
        }

        /// <summary>Fills the tank with <see cref="_fishCount"/> fish, each a random one from the selected aquarium's pond.</summary>
        private void Populate()
        {
            foreach (GameObject fish in _fish) Destroy(fish);
            _fish.Clear();

            _populatedFor = _progress.SelectedIndex;
            var sprites = new List<Sprite>();
            foreach (PondDefinition.Entry entry in _aquariums.Aquariums[_populatedFor].Pond.Fish)
            {
                Sprite sprite = entry.Fish != null ? _catalog.SpriteFor(entry.Fish) : null;
                if (sprite != null) sprites.Add(sprite);
            }
            if (sprites.Count == 0) return;

            float halfHeight = _camera.orthographicSize;
            float halfWidth = halfHeight * _camera.aspect;
            Vector3 center = _camera.transform.position;
            var bounds = Rect.MinMaxRect(
                center.x - halfWidth + EdgeMargin, center.y - halfHeight + EdgeMargin,
                center.x + halfWidth - EdgeMargin, center.y + halfHeight - EdgeMargin);

            for (int i = 0; i < _fishCount; i++)
            {
                var go = new GameObject("Fish");
                go.transform.SetParent(_fishParent != null ? _fishParent : transform, false);
                go.AddComponent<SpriteRenderer>();
                go.AddComponent<AquariumFish>().Initialize(
                    sprites[Random.Range(0, sprites.Count)], bounds, _speedRange, _deepestScale, _deepestTint, _depthSpeedRange);
                _fish.Add(go);
            }
        }
    }
}
