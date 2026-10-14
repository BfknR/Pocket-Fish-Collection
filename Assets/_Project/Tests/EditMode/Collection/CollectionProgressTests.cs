using System;
using NUnit.Framework;
using PocketFish.Domain.Catch;
using PocketFish.Domain.Collection;

namespace PocketFish.Domain.Tests
{
    public sealed class CollectionProgressTests
    {
        private static readonly DateTime T0 = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static CollectionProgress Create() => new CollectionProgress(new[]
        {
            new FishSpecies("a", "A", Rarity.Common),
            new FishSpecies("b", "B", Rarity.Rare),
            new FishSpecies("c", "C", Rarity.Legendary),
        });

        private static CatchRecord Catch(string id, Quality quality = Quality.Normal, int day = 0) =>
            new CatchRecord(id, quality, null, T0.AddDays(day));

        private static SpeciesRecord Get(CollectionProgress p, string id)
        {
            Assert.IsTrue(p.TryGetRecord(id, out SpeciesRecord record));
            return record;
        }

        [Test]
        public void CountPerSpecies_MatchesRecords()
        {
            CollectionProgress p = Create();
            p.Add(Catch("a")); p.Add(Catch("a")); p.Add(Catch("b"));
            Assert.AreEqual(2, Get(p, "a").Count);
            Assert.AreEqual(1, Get(p, "b").Count);
        }

        [Test]
        public void CountPerQuality_SumsToSpeciesCount()
        {
            CollectionProgress p = Create();
            p.Add(Catch("a", Quality.Normal)); p.Add(Catch("a", Quality.Good));
            p.Add(Catch("a", Quality.Good)); p.Add(Catch("a", Quality.Perfect));
            SpeciesRecord a = Get(p, "a");
            Assert.AreEqual(1, a.CountOf(Quality.Normal));
            Assert.AreEqual(2, a.CountOf(Quality.Good));
            Assert.AreEqual(0, a.CountOf(Quality.Great));
            Assert.AreEqual(1, a.CountOf(Quality.Perfect));
            int sum = 0;
            foreach(Quality q in Enum.GetValues(typeof(Quality))) sum += a.CountOf(q);
            Assert.AreEqual(a.Count, sum);
        }

        [Test]
        public void BestCatch_PicksHighestQualityFirst()
        {
            CollectionProgress p = Create();
            p.Add(Catch("a", Quality.Good, 1)); p.Add(Catch("a", Quality.Great, 5));
            Assert.AreEqual(Quality.Great, Get(p, "a").BestCatch.Quality);
        }

        [Test]
        public void BestCatch_OnEqualQuality_KeepsEarliest()
        {
            CollectionProgress p = Create();
            p.Add(Catch("a", Quality.Good, 3)); p.Add(Catch("a", Quality.Good, 1)); p.Add(Catch("a", Quality.Good, 2));
            Assert.AreEqual(T0.AddDays(1), Get(p, "a").BestCatch.CaughtUtc);
        }

        [Test]
        public void ZeroRecords_IsUndiscovered()
        {
            CollectionProgress p = Create();
            Assert.IsFalse(Get(p, "c").IsDiscovered);
            Assert.AreEqual(0, Get(p, "c").Count);
        }

        [Test]
        public void ProfileTotals_MatchSumAcrossSpecies()
        {
            CollectionProgress p = Create();
            p.Add(Catch("a")); p.Add(Catch("a", Quality.Great)); p.Add(Catch("b", Quality.Good));
            int sum = 0;
            foreach(SpeciesRecord r in p.Species) sum += r.Count;
            Assert.AreEqual(sum, p.TotalCatches);
            Assert.AreEqual(2, p.SpeciesDiscovered);
            Assert.AreEqual(Quality.Great, p.BestQuality);
        }

        [Test]
        public void BestQuality_IsNullWhenNothingCaught()
        {
            Assert.IsNull(Create().BestQuality);
        }

        [Test]
        public void Completion_AtZeroPartialAndFull()
        {
            CollectionProgress p = Create();
            Assert.AreEqual(0f, p.Completion);
            p.Add(Catch("a"));
            Assert.AreEqual(1f / 3f, p.Completion, 0.0001f);
            p.Add(Catch("b")); p.Add(Catch("c"));
            Assert.AreEqual(1f, p.Completion);
        }

        [Test]
        public void Completion_WithNoSpecies_IsZero()
        {
            Assert.AreEqual(0f, new CollectionProgress(new FishSpecies[0]).Completion);
        }

        [Test]
        public void Records_UpdateWhenNewCatchAdded()
        {
            CollectionProgress p = Create();
            p.Add(Catch("a", Quality.Normal, day: 5));
            SpeciesRecord a = Get(p, "a");
            Assert.AreEqual(T0.AddDays(5), a.FirstCaughtUtc);

            p.Add(Catch("a", Quality.Perfect, day: 2));
            Assert.AreEqual(Quality.Perfect, a.BestQuality);
            Assert.AreEqual(T0.AddDays(2), a.FirstCaughtUtc);
        }

        [Test]
        public void UnknownSpeciesId_IsRejected_NotCounted()
        {
            CollectionProgress p = Create();
            Assert.IsFalse(p.Add(Catch("ghost")));
            Assert.IsFalse(p.Add(new CatchRecord(null, Quality.Normal, null, T0)));
            Assert.AreEqual(0, p.TotalCatches);
            Assert.AreEqual(0, p.SpeciesDiscovered);
            Assert.IsNull(p.BestQuality);
        }
    }
}
