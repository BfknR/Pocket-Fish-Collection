using System.Collections.Generic;
using PocketFish.Domain.Progression;
using UnityEngine;

namespace PocketFish.Game.Data
{
    /// <summary>
    /// Every aquarium in the order they are bought, plus the coin rewards that apply across the game.
    /// </summary>
    [CreateAssetMenu(menuName = "PocketFish/Aquarium Catalog", fileName = "AquariumCatalog")]
    public sealed class AquariumCatalog : ScriptableObject
    {
        [SerializeField] private AquariumDefinition[] _aquariums;
        [SerializeField] private EconomyTuning _economy;

        /// <summary>Every aquarium, in the order they are bought. The first is always owned.</summary>
        public IReadOnlyList<AquariumDefinition> Aquariums => _aquariums;

        /// <summary>The coin rewards.</summary>
        public EconomyTuning Economy => _economy;

        /// <summary>Builds the engine-free list used by the progression rules.</summary>
        public List<AquariumSpec> ToDomainList()
        {
            var list = new List<AquariumSpec>();
            foreach (AquariumDefinition definition in _aquariums)
            {
                if (definition != null) list.Add(definition.ToDomain());
            }
            return list;
        }
    }
}
