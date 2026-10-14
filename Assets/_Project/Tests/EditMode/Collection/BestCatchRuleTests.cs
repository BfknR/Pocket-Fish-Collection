using System;
using NUnit.Framework;
using PocketFish.Domain.Catch;
using PocketFish.Domain.Collection;

namespace PocketFish.Domain.Tests
{
    public sealed class BestCatchRuleTests
    {
        private static readonly DateTime T0 = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static CatchRecord Make(Quality quality, int day = 0) =>
            new CatchRecord("fish", quality, null, T0.AddDays(day));

        [Test]
        public void HigherQuality_WinsEvenWhenLater()
        {
            Assert.IsTrue(BestCatchRule.IsBetter(Make(Quality.Great, 9), Make(Quality.Good, 1)));
            Assert.IsFalse(BestCatchRule.IsBetter(Make(Quality.Good, 1), Make(Quality.Great, 9)));
        }

        [Test]
        public void EqualQuality_EarlierCatchWins()
        {
            Assert.IsTrue(BestCatchRule.IsBetter(Make(Quality.Good, 1), Make(Quality.Good, 5)));
            Assert.IsFalse(BestCatchRule.IsBetter(Make(Quality.Good, 5), Make(Quality.Good, 1)));
        }

        [Test]
        public void FullTie_KeepsCurrent()
        {
            Assert.IsFalse(BestCatchRule.IsBetter(Make(Quality.Good, 3), Make(Quality.Good, 3)));
        }
    }
}
