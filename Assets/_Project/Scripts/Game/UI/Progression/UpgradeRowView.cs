using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PocketFish.Game.UI
{
    /// <summary>Presentation only. One line of the upgrade list: item name, level and a buy button.</summary>
    public sealed class UpgradeRowView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private TMP_Text _levelLabel;
        [SerializeField] private Button _buyButton;
        [SerializeField] private TMP_Text _buyLabel;

        /// <summary>Raised when the buy button is pressed.</summary>
        public event Action BuyClicked;

        private void Awake()
        {
            _buyButton.onClick.AddListener(() => BuyClicked?.Invoke());
        }

        /// <summary>
        /// Shows an item's current state.
        /// </summary>
        /// <param name="displayName">Item name.</param>
        /// <param name="level">Current level.</param>
        /// <param name="maxLevel">Top level.</param>
        /// <param name="nextCost">Cost of the next level, or null if maxed.</param>
        /// <param name="canAfford">True if the player has enough coins for the next level.</param>
        public void Show(string displayName, int level, int maxLevel, int? nextCost, bool canAfford)
        {
            _nameLabel.text = displayName;
            _levelLabel.text = $"Lv {level}/{maxLevel}";
            _buyLabel.text = nextCost.HasValue ? nextCost.Value.ToString("N0") : "MAX";
            _buyButton.interactable = nextCost.HasValue && canAfford;
        }
    }
}
