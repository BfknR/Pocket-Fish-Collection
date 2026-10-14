using NUnit.Framework;
using PocketFish.Domain.Fishing;

namespace PocketFish.Domain.Tests
{
    public sealed class PondTimeTests
    {
        private const long Second = System.TimeSpan.TicksPerSecond;

        [Test]
        public void NoSave_ReturnsZero()
        {
            Assert.AreEqual(0, PondTime.ElapsedSeconds(0, 100 * Second), 0.0001);
        }

        [Test]
        public void SixtySecondsLater_ReturnsSixty()
        {
            Assert.AreEqual(60, PondTime.ElapsedSeconds(10 * Second, 70 * Second), 0.0001);
        }

        [Test]
        public void ClockWentBackwards_ReturnsZero()
        {
            Assert.AreEqual(0, PondTime.ElapsedSeconds(100 * Second, 50 * Second), 0.0001);
        }

        [Test]
        public void SameInstant_ReturnsZero()
        {
            Assert.AreEqual(0, PondTime.ElapsedSeconds(10 * Second, 10 * Second), 0.0001);
        }
    }
}