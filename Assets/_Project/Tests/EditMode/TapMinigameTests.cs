using NUnit.Framework;
using PocketFish.Domain.Fishing;

namespace PocketFish.Domain.Tests
{
    public sealed class TapMinigameTests
    {
        // Binary-exact values so boundary checks don't depend on float rounding.
        // With a 1 second pass, the marker position equals elapsed time on the way out.
        private static readonly MinigameConfig Config = new MinigameConfig(
            sweepSeconds: 1f,
            timeLimitSeconds: 2f,
            goodHalfWidth: 0.25f,
            greatHalfWidth: 0.125f,
            perfectHalfWidth: 0.0625f);

        private TapMinigame _minigame;

        [SetUp]
        public void SetUp()
        {
            _minigame = new TapMinigame();
            _minigame.Begin(Config);
        }

        [TestCase(0.5f, TapGrade.Perfect)]     // exact centre
        [TestCase(0.5625f, TapGrade.Perfect)]  // perfect edge, inclusive
        [TestCase(0.625f, TapGrade.Great)]     // great edge, inclusive
        [TestCase(0.6875f, TapGrade.Good)]     // inside good, outside great
        [TestCase(0.75f, TapGrade.Good)]       // good edge, inclusive
        [TestCase(0.8125f, TapGrade.Miss)]     // outside good
        [TestCase(0.125f, TapGrade.Miss)]      // tapped before the marker reaches the zone
        [TestCase(1.4375f, TapGrade.Perfect)]  // return pass, perfect edge
        [TestCase(1.5f, TapGrade.Perfect)]     // return pass, centre
        public void TapAtTime_GradesByDistanceFromCentre(float time, TapGrade expected)
        {
            _minigame.Tick(time);
            _minigame.Tap();

            Assert.AreEqual(expected, _minigame.Result);
        }

        [Test]
        public void NoTapBeforeTimeLimit_IsMiss()
        {
            _minigame.Tick(2f);

            Assert.IsTrue(_minigame.IsFinished);
            Assert.AreEqual(TapGrade.Miss, _minigame.Result);
        }

        [Test]
        public void FirstTapEndsMinigame_LaterTapsDoNotChangeResult()
        {
            _minigame.Tick(0.5f);
            _minigame.Tap();

            _minigame.Tick(0.5f);  // marker now far from centre
            _minigame.Tap();

            Assert.AreEqual(TapGrade.Perfect, _minigame.Result);
        }

        [Test]
        public void Progress_RisesFromZeroToOne()
        {
            Assert.AreEqual(0f, _minigame.Progress);

            _minigame.Tick(1f);
            Assert.AreEqual(0.5f, _minigame.Progress);

            _minigame.Tick(1f);
            Assert.AreEqual(1f, _minigame.Progress);
        }

        [Test]
        public void Begin_ResetsAFinishedMinigame()
        {
            _minigame.Tick(2f);

            _minigame.Begin(Config);

            Assert.IsFalse(_minigame.IsFinished);
            Assert.AreEqual(0f, _minigame.Progress);
        }

        [Test]
        public void TimeLimit_IsFixedRegardlessOfSweepSpeed()
        {
            var fast = new MinigameConfig(
                sweepSeconds: 0.5f,
                timeLimitSeconds: 2f,
                goodHalfWidth: 0.25f,
                greatHalfWidth: 0.125f,
                perfectHalfWidth: 0.0625f);
            _minigame.Begin(fast);

            _minigame.Tick(1.5f);
            Assert.IsFalse(_minigame.IsFinished);

            _minigame.Tick(0.5f);
            Assert.IsTrue(_minigame.IsFinished);
        }
    }
}