using UnityEngine;

namespace PocketFish.Game.Persistence
{
    /// <summary>
    /// Reads and writes the pond save as JSON in PlayerPrefs. Failures are logged, never thrown.
    /// </summary>
    public static class PondSaveStore
    {
        private const string Key = "pond.v1";

        /// <summary>
        /// Loads the saved pond.
        /// </summary>
        /// <returns>The saved data, or null if there is no save or it could not be read.</returns>
        public static PondSaveData Load()
        {
            try
            {
                if (!PlayerPrefs.HasKey(Key)) return null;
                return JsonUtility.FromJson<PondSaveData>(PlayerPrefs.GetString(Key));
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Pond save could not be read: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Writes the pond save and flushes PlayerPrefs to disk.
        /// </summary>
        /// <param name="data">The state to save.</param>
        public static void Save(PondSaveData data)
        {
            try
            {
                PlayerPrefs.SetString(Key, JsonUtility.ToJson(data));
                PlayerPrefs.Save();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Pond save could not be written: {e.Message}");
            }
        }
    }
}
