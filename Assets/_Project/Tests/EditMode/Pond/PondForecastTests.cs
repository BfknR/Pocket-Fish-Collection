using NUnit.Framework;
using PocketFish.Domain.Fishing;

namespace PocketFish.Domain.Tests
{
    public sealed class PondForecastTests
    {
        [Test]
        public void NoTimePassed_KeepsTheSavedCount()
        {
            Assert.AreEqual(3, PondForecast.FishCount(3, 0, 0, 6, 10));
        }

        [Test]
        public void EachFullInterval_AddsOneFish()
        {
            Assert.AreEqual(5, PondForecast.FishCount(3, 0, 25, 6, 10));
        }

        [Test]
        public void SavedTimerCountsTowardTheNextFish()
        {
            Assert.AreEqual(4, PondForecast.FishCount(3, 8, 3, 6, 10));
        }

        [Test]
        public void NeverExceedsTheCapacity()
        {
            Assert.AreEqual(6, PondForecast.FishCount(3, 0, 10000, 6, 10));
        }

        [Test]
        public void MoreSavedFishThanCapacity_IsCapped()
        {
            Assert.AreEqual(4, PondForecast.FishCount(9, 0, 0, 4, 10));
        }

        [Test]
        public void EmptyPond_FillsOverTime()
        {
            Assert.AreEqual(0, PondForecast.FishCount(0, 0, 9, 6, 10));
            Assert.AreEqual(1, PondForecast.FishCount(0, 0, 10, 6, 10));
        }

        [Test]
        public void NegativeElapsedTime_AddsNothing()
        {
            Assert.AreEqual(3, PondForecast.FishCount(3, 0, -50, 6, 10));
        }

        [Test]
        public void NextFish_IsTheIntervalMinusTheTimer()
        {
            var result = PondForecast.Project(3, 4, 0, 6, 10);

            Assert.AreEqual(6.0, result.SecondsToNextFish.Value, 0.0001);
        }

        [Test]
        public void NextFish_CountsDownAsTimePasses()
        {
            var result = PondForecast.Project(3, 4, 5, 6, 10);

            Assert.AreEqual(3, result.FishCount, "the timer is at 9 of 10, so nothing has spawned yet");
            Assert.AreEqual(1.0, result.SecondsToNextFish.Value, 0.0001);
        }

        [Test]
        public void NextFish_RestartsAfterASpawn()
        {
            var result = PondForecast.Project(3, 0, 12, 6, 10);

            Assert.AreEqual(4, result.FishCount);
            Assert.AreEqual(8.0, result.SecondsToNextFish.Value, 0.0001);
        }

        [Test]
        public void FullPond_HasNoNextFish()
        {
            Assert.IsNull(PondForecast.Project(6, 3, 0, 6, 10).SecondsToNextFish);
            Assert.IsNull(PondForecast.Project(0, 0, 10000, 6, 10).SecondsToNextFish);
        }
    }
}
