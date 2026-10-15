using UnityEngine;

namespace PocketFish.Game.Aquarium
{
    /// <summary>
    /// A fish sprite swimming back and forth inside a rectangle. It turns around at the edges,
    /// drifts up and down toward random heights, and bobs gently. It also drifts toward and away from
    /// the viewer, getting smaller and tinted as it goes deeper.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class AquariumFish : MonoBehaviour
    {
        /// <summary>True if the source art points its nose to the right. The current art points left.</summary>
        private const bool ArtFacesRight = false;

        private SpriteRenderer _renderer;
        private Rect _bounds;
        private float _direction;
        private float _speed;
        private float _targetY;
        private float _driftSpeed;
        private float _bobAmount;
        private float _bobSpeed;
        private float _bobPhase;
        private float _y;
        private float _baseScale;
        private float _depth;
        private float _targetDepth;
        private float _depthSpeed;
        private float _deepestScale;
        private Color _deepestTint;

        /// <summary>
        /// Puts the fish in the tank and starts it swimming.
        /// </summary>
        /// <param name="sprite">The fish's art.</param>
        /// <param name="bounds">World-space area the fish stays inside.</param>
        /// <param name="speedRange">Min and max swimming speed in world units per second, picked per fish.</param>
        /// <param name="deepestScale">Size multiplier at the very back of the tank. The front is full size.</param>
        /// <param name="deepestTint">Colour at the very back. The front is untinted.</param>
        /// <param name="depthSpeedRange">Min and max depth change per second, picked per fish. Depth runs from 0 to 1.</param>
        public void Initialize(Sprite sprite, Rect bounds, Vector2 speedRange, float deepestScale, Color deepestTint, Vector2 depthSpeedRange)
        {
            _renderer = GetComponent<SpriteRenderer>();
            _renderer.sprite = sprite;
            _bounds = bounds;

            float width = sprite.bounds.size.x;
            float targetWidth = Random.Range(1.4f, 2.2f);
            _baseScale = width > 0f ? targetWidth / width : 1f;
            _deepestScale = deepestScale;
            _deepestTint = deepestTint;
            _depth = Random.value;
            _targetDepth = _depth;
            _depthSpeed = Random.Range(depthSpeedRange.x, depthSpeedRange.y);
            ApplyDepth();

            _direction = Random.value < 0.5f ? -1f : 1f;
            _speed = Random.Range(speedRange.x, speedRange.y);
            _driftSpeed = Random.Range(0.1f, 0.3f);
            _bobAmount = Random.Range(0.05f, 0.15f);
            _bobSpeed = Random.Range(0.8f, 1.6f);
            _bobPhase = Random.Range(0f, Mathf.PI * 2f);

            _y = Random.Range(bounds.yMin, bounds.yMax);
            _targetY = _y;
            transform.position = new Vector3(Random.Range(bounds.xMin, bounds.xMax), _y, 0f);
            ApplyFacing();
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            Vector3 position = transform.position;

            position.x += _direction * _speed * dt;
            if (position.x > _bounds.xMax) Turn(-1f, ref position.x, _bounds.xMax);
            else if (position.x < _bounds.xMin) Turn(1f, ref position.x, _bounds.xMin);

            _y = Mathf.MoveTowards(_y, _targetY, _driftSpeed * dt);
            if (Mathf.Approximately(_y, _targetY)) _targetY = Random.Range(_bounds.yMin, _bounds.yMax);

            _depth = Mathf.MoveTowards(_depth, _targetDepth, _depthSpeed * dt);
            if (Mathf.Approximately(_depth, _targetDepth)) _targetDepth = Random.value;
            ApplyDepth();

            position.y = _y + Mathf.Sin(Time.time * _bobSpeed + _bobPhase) * _bobAmount;
            transform.position = position;
        }

        /// <summary>Deeper fish are smaller and tinted, and draw behind the ones in front.</summary>
        private void ApplyDepth()
        {
            transform.localScale = Vector3.one * (_baseScale * Mathf.Lerp(1f, _deepestScale, _depth));
            _renderer.color = Color.Lerp(Color.white, _deepestTint, _depth);
            _renderer.sortingOrder = -Mathf.RoundToInt(_depth * 100f);
        }

        /// <summary>Swims back the other way from the edge.</summary>
        private void Turn(float newDirection, ref float x, float edge)
        {
            _direction = newDirection;
            x = edge;
            ApplyFacing();
        }

        /// <summary>Flips the sprite so the nose leads.</summary>
        private void ApplyFacing()
        {
            _renderer.flipX = ArtFacesRight ? _direction < 0f : _direction > 0f;
        }
    }
}
