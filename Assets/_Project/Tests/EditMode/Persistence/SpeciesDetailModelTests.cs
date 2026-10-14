using System;
using NUnit.Framework;
using PocketFish.Domain.Catch;
using PocketFish.Domain.Collection;
using PocketFish.Game.UI;

namespace PocketFish.Game.Tests
{
    public sealed class SpeciesDetailModelTests
    {
        private static readonly DateTime T0 = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

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
        public void Undiscovered_HidesEverythingExceptRarity()
        {
            SpeciesDetailModel m = SpeciesDetailModel.From(Record(Create()), null, "secret lore");
            Assert.IsFalse(m.IsDiscovered);
            Assert.AreEqual("Rare", m.RarityText);
            Assert.AreEqual(string.Empty, m.DisplayName);
            Assert.AreEqual(string.Empty, m.Description);
            Assert.AreEqual(string.Empty, m.CaughtText);
            Assert.AreEqual(string.Empty, m.QualityCountsText);
            Assert.AreEqual(string.Empty, m.BestQualityText);
            Assert.AreEqual(string.Empty, m.FirstCaughtText);
            Assert.AreEqual(string.Empty, m.CompletionText);
        }

        [Test]
        public void Discovered_ShowsCountsRecordsAndCompletion()
        {
            CollectionProgress p = Create();
            p.Add(new CatchRecord("a", Quality.Good, null, T0.AddDays(2)));
            p.Add(new CatchRecord("a", Quality.Good, null, T0.AddDays(3)));
            p.Add(new CatchRecord("a", Quality.Perfect, null, T0));

            SpeciesDetailModel m = SpeciesDetailModel.From(Record(p), null, "lore");
            Assert.IsTrue(m.IsDiscovered);
            Assert.AreEqual("Alpha", m.DisplayName);
            Assert.AreEqual("lore", m.Description);
            Assert.AreEqual("Caught: 3", m.CaughtText);
            Assert.AreEqual("Normal: 0\nGood: 2\nGreat: 0\nPerfect: 1", m.QualityCountsText);
            Assert.AreEqual("Best quality: Perfect", m.BestQualityText);
            Assert.AreEqual("First caught: " + T0.ToLocalTime().ToShortDateString(), m.FirstCaughtText);
            Assert.AreEqual("Qualities: 2 / 4", m.CompletionText);
        }
    }
}
