using NUnit.Framework;
using PocketFish.Domain.Catch;
using PocketFish.Domain.Progression;

namespace PocketFish.Domain.Tests
{
    public sealed class CoinRewardTableTests
    {
        private static CoinRewardTable NewTable() =>
            new CoinRewardTable(new[] { 10, 25, 60, 150 }, new[] { 1f, 1.5f, 2f, 3f });

        [Test]
        public void NormalQuality_PaysTheBaseAmount()
        {
            Assert.AreEqual(10, NewTable().For(Rarity.Common, Quality.Normal));
            Assert.AreEqual(150, NewTable().For(Rarity.Legendary, Quality.Normal));
        }

        [Test]
        public void Quality_MultipliesTheBaseAmount()
        {
            Assert.AreEqual(30, NewTable().For(Rarity.Common, Quality.Perfect));
            Assert.AreEqual(38, NewTable().For(Rarity.Uncommon, Quality.Good)); // 37.5 rounds up
        }

        [Test]
        public void RarityWithNoEntry_PaysNothing()
        {
            var table = new CoinRewardTable(new[] { 10 }, new[] { 1f });

            Assert.AreEqual(0, table.For(Rarity.Rare, Quality.Normal));
        }

        [Test]
        public void QualityWithNoEntry_CountsAsTimesOne()
        {
            var table = new CoinRewardTable(new[] { 10 }, new[] { 1f });

            Assert.AreEqual(10, table.For(Rarity.Common, Quality.Perfect));
        }

        [Test]
        public void NegativeMultiplier_NeverPaysNegative()
        {
            var table = new CoinRewardTable(new[] { 10 }, new[] { -2f });

            Assert.AreEqual(0, table.For(Rarity.Common, Quality.Normal));
        }
    }
}
