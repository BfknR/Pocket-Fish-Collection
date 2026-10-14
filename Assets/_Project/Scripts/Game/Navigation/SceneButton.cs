using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PocketFish.Game.Navigation
{
    /// <summary>
    /// Put on a button to make it load a scene when pressed.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public sealed class SceneButton : MonoBehaviour
    {
        [SerializeField] private string _sceneName;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Load);
        }

        private void OnDestroy()
        {
            if (_button != null) _button.onClick.RemoveListener(Load);
        }

        /// <summary>Loads the scene this button points to.</summary>
        private void Load() => SceneManager.LoadScene(_sceneName);
    }
}
