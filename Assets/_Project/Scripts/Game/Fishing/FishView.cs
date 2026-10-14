using PocketFish.Domain.Fishing;
using UnityEngine;

namespace PocketFish.Game.Fishing
{
    /// <summary>
    /// Presentation only: a shadow that drifts around the pond. Holds no game rules.
    /// </summary>
    public sealed class FishView : MonoBehaviour
    {
        [SerializeField] private Transform _shadow;
        [SerializeField] private float _speed = 0.6f;
        [SerializeField, Min(0.1f)] private float _minTapRadius = 0.5f;
        [SerializeField, Min(0f)] private float _tapRadiusPerScale = 0.6f;

        private CircleCollider2D _tapArea;
        private Rect _bounds;
        private Vector2 _target;

        /// <summary>The pond fish this view represents.</summary>
        public PondFish Fish { get; private set; }

        /// <summary>Caches the tap collider.</summary>
        private void Awake()
        {
            _tapArea = GetComponent<CircleCollider2D>();
        }

        /// <summary>
        /// Sets up the view for a pond fish and starts it swimming.
        /// </summary>
        /// <param name="fish">The pond fish to represent.</param>
        /// <param name="shadowScale">Scale of the shadow. Also sizes the tap area.</param>
        /// <param name="bounds">World-space area the fish swims within.</param>
        /// <param name="start">World position to start from.</param>
        public void Initialize(PondFish fish, float shadowScale, Rect bounds, Vector2 start)
        {
            Fish = fish;
            _bounds = bounds;
            _shadow.localScale = Vector3.one * shadowScale;
            //Bigger shadow, bigget tap area. The floor keeps small fish tappable.
            _tapArea.radius = Mathf.Max(_minTapRadius, _tapRadiusPerScale * shadowScale);
            transform.position = start;
            PickTarget();
        }

        /// <summary>Moves toward the current target and picks a new one on arrival.</summary>
        private void Update()
        {
            transform.position = Vector2.MoveTowards(transform.position, _target, _speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, _target) < 0.01f) PickTarget();
        }

        /// <summary>Chooses a random point inside the swimming bounds as the next destination.</summary>
        private void PickTarget()
        {
            _target = new Vector2(
                Random.Range(_bounds.xMin, _bounds.xMax),
                Random.Range(_bounds.yMin, _bounds.yMax));
        }
    }
}
