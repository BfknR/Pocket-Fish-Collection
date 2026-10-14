using System;
using NUnit.Framework;
using PocketFish.Domain.Catch;
using PocketFish.Domain.Collection;
using PocketFish.Game.UI;

namespace PocketFish.Game.Tests
{
    public sealed class SpeciesTileModelTests
    {
        private static readonly DateTime T0 = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static CollectionProgress Create() => new CollectionProgress(new[]
        {
            new FishSpecies("a", "Alpha", Rarity.Rare),
        });

        private static SpeciesRecord Record(CollectionProgress p)
        {
            p.TryGetRecord("a", out SpeciesRecord record);
            return record;
        }

        [Test]
        public void Undiscovered_HidesNameBadgeAndCount_ButKeepsRarity()
        {
            CollectionProgress p = Create();
            SpeciesTileModel model = SpeciesTileModel.From(Record(p), null);
            Assert.IsFalse(model.IsDiscovered);
            Assert.AreEqual(string.Empty, model.DisplayName);
            Assert.AreEqual(string.Empty, model.QualityText);
            Assert.AreEqual(string.Empty, model.CountText);
            Assert.AreEqual(Rarity.Rare, model.Rarity);
            Assert.AreEqual("a", model.SpeciesId);
        }

        [Test]
        public void Discovered_ShowsNameBestQualityAndCount()
        {
            CollectionProgress p = Create();
            p.Add(new CatchRecord("a", Quality.Good, null, T0));
            p.Add(new CatchRecord("a", Quality.Great, null, T0.AddDays(1)));
            p.Add(new CatchRecord("a", Quality.Normal, null, T0.AddDays(2)));

            SpeciesTileModel model = SpeciesTileModel.From(Record(p), null);
            Assert.IsTrue(model.IsDiscovered);
            Assert.AreEqual("Alpha", model.DisplayName);
            Assert.AreEqual("Great", model.QualityText);
            Assert.AreEqual("x 3", model.CountText);
        }
    }
}
