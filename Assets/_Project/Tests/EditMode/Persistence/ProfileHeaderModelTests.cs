using System;
using NUnit.Framework;
using PocketFish.Domain.Catch;
using PocketFish.Domain.Collection;
using PocketFish.Game.UI;

namespace PocketFish.Game.Tests
{
    public sealed class ProfileHeaderModelTests
    {
        private static readonly DateTime T0 = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static CollectionProgress Create() => new CollectionProgress(new[]
        {
            new FishSpecies("a", "A", Rarity.Common),
            new FishSpecies("b", "B", Rarity.Rare),
            new FishSpecies("c", "C", Rarity.Legendary),
        });

        [Test]
        public void EmptyBook_ShowsZerosAndDash()
        {
            ProfileHeaderModel model = ProfileHeaderModel.From(Create(), "Player");
            Assert.AreEqual("Player", model.PlayerName);
            Assert.AreEqual("0 / 3", model.SpeciesText);
            Assert.AreEqual("0", model.CatchesText);
            Assert.AreEqual("-", model.BestQualityText);
        }

        [Test]
        public void PartialBook_ShowsTotalsAndBestQuality()
        {
            CollectionProgress p = Create();
            p.Add(new CatchRecord("a", Quality.Good, null, T0));
            p.Add(new CatchRecord("a", Quality.Normal, null, T0));
            p.Add(new CatchRecord("b", Quality.Great, null, T0));

            ProfileHeaderModel model = ProfileHeaderModel.From(p, "Player");
            Assert.AreEqual("2 / 3", model.SpeciesText);
            Assert.AreEqual("3", model.CatchesText);
            Assert.AreEqual("Great", model.BestQualityText);
        }

        [Test]
        public void FullBook_ShowsAllSpecies()
        {
            CollectionProgress p = Create();
            p.Add(new CatchRecord("a", Quality.Normal, null, T0));
            p.Add(new CatchRecord("b", Quality.Normal, null, T0));
            p.Add(new CatchRecord("c", Quality.Perfect, null, T0));
            Assert.AreEqual("3 / 3", ProfileHeaderModel.From(p, "Player").SpeciesText);
        }
    }
}
