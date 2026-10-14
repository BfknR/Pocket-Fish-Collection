using NUnit.Framework;
using PocketFish.Domain.Catch;
using PocketFish.Domain.Fishing;

namespace PocketFish.Domain.Tests
{
    public sealed class MultiTapMinigameTests
    {
        // Binary-exact values so the average and the zone edges don't depend on float rounding.
        // With a 1 second sweep the marker position equals elapsed time on the way out:
        // 0.5s is Perfect, 0.625s is Great (edge), 0.75s is Good (edge).
        private static readonly MinigameConfig Tier = new MinigameConfig(
            sweepSeconds: 1f, timeLimitSeconds: 2f,
            goodHalfWidth: 0.25f, greatHalfWidth: 0.125f, perfectHalfWidth: 0.0625f);

        private static readonly TapGradeScores Scores = new TapGradeScores(miss: 0f, good: 0.5f, great: 0.75f, perfect: 1f);
        private static readonly GradeThresholds Thresholds = new GradeThresholds(goodMin: 0.40f, greatMin: 0.65f, perfectMin: 0.85f);

        private static MultiTapMinigame Create(int taps, params MinigameConfig[] tiers)
        {
            if (tiers.Length == 0) tiers = new[] { Tier, Tier, Tier, Tier };
            var game = new MultiTapMinigame(taps, tiers, Scores, Thresholds);
            game.Begin();
            return game;
        }

        private static void TapAfter(MultiTapMinigame game, float seconds)
        {
            game.Tick(seconds);
            game.Tap();
        }

        // 1
        [TestCase(Rarity.Common, 1)]
        [TestCase(Rarity.Uncommon, 2)]
        [TestCase(Rarity.Rare, 3)]
        [TestCase(Rarity.Legendary, 4)]
        public void TapCount_MatchesRarity(Rarity rarity, int expected)
        {
            var counts = new RarityTapCounts(common: 1, uncommon: 2, rare: 3, legendary: 4);

            Assert.AreEqual(expected, counts.For(rarity));
        }

        // 2
        [Test]
        public void Average_IsExactMeanOfScores()
        {
            var game = Create(4);
            TapAfter(game, 0.5f);    // Perfect 1
            TapAfter(game, 0.625f);  // Great 0.75
            TapAfter(game, 0.75f);   // Good 0.5
            game.Tick(2f);           // timeout 0

            Assert.IsTrue(game.IsFinished);
            Assert.AreEqual(2.25f / 4f, game.Result.AverageScore); // 0.5625, no rounding
        }

        // 3
        [Test]
        public void TimedOutAttempt_ScoresZeroAndCountsInAverage()
        {
            var game = Create(2);
            TapAfter(game, 0.5f);    // Perfect 1
            game.Tick(2f);           // timeout

            TapAttemptResult timedOut = game.Result.Attempts[1];
            Assert.AreEqual(TapGrade.Miss, timedOut.Grade);
            Assert.AreEqual(0f, timedOut.Score);
            Assert.AreEqual(0.5f, game.Result.AverageScore);
        }

        // 4
        [Test]
        public void EachAttempt_UsesTierForItsIndex()
        {
            var shortTier = new MinigameConfig(1f, 1f, 0.25f, 0.125f, 0.0625f);
            var longTier = new MinigameConfig(1f, 3f, 0.25f, 0.125f, 0.0625f);
            var game = Create(2, shortTier, longTier);

            Assert.AreEqual(shortTier.TimeLimitSeconds, game.CurrentTier.TimeLimitSeconds);
            game.Tick(1f);  // first tier times out at 1s
            Assert.AreEqual(1, game.CurrentIndex);
            Assert.AreEqual(longTier.TimeLimitSeconds, game.CurrentTier.TimeLimitSeconds);

            game.Tick(2f);  // would have timed out under the first tier, not under the second
            Assert.IsFalse(game.IsFinished);
            game.Tick(1f);
            Assert.IsTrue(game.IsFinished);
        }

        // 5
        [Test]
        public void TapsAfterFinalAttempt_AreIgnored()
        {
            var game = Create(1);
            TapAfter(game, 0.5f);
            MinigameResult result = game.Result;

            game.Tick(0.5f);
            game.Tap();
            game.Tap();

            Assert.AreSame(result, game.Result);
            Assert.AreEqual(1, game.Result.Attempts.Count);
            Assert.AreEqual(1f, game.Result.AverageScore);
        }

        // 6
        [Test]
        public void Attempts_AreStoredInOrder()
        {
            var game = Create(3);
            TapAfter(game, 0.75f);   // Good
            TapAfter(game, 0.5f);    // Perfect
            game.Tick(2f);           // Miss

            var attempts = game.Result.Attempts;
            Assert.AreEqual(3, attempts.Count);
            for (int i = 0; i < 3; i++) Assert.AreEqual(i, attempts[i].Index);
            Assert.AreEqual(TapGrade.Good, attempts[0].Grade);
            Assert.AreEqual(TapGrade.Perfect, attempts[1].Grade);
            Assert.AreEqual(TapGrade.Miss, attempts[2].Grade);
            Assert.AreEqual(0.5f, attempts[0].Score);
            Assert.AreEqual(1f, attempts[1].Score);
        }

        // 7
        [TestCase(0.85f, TapGrade.Perfect)]
        [TestCase(0.8499f, TapGrade.Great)]
        [TestCase(0.65f, TapGrade.Great)]
        [TestCase(0.6499f, TapGrade.Good)]
        [TestCase(0.40f, TapGrade.Good)]
        [TestCase(0.3999f, TapGrade.Miss)]
        [TestCase(0f, TapGrade.Miss)]
        [TestCase(1f, TapGrade.Perfect)]
        public void Average_MapsToFinalGradeAtBoundaries(float average, TapGrade expected)
        {
            Assert.AreEqual(expected, Thresholds.GradeFor(average));
        }

        [Test]
        public void FinalGrade_ComesFromAverage()
        {
            var game = Create(2);
            TapAfter(game, 0.5f);    // 1
            TapAfter(game, 0.625f);  // 0.75 -> average 0.875

            Assert.AreEqual(TapGrade.Perfect, game.Result.FinalGrade);
        }

        [Test]
        public void Begin_ResetsAFinishedMinigame()
        {
            var game = Create(1);
            TapAfter(game, 0.5f);

            game.Begin();

            Assert.IsFalse(game.IsFinished);
            Assert.AreEqual(0, game.CurrentIndex);
            Assert.IsNull(game.Result);
        }

        [Test]
        public void Constructor_RejectsFewerTiersThanTaps()
        {
            Assert.Throws<System.ArgumentException>(
                () => new MultiTapMinigame(3, new[] { Tier, Tier }, Scores, Thresholds));
        }

        [Test]
        public void Thresholds_MustBeAscending()
        {
            Assert.Throws<System.ArgumentException>(() => new GradeThresholds(0.65f, 0.40f, 0.85f));
        }
    }
}
