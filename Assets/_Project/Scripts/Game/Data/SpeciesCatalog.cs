using System.Collections.Generic;
using PocketFish.Domain.Catch;
using UnityEngine;

namespace PocketFish.Game.Data
{
    /// <summary>
    /// Every species in the game, in the order the Collection Book shows them.
    /// Separate from each pond's spawn table: a pond only spawns some of these, the book lists all.
    /// </summary>
    [CreateAssetMenu(menuName = "PocketFish/Species Catalog", fileName = "SpeciesCatalog")]
    public sealed class SpeciesCatalog : ScriptableObject
    {
        [SerializeField] private FishDefinition[] _species;

        [Header("Fallback Art")]
        [SerializeField] private Sprite _placeholderSprite;

        /// <summary>Every species, in display order.</summary>
        public IReadOnlyList<FishDefinition> Species => _species;

        /// <summary>Sprite shown when a species has no art of its own.</summary>
        public Sprite PlaceholderSprite => _placeholderSprite;

        /// <summary>
        /// Checks whether a species is in the catalog.
        /// </summary>
        /// <param name="id">The <see cref="FishDefinition.Id"/> to look for.</param>
        /// <returns>True if a definition with that id is listed.</returns>
        public bool Contains(string id) => Find(id) != null;

        /// <summary>
        /// Looks up a species by id.
        /// </summary>
        /// <param name="id">The <see cref="FishDefinition.Id"/> to find.</param>
        /// <returns>The matching definition, or null if the catalog doesn't list it.</returns>
        public FishDefinition Find(string id)
        {
            foreach (FishDefinition definition in _species)
            {
                if (definition != null && definition.Id == id) return definition;
            }
            return null;
        }

        /// <summary>
        /// Gets the art to show for a species.
        /// </summary>
        /// <param name="definition">The species.</param>
        /// <returns>Its own sprite, or the placeholder if it has none.</returns>
        public Sprite SpriteFor(FishDefinition definition)
        {
            return definition != null && definition.Sprite != null ? definition.Sprite : _placeholderSprite;
        }

        /// <summary>
        /// Builds the engine-free species list for the domain layer, in display order.
        /// Empty slots and repeated ids are skipped.
        /// </summary>
        /// <returns>One <see cref="FishSpecies"/> per listed definition.</returns>
        public List<FishSpecies> ToDomainList()
        {
            var list = new List<FishSpecies>();
            var seen = new HashSet<string>();
            foreach (FishDefinition definition in _species)
            {
                if (definition == null || !seen.Add(definition.Id)) continue;
                list.Add(definition.ToDomain());
            }
            return list;
        }

        /// <summary>
        /// Editor-only check. Warns about empty slots, repeated ids and a missing placeholder.
        /// </summary>
        private void OnValidate()
        {
            var seen = new HashSet<string>();
            foreach (FishDefinition definition in _species)
            {
                if (definition == null)
                {
                    Debug.LogWarning($"{name}: has an empty species slot.", this);
                    continue;
                }
                if (!seen.Add(definition.Id))
                {
                    Debug.LogWarning($"{name}: species id '{definition.Id}' is listed more than once.", this);
                }
            }

            if (_placeholderSprite == null)
            {
                Debug.LogWarning($"{name}: no placeholder sprite assigned.", this);
            }
        }
    }
}
