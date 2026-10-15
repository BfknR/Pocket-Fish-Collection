using System.Collections.Generic;
using PocketFish.Domain.Progression;
using PocketFish.Game.Data;
using PocketFish.Game.Progression;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PocketFish.Game.UI
{
    /// <summary>
    /// Presentation only. The upgrade screen for the selected aquarium: one row per item, plus a button to buy
    /// the next aquarium once everything is maxed. Buying goes through <see cref="PlayerProgress"/>.
    /// </summary>
    public sealed class UpgradePanelView : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Transform _rowsParent;
        [SerializeField] private UpgradeRowView _rowPrefab;
        [SerializeField] private Button _buyNextButton;
        [SerializeField] private TMP_Text _buyNextLabel;
        [SerializeField] private Button _closeButton;

        private readonly List<UpgradeRowView> _rows = new List<UpgradeRowView>();
        private PlayerProgress _progress;
        private AquariumCatalog _catalog;
        private int _builtForAquarium = -1;

        /// <summary>True while the panel is showing.</summary>
        public bool IsOpen => _root.activeSelf;

        private void Awake()
        {
            _root.SetActive(false);
            _closeButton.onClick.AddListener(Close);
            _buyNextButton.onClick.AddListener(BuyNextAquarium);
        }

        /// <summary>
        /// Gives the panel the progress and catalog it works on.
        /// </summary>
        /// <param name="progress">The player's progress.</param>
        /// <param name="catalog">Names, costs and items of the aquariums.</param>
        public void Bind(PlayerProgress progress, AquariumCatalog catalog)
        {
            _progress = progress;
            _catalog = catalog;
            _progress.Changed += OnProgressChanged;
        }

        private void OnDestroy()
        {
            if (_progress != null) _progress.Changed -= OnProgressChanged;
        }

        /// <summary>Opens the panel for the selected aquarium.</summary>
        public void Open()
        {
            Rebuild();
            _root.SetActive(true);
        }

        /// <summary>Closes the panel.</summary>
        public void Close() => _root.SetActive(false);

        private void OnProgressChanged()
        {
            if (!IsOpen) return;
            if (_progress.SelectedIndex != _builtForAquarium) Rebuild();
            else Refresh();
        }

        /// <summary>Creates one row per item of the selected aquarium.</summary>
        private void Rebuild()
        {
            foreach (UpgradeRowView row in _rows) Destroy(row.gameObject);
            _rows.Clear();

            _builtForAquarium = _progress.SelectedIndex;
            AquariumDefinition aquarium = _catalog.Aquariums[_builtForAquarium];
            for (int i = 0; i < aquarium.Upgrades.Count; i++)
            {
                int upgradeIndex = i;
                UpgradeRowView row = Instantiate(_rowPrefab, _rowsParent);
                row.BuyClicked += () => Buy(upgradeIndex);
                _rows.Add(row);
            }
            Refresh();
        }

        /// <summary>Updates every row and the buy-next button from the current progress.</summary>
        private void Refresh()
        {
            int aquariumIndex = _builtForAquarium;
            AquariumDefinition aquarium = _catalog.Aquariums[aquariumIndex];
            _title.text = aquarium.DisplayName;

            for (int i = 0; i < _rows.Count; i++)
            {
                AquariumDefinition.UpgradeItem item = aquarium.Upgrades[i];
                int? cost = _progress.NextUpgradeCost(aquariumIndex, i);
                _rows[i].Show(item.DisplayName, _progress.LevelOf(aquariumIndex, i), item.LevelCosts.Length,
                    cost, cost.HasValue && _progress.Coins >= cost.Value);
            }

            RefreshBuyNext(aquariumIndex);
        }

        /// <summary>
        /// The next aquarium is only offered from the latest one you own, and only unlocks once it is fully upgraded.
        /// </summary>
        private void RefreshBuyNext(int aquariumIndex)
        {
            int? cost = _progress.NextAquariumCost;
            bool isLatest = aquariumIndex == _progress.OwnedCount - 1;
            _buyNextButton.gameObject.SetActive(isLatest && cost.HasValue);
            if (!isLatest || !cost.HasValue) return;

            string nextName = _catalog.Aquariums[_progress.OwnedCount].DisplayName;
            if (_progress.CanBuyNextAquarium)
            {
                _buyNextLabel.text = $"Buy {nextName}\n{cost.Value:N0}";
                _buyNextButton.interactable = _progress.Coins >= cost.Value;
            }
            else
            {
                _buyNextLabel.text = $"{nextName}\nMax every upgrade to unlock";
                _buyNextButton.interactable = false;
            }
        }

        private void Buy(int upgradeIndex)
        {
            if (_progress.TryUpgrade(_builtForAquarium, upgradeIndex) == PurchaseResult.Success) ProgressSession.Save();
        }

        private void BuyNextAquarium()
        {
            if (_progress.TryBuyNextAquarium() == PurchaseResult.Success) ProgressSession.Save();
        }
    }
}
