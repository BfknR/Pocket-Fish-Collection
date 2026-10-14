using PocketFish.Domain.Catch;
using UnityEngine;

namespace PocketFish.Game.Data
{
    /// <summary>
    /// Designer-editable data for one fish species: identity, rarity and presentation.
    /// Converted to the engine-free <see cref="FishSpecies"/> for use by the domain layer.
    /// </summary>
    [CreateAssetMenu(menuName = "PocketFish/Fish Definition", fileName = "Fish_New")]
    public sealed class FishDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField] private Rarity _rarity;

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
        /// <returns>A <see cref="FishSpecies"/> with this definition's identity and rarity.</returns>
        public FishSpecies ToDomain() => new FishSpecies(_id, _displayName, _rarity);

        /// <summary>
        /// Editor-only check. Warns about a missing sprite.
        /// </summary>
        private void OnValidate()
        {
            if(_sprite == null)
            {
                Debug.LogWarning($"{name}: no sprite assigned.", this);
            }
        }
    }
}
