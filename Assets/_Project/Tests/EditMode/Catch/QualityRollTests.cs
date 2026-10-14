using System;
using NUnit.Framework;
using PocketFish.Domain.Catch;

namespace PocketFish.Domain.Tests
{
    public sealed class QualityRollTests
    {
        /// <summary>Returns a fixed value from NextDouble so a roll lands exactly where the test wants.</summary>
        private sealed class FakeRandom : Random
        {
            private readonly double _value;
            public FakeRandom(double value) { _value = value; }
            public override double NextDouble() => _value;
        }

        // Weights 1/2/4/1 over a total of 8 give binary-exact boundaries at 0.125, 0.375 and 0.875.
        private static readonly QualityOdds Odds = new QualityOdds(normal: 1f, good: 2f, great: 4f, perfect: 1f);

        // 8
        [TestCase(0.0, Quality.Normal)]
        [TestCase(0.124, Quality.Normal)]
        [TestCase(0.125, Quality.Good)]     // boundary belongs to the next bucket
        [TestCase(0.374, Quality.Good)]
        [TestCase(0.375, Quality.Great)]
        [TestCase(0.874, Quality.Great)]
        [TestCase(0.875, Quality.Perfect)]
        [TestCase(0.999, Quality.Perfect)]
        public void Roll_ReturnsExpectedQualityAtEachWeightBoundary(double roll, Quality expected)
        {
            Assert.AreEqual(expected, CatchRoller.PickQuality(Odds, new FakeRandom(roll)));
        }

        // 9
        [Test]
        public void Roll_RejectsZeroTotal()
        {
            Assert.Catch<ArgumentException>(() => CatchRoller.PickQuality(default, new FakeRandom(0.5)));
        }

        [TestCase(-1f, 1f, 1f, 1f)]
        [TestCase(1f, -0.001f, 1f, 1f)]
        [TestCase(0f, 0f, 0f, 0f)]
        [TestCase(float.NaN, 1f, 1f, 1f)]
        [TestCase(1f, float.PositiveInfinity, 1f, 1f)]
        public void Odds_RejectInvalidWeights(float normal, float good, float great, float perfect)
        {
            Assert.Catch<ArgumentException>(() => new QualityOdds(normal, good, great, perfect));
        }

        // 10
        [TestCase(0.0)]
        [TestCase(0.5)]
        [TestCase(0.9999999)]
        [TestCase(1.0)]  // out of range on purpose, as float rounding can push a roll to the edge
        public void Roll_NeverReturnsZeroWeightQuality(double roll)
        {
            var onlyGood = new QualityOdds(0f, 1f, 0f, 0f);
            var noPerfect = new QualityOdds(1f, 1f, 1f, 0f);

            Assert.AreEqual(Quality.Good, CatchRoller.PickQuality(onlyGood, new FakeRandom(roll)));
            Assert.AreNotEqual(Quality.Perfect, CatchRoller.PickQuality(noPerfect, new FakeRandom(roll)));
        }
    }
}
