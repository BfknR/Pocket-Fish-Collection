using System.Collections.Generic;
using NUnit.Framework;
using PocketFish.Domain.Catch;
using PocketFish.Domain.Fishing;

namespace PocketFish.Domain.Tests
{
    public sealed class PondPopulationTests
    {
        private const double Interval = 60;
        private int _rolls;

        private PondPopulation Create(int capacity)
        {
            _rolls = 0;
            return new PondPopulation(capacity, Interval, () =>
            {
                _rolls++;
                return default;
            });
        }

        [Test]
        public void AdvanceBelowInterval_SpawnsNothing()
        {
            PondPopulation pond = Create(5);
            pond.Advance(Interval - 1);
            Assert.AreEqual(0, pond.Count);
        }

        [Test]
        public void AdvanceExactlyInterval_SpawnsOne()
        {
            PondPopulation pond = Create(5);
            pond.Advance(Interval);
            Assert.AreEqual(1, pond.Count);
        }

        [Test]
        public void AdvanceTwoAndAHalfIntervals_SpawnsTwo_KeepsHalfInterval()
        {
            PondPopulation pond = Create(5);
            pond.Advance(Interval * 2.5);
            Assert.AreEqual(2, pond.Count);
            Assert.AreEqual(0.5 * Interval, pond.TimerSeconds, 0.0001);
        }

        [Test]
        public void CatchingFreesSlot_NextIntervalRefills()
        {
            PondPopulation pond = Create(2);
            pond.Advance(Interval * 2);
            int id = pond.Fish[0].InstanceId;

            Assert.IsTrue(pond.Remove(id));
            Assert.AreEqual(1, pond.Count);

            pond.Advance(Interval);
            Assert.AreEqual(2, pond.Count);
        }

        [Test]
        public void RemoveUnknownId_ReturnsFalse()
        {
            PondPopulation pond = Create(1);
            Assert.IsFalse(pond.Remove(999));
        }

        [Test]
        public void Restore_KeepsSavedSpeciesInOrder_AndTruncatesToCapacity()
        {
            PondPopulation pond = Create(2);
            var saved = new List<FishSpecies> { default, default, default };
            pond.Restore(saved, 0);
            Assert.AreEqual(2, pond.Count);
        }

        [Test]
        public void Restore_NegativeTimer_ClampsToZero()
        {
            PondPopulation pond = Create(2);
            pond.Restore(new List<FishSpecies>(), -5);
            Assert.AreEqual(0, pond.TimerSeconds, 0.0001);
        }

        [Test]
        public void InstanceIds_AreUnique_AcrossRestoreAndSpawn()
        {
            PondPopulation pond = Create(4);
            pond.Restore(new List<FishSpecies> { default, default }, 0);
            pond.Advance(Interval * 2);

            var ids = new HashSet<int>();
            foreach(PondFish fish in pond.Fish)
            {
                Assert.IsTrue(ids.Add(fish.InstanceId));
            }
        }

        [Test]
        public void Spawns_UseRollFunction()
        {
            PondPopulation pond = Create(5);
            pond.Advance(Interval * 3);
            Assert.AreEqual(3, _rolls);
        }
    }
}