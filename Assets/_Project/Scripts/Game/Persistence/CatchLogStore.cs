using UnityEngine;

namespace PocketFish.Game.Persistence
{
    /// <summary>
    /// Reads and writes the catch log as JSON in PlayerPrefs. Failures are logged, never thrown.
    /// </summary>
    public static class CatchLogStore
    {
        private const string Key = "catches.v1";

        /// <summary>
        /// Loads the saved catch log.
        /// </summary>
        /// <returns>The saved log, or an empty one if there is no save or it could not be read.</returns>
        public static CatchLogData Load()
        {
            try
            {
                if (!PlayerPrefs.HasKey(Key)) return new CatchLogData();
                CatchLogData data = JsonUtility.FromJson<CatchLogData>(PlayerPrefs.GetString(Key));
                if (data == null) return new CatchLogData();
                if (data.Entries == null) data.Entries = new System.Collections.Generic.List<CatchLogEntry>();
                return data;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Catch log could not be read: {e.Message}");
                return new CatchLogData();
            }
        }

        /// <summary>
        /// Writes the catch log and flushes PlayerPrefs to disk.
        /// </summary>
        /// <param name="data">The log to save.</param>
        public static void Save(CatchLogData data)
        {
            try
            {
                PlayerPrefs.SetString(Key, JsonUtility.ToJson(data));
                PlayerPrefs.Save();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Catch log could not be written: {e.Message}");
            }
        }
    }
}
