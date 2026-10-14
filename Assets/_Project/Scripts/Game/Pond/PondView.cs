using System.Collections.Generic;
using PocketFish.Domain.Fishing;
using PocketFish.Game.Data;
using UnityEngine;

namespace PocketFish.Game.Fishing
{
    /// <summary>
    /// Presentation only. Keeps one <see cref="FishView"/> on screen for each fish in the pond,
    /// spawning views for new fish and destroying views for fish that are gone.
    /// </summary>
    public sealed class PondView : MonoBehaviour
    {
        [SerializeField] private PondSession _session;
        [SerializeField] private FishView _fishPrefab;
        [SerializeField] private Vector2 _areaSize = new Vector2(8f, 6f);
        [SerializeField] private CatchTuning _tuning;

        private readonly Dictionary<int, FishView> _views = new Dictionary<int, FishView>();
        private readonly HashSet<int> _present = new HashSet<int>();

        /// <summary>Syncs the fish views with the population every frame.</summary>
        private void Update()
        {
            _present.Clear();
            foreach(PondFish fish in _session.Population.Fish)
            {
                _present.Add(fish.InstanceId);
                if (_views.ContainsKey(fish.InstanceId)) continue;

                FishDefinition definition = _session.DefinitionFor(fish);
                FishView view = Instantiate(_fishPrefab, transform);
                view.Initialize(fish, _tuning.ShadowScaleFor(definition.ShadowSize), Bounds(), RandomPoint());
                _views.Add(fish.InstanceId, view);
            }
            List<int> gone = new List<int>();
            foreach (KeyValuePair<int, FishView> pair in _views)
            {
                if (!_present.Contains(pair.Key)) gone.Add(pair.Key);
            }
            foreach (int id in gone)
            {
                Destroy(_views[id].gameObject);
                _views.Remove(id);
            }
        }

        /// <summary>The swimming area in world space, centered on this object.</summary>
        private Rect Bounds()
        {
            Vector2 center = transform.position;
            return new Rect(center - _areaSize * 0.5f, _areaSize);
        }

        /// <summary>A random world position inside the swimming area.</summary>
        private Vector2 RandomPoint()
        {
            Rect b = Bounds();
            return new Vector2(Random.Range(b.xMin, b.xMax), Random.Range(b.yMin, b.yMax));
        }
        
        private void OnDrawGizmos()
        {
            Rect b = Bounds();
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(b.center, b.size);
        }
    }
}
