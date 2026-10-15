using PocketFish.Game.Collection;
using PocketFish.Game.Data;
using PocketFish.Game.Persistence;
using PocketFish.Game.UI;
using UnityEngine;

namespace PocketFish.Game.Aquarium
{
    /// <summary>
    /// The lobby scene the game opens on. Loads the player's collection from the saved catch log,
    /// hands it to the Collection Book, and fills the tank with randomly chosen fish.
    /// The Go Fish and Collection buttons are wired in the scene.
    /// </summary>
    public sealed class AquariumController : MonoBehaviour
    {
        /// <summary>Space kept between the fish and the edges of the screen, in world units.</summary>
        private const float EdgeMargin = 1f;

        [SerializeField] private SpeciesCatalog _catalog;
        [SerializeField] private CollectionBookView _book;
        [SerializeField] private Camera _camera;

        [Header("Tank")]
        [SerializeField, Min(0)] private int _fishCount = 8;

        [Tooltip("Min and max swimming speed in world units per second. Each fish picks its own.")]
        [SerializeField] private Vector2 _speedRange = new Vector2(0.5f, 1.4f);

        [Header("Depth")]
        [SerializeField, Range(0.1f, 1f)] private float _deepestScale = 0.45f;
        [SerializeField] private Color _deepestTint = new Color(0.45f, 0.65f, 0.85f);
        [SerializeField] private Vector2 _depthSpeedRange = new Vector2(0.03f, 0.1f);

        private void Awake()
        {
            var collection = new CollectionSession(_catalog.ToDomainList(), CatchLogStore.Load());
            _book.Bind(collection, _catalog);
            PopulateTank();
        }

        /// <summary>Fills the tank with <see cref="_fishCount"/> fish, each a random species from the catalog.</summary>
        private void PopulateTank()
        {
            var species = new System.Collections.Generic.List<Sprite>();
            foreach (FishDefinition definition in _catalog.Species)
            {
                Sprite sprite = _catalog.SpriteFor(definition);
                if (sprite != null) species.Add(sprite);
            }
            if (species.Count == 0) return;

            float halfHeight = _camera.orthographicSize;
            float halfWidth = halfHeight * _camera.aspect;
            Vector3 center = _camera.transform.position;
            var bounds = Rect.MinMaxRect(
                center.x - halfWidth + EdgeMargin, center.y - halfHeight + EdgeMargin,
                center.x + halfWidth - EdgeMargin, center.y + halfHeight - EdgeMargin);

            for (int i = 0; i < _fishCount; i++)
            {
                var go = new GameObject("Fish");
                go.transform.SetParent(transform, false);
                go.AddComponent<SpriteRenderer>();
                go.AddComponent<AquariumFish>().Initialize(
                    species[Random.Range(0, species.Count)], bounds, _speedRange, _deepestScale, _deepestTint, _depthSpeedRange);
            }
        }
    }
}
