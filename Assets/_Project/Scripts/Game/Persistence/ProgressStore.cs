using System.Collections.Generic;
using PocketFish.Domain.Progression;
using UnityEngine;

namespace PocketFish.Game.Persistence
{
    /// <summary>
    /// Reads and writes the coins and aquarium progression as JSON in PlayerPrefs.
    /// Saves use ids, not positions, so reordering the catalog can't scramble a save.
    /// Failures are logged, never thrown.
    /// </summary>
    public static class ProgressStore
    {
        private const string Key = "progress.v1";

        /// <summary>
        /// Fills progress from the save.
        /// </summary>
        /// <param name="progress">The progress to fill. Left as it is if there is no save or it can't be read.</param>
        public static void Load(PlayerProgress progress)
        {
            try
            {
                if (!PlayerPrefs.HasKey(Key)) return;
                ProgressSaveData data = JsonUtility.FromJson<ProgressSaveData>(PlayerPrefs.GetString(Key));
                if (data == null) return;

                IReadOnlyList<AquariumSpec> aquariums = progress.Aquariums;

                // Aquariums are bought in order, so ownership is "everything up to the furthest saved id".
                int owned = 1;
                int selected = 0;
                for (int a = 0; a < aquariums.Count; a++)
                {
                    if (data.OwnedAquariumIds != null && data.OwnedAquariumIds.Contains(aquariums[a].Id)) owned = a + 1;
                    if (aquariums[a].Id == data.SelectedAquariumId) selected = a;
                }

                var levels = new List<IReadOnlyList<int>>();
                foreach (AquariumSpec aquarium in aquariums)
                {
                    var row = new int[aquarium.Upgrades.Count];
                    for (int u = 0; u < row.Length; u++)
                    {
                        row[u] = SavedLevel(data, aquarium.Id, aquarium.Upgrades[u].Id);
                    }
                    levels.Add(row);
                }

                progress.Restore(data.Coins, owned, selected, levels);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Progress could not be read: {e.Message}");
            }
        }

        /// <summary>
        /// Writes the progress and flushes PlayerPrefs to disk.
        /// </summary>
        /// <param name="progress">The progress to save.</param>
        public static void Save(PlayerProgress progress)
        {
            try
            {
                var data = new ProgressSaveData { Coins = progress.Coins };
                for (int a = 0; a < progress.Aquariums.Count; a++)
                {
                    AquariumSpec aquarium = progress.Aquariums[a];
                    if (progress.IsOwned(a)) data.OwnedAquariumIds.Add(aquarium.Id);
                    if (a == progress.SelectedIndex) data.SelectedAquariumId = aquarium.Id;

                    for (int u = 0; u < aquarium.Upgrades.Count; u++)
                    {
                        int level = progress.LevelOf(a, u);
                        if (level <= 0) continue;
                        data.Levels.Add(new UpgradeLevelEntry { AquariumId = aquarium.Id, UpgradeId = aquarium.Upgrades[u].Id, Level = level });
                    }
                }
                PlayerPrefs.SetString(Key, JsonUtility.ToJson(data));
                PlayerPrefs.Save();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Progress could not be written: {e.Message}");
            }
        }

        private static int SavedLevel(ProgressSaveData data, string aquariumId, string upgradeId)
        {
            if (data.Levels == null) return 0;
            foreach (UpgradeLevelEntry entry in data.Levels)
            {
                if (entry.AquariumId == aquariumId && entry.UpgradeId == upgradeId) return entry.Level;
            }
            return 0;
        }
    }
}
