using System;
using System.Collections.Generic;

namespace PocketFish.Game.Persistence
{
    /// <summary>The saved coins and aquarium progression, serialized to JSON by <see cref="ProgressStore"/>.</summary>
    [Serializable]
    public sealed class ProgressSaveData
    {
        /// <summary>Coins the player has.</summary>
        public int Coins;

        /// <summary>Ids of the owned aquariums.</summary>
        public List<string> OwnedAquariumIds = new List<string>();

        /// <summary>Id of the selected aquarium.</summary>
        public string SelectedAquariumId;

        /// <summary>The level of every upgraded item.</summary>
        public List<UpgradeLevelEntry> Levels = new List<UpgradeLevelEntry>();
    }

    /// <summary>The level of one item of one aquarium.</summary>
    [Serializable]
    public sealed class UpgradeLevelEntry
    {
        public string AquariumId;
        public string UpgradeId;
        public int Level;
    }
}
