using PocketFish.Domain.Progression;
using TMPro;
using UnityEngine;

namespace PocketFish.Game.UI
{
    /// <summary>Presentation only. Shows the player's coins and keeps the number up to date.</summary>
    public sealed class CoinCounterView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;

        private PlayerProgress _progress;

        /// <summary>
        /// Starts showing a player's coins.
        /// </summary>
        /// <param name="progress">The progress to watch.</param>
        public void Bind(PlayerProgress progress)
        {
            if (_progress != null) _progress.Changed -= Refresh;
            _progress = progress;
            _progress.Changed += Refresh;
            Refresh();
        }

        private void OnDestroy()
        {
            if (_progress != null) _progress.Changed -= Refresh;
        }

        private void Refresh() => _label.text = _progress.Coins.ToString("N0");
    }
}
