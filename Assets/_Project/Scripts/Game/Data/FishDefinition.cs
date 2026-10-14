using PocketFish.Domain.Catch;
using PocketFish.Domain.Fishing;
using UnityEngine;

namespace PocketFish.Game.Data
{
    /// <summary>
    /// Designer-editable data for one fish species: identity, minigame difficulty and presentation.
    /// Converted to the engine-free <see cref="FishSpecies"/> for use by the domain layer.
    /// </summary>
    [CreateAssetMenu(menuName = "PocketFish/Fish Definition", fileName = "Fish_New")]
    public sealed class FishDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField] private Rarity _rarity;

        [Header("Minigame")]
        [Tooltip("Seconds for the arrow to cross the arc one way. Smaller is faster.")]
        [SerializeField, Min(0.2f)] private float _sweepSeconds = 2f;
        [SerializeField, Range(0.01f, 0.5f)] private float _goodHalfWidth = 0.2f;
        [SerializeField, Range(0.01f, 0.5f)] private float _greatHalfWidth = 0.1f;
        [SerializeField, Range(0.01f, 0.5f)] private float _perfectHalfWidth = 0.04f;

        [Header("Presentation")]
        [SerializeField] private Sprite _sprite;
        [SerializeField] private ShadowSize _shadowSize = ShadowSize.Medium;

        /// <summary>The fish's picture, shown on the catch reveal.</summary>
        public Sprite Sprite => _sprite;

        /// <summary>Stable identifier, used to match saved pond data back to this definition.</summary>
        public string Id => _id;

        /// <summary>Size of the shadow while the fish swims in the pond.</summary>
        public ShadowSize ShadowSize => _shadowSize;

        /// <summary>
        /// Builds the engine-free snapshot of this species.
        /// </summary>
        /// <param name="timeLimitSeconds">Minigame time limit. Passed in from tuning, so it stays the same for every fish.</param>
        /// <returns>A <see cref="FishSpecies"/> with this definition's minigame difficulty.</returns>
        public FishSpecies ToDomain(float timeLimitSeconds)
        {
            var minigame = new MinigameConfig(_sweepSeconds, timeLimitSeconds,
                _goodHalfWidth, _greatHalfWidth, _perfectHalfWidth);
            return new FishSpecies(_id, _displayName, _rarity, minigame);
        }

        /// <summary>
        /// Editor-only check. Warns about a missing sprite and keeps the target zones nested.
        /// </summary>
        private void OnValidate()
        {
            if(_sprite == null)
            {
                Debug.LogWarning($"{name}: no sprite assigned.", this);
            }
            // Target zones must nest: perfect inside great inside good.
            _greatHalfWidth = Mathf.Min(_greatHalfWidth, _goodHalfWidth);
            _perfectHalfWidth = Mathf.Min(_perfectHalfWidth, _greatHalfWidth);
        }
    }
}
