using PocketFish.Domain.Progression;
using PocketFish.Game.Data;
using PocketFish.Game.Persistence;
using UnityEngine;

namespace PocketFish.Game.Progression
{
    /// <summary>
    /// Holds the player's progress for the whole run, so the Aquarium and the Pond share the same coins
    /// and the same selected aquarium. Loaded from the save the first time a scene asks for it.
    /// </summary>
    public static class ProgressSession
    {
        private static PlayerProgress _progress;

        /// <summary>
        /// Gets the player's progress, loading it on first use.
        /// </summary>
        /// <param name="catalog">The aquarium catalog that describes the progression.</param>
        public static PlayerProgress Get(AquariumCatalog catalog)
        {
            if (_progress == null)
            {
                _progress = new PlayerProgress(catalog.ToDomainList());
                ProgressStore.Load(_progress);
            }
            return _progress;
        }

        /// <summary>Writes the progress to disk. Call after anything that changes it.</summary>
        public static void Save()
        {
            if (_progress != null) ProgressStore.Save(_progress);
        }

        /// <summary>Forgets the loaded progress when the game starts, in case domain reload is off in the editor.</summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset() => _progress = null;
    }
}
