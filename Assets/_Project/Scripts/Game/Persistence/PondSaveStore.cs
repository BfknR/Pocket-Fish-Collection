using UnityEngine;

namespace PocketFish.Game.Persistence
{
    /// <summary>
    /// Reads and writes each pond's save as JSON in PlayerPrefs, one per aquarium. Failures are logged, never thrown.
    /// </summary>
    public static class PondSaveStore
    {
        /// <summary>The key the single pond used before there were several. The first aquarium still reads it.</summary>
        private const string LegacyKey = "pond.v1";

        private static string KeyFor(string pondId) => $"pond.v1.{pondId}";

        /// <summary>
        /// Loads a pond's save.
        /// </summary>
        /// <param name="pondId">Id of the aquarium the pond belongs to.</param>
        /// <param name="readLegacySave">True for the first aquarium, so a save made before there were several ponds isn't lost.</param>
        /// <returns>The saved data, or null if there is no save or it could not be read.</returns>
        public static PondSaveData Load(string pondId, bool readLegacySave)
        {
            try
            {
                string key = KeyFor(pondId);
                if (!PlayerPrefs.HasKey(key))
                {
                    if (!readLegacySave || !PlayerPrefs.HasKey(LegacyKey)) return null;
                    key = LegacyKey;
                }
                return JsonUtility.FromJson<PondSaveData>(PlayerPrefs.GetString(key));
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Pond save could not be read: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Writes a pond's save and flushes PlayerPrefs to disk.
        /// </summary>
        /// <param name="data">The state to save.</param>
        /// <param name="pondId">Id of the aquarium the pond belongs to.</param>
        public static void Save(PondSaveData data, string pondId)
        {
            try
            {
                PlayerPrefs.SetString(KeyFor(pondId), JsonUtility.ToJson(data));
                PlayerPrefs.Save();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Pond save could not be written: {e.Message}");
            }
        }
    }
}
