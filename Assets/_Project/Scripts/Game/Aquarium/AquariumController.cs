using PocketFish.Game.Collection;
using PocketFish.Game.Data;
using PocketFish.Game.Persistence;
using PocketFish.Game.UI;
using UnityEngine;

namespace PocketFish.Game.Aquarium
{
    /// <summary>
    /// The lobby scene the game opens on. Loads the player's collection from the saved catch log
    /// and hands it to the Collection Book. The Go Fish and Collection buttons are wired in the scene.
    /// </summary>
    public sealed class AquariumController : MonoBehaviour
    {
        [SerializeField] private SpeciesCatalog _catalog;
        [SerializeField] private CollectionBookView _book;

        private void Awake()
        {
            var collection = new CollectionSession(_catalog.ToDomainList(), CatchLogStore.Load());
            _book.Bind(collection, _catalog);
        }
    }
}
