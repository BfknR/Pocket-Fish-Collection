using TMPro;
using UnityEngine;

namespace PocketFish.Game.UI
{
    /// <summary>
    /// Presentation only. The profile block at the top of the Collection Book: name and stats.
    /// </summary>
    public sealed class ProfileHeaderView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _speciesText;
        [SerializeField] private TMP_Text _catchesText;
        [SerializeField] private TMP_Text _bestQualityText;

        /// <summary>
        /// Shows the header.
        /// </summary>
        /// <param name="model">What to show.</param>
        public void Bind(ProfileHeaderModel model)
        {
            _nameText.text = model.PlayerName;
            _speciesText.text = model.SpeciesText;
            _catchesText.text = model.CatchesText;
            _bestQualityText.text = model.BestQualityText;
        }
    }
}
