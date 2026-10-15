using NUnit.Framework;
using PocketFish.Domain.Progression;

namespace PocketFish.Domain.Tests
{
    public sealed class PlayerProgressTests
    {
        private static UpgradeSpec Item(string id) => new UpgradeSpec(id, new[] { 10, 20, 30 });

        private static AquariumSpec Aquarium(string id, int cost) =>
            new AquariumSpec(id, cost, new[] { Item("a"), Item("b") });

        private static PlayerProgress NewProgress() =>
            new PlayerProgress(new[] { Aquarium("one", 0), Aquarium("two", 100), Aquarium("three", 500) });

        private static void MaxOut(PlayerProgress p, int aquarium)
        {
            p.AddCoins(1000);
            for (int item = 0; item < 2; item++)
            {
                for (int level = 0; level < 3; level++) p.TryUpgrade(aquarium, item);
            }
        }

        [Test]
        public void NewProgress_OwnsOnlyTheFirstAquarium_WithNoCoins()
        {
            var p = NewProgress();

            Assert.AreEqual(0, p.Coins);
            Assert.AreEqual(1, p.OwnedCount);
            Assert.AreEqual(0, p.SelectedIndex);
            Assert.IsTrue(p.IsOwned(0));
            Assert.IsFalse(p.IsOwned(1));
        }

        [Test]
        public void AddCoins_IgnoresZeroAndNegative()
        {
            var p = NewProgress();

            p.AddCoins(5);
            p.AddCoins(0);
            p.AddCoins(-3);

            Assert.AreEqual(5, p.Coins);
        }

        [Test]
        public void TryUpgrade_SpendsCoinsAndRaisesTheLevel()
        {
            var p = NewProgress();
            p.AddCoins(25);

            Assert.AreEqual(PurchaseResult.Success, p.TryUpgrade(0, 0));
            Assert.AreEqual(15, p.Coins);
            Assert.AreEqual(1, p.LevelOf(0, 0));
            Assert.AreEqual(20, p.NextUpgradeCost(0, 0));
        }

        [Test]
        public void TryUpgrade_NotEnoughCoins_ChangesNothing()
        {
            var p = NewProgress();
            p.AddCoins(9);

            Assert.AreEqual(PurchaseResult.NotEnoughCoins, p.TryUpgrade(0, 0));
            Assert.AreEqual(9, p.Coins);
            Assert.AreEqual(0, p.LevelOf(0, 0));
        }

        [Test]
        public void TryUpgrade_AtMaxLevel_IsRefused()
        {
            var p = NewProgress();
            MaxOut(p, 0);

            Assert.AreEqual(PurchaseResult.AlreadyMaxLevel, p.TryUpgrade(0, 0));
            Assert.IsNull(p.NextUpgradeCost(0, 0));
        }

        [Test]
        public void TryUpgrade_UnownedAquarium_IsRefused()
        {
            var p = NewProgress();
            p.AddCoins(1000);

            Assert.AreEqual(PurchaseResult.NotOwned, p.TryUpgrade(1, 0));
            Assert.AreEqual(1000, p.Coins);
        }

        [TestCase(-1, 0)]
        [TestCase(0, 5)]
        [TestCase(9, 0)]
        public void TryUpgrade_UnknownIndexes_AreRefused(int aquarium, int item)
        {
            Assert.AreEqual(PurchaseResult.Unknown, NewProgress().TryUpgrade(aquarium, item));
        }

        [Test]
        public void IsFullyUpgraded_NeedsEveryItemMaxed()
        {
            var p = NewProgress();
            p.AddCoins(1000);
            for (int level = 0; level < 3; level++) p.TryUpgrade(0, 0);

            Assert.IsFalse(p.IsFullyUpgraded(0));

            for (int level = 0; level < 3; level++) p.TryUpgrade(0, 1);

            Assert.IsTrue(p.IsFullyUpgraded(0));
        }

        [Test]
        public void BuyNextAquarium_IsLockedUntilTheCurrentOneIsFullyUpgraded()
        {
            var p = NewProgress();
            p.AddCoins(1000);

            Assert.IsFalse(p.CanBuyNextAquarium);
            Assert.AreEqual(PurchaseResult.NotAvailable, p.TryBuyNextAquarium());
            Assert.AreEqual(1000, p.Coins);
        }

        [Test]
        public void BuyNextAquarium_NotEnoughCoins_ChangesNothing()
        {
            var p = NewProgress();
            MaxOut(p, 0);
            int coins = p.Coins;
            p.Restore(50, 1, 0, new[] { new[] { 3, 3 } });

            Assert.IsTrue(p.CanBuyNextAquarium);
            Assert.AreEqual(PurchaseResult.NotEnoughCoins, p.TryBuyNextAquarium());
            Assert.AreEqual(1, p.OwnedCount);
            Assert.AreEqual(50, p.Coins);
            Assert.Greater(coins, 0);
        }

        [Test]
        public void BuyNextAquarium_SpendsCoins_OwnsItAndSelectsIt()
        {
            var p = NewProgress();
            MaxOut(p, 0);
            p.AddCoins(1000);
            int before = p.Coins;

            Assert.AreEqual(PurchaseResult.Success, p.TryBuyNextAquarium());
            Assert.AreEqual(before - 100, p.Coins);
            Assert.AreEqual(2, p.OwnedCount);
            Assert.AreEqual(1, p.SelectedIndex);
            Assert.IsFalse(p.CanBuyNextAquarium, "the new aquarium must be upgraded before the next purchase");
        }

        [Test]
        public void BuyNextAquarium_AfterTheLastOne_IsNotAvailable()
        {
            var p = NewProgress();
            MaxOut(p, 0);
            p.TryBuyNextAquarium();
            MaxOut(p, 1);
            p.TryBuyNextAquarium();
            MaxOut(p, 2);

            Assert.IsNull(p.NextAquariumCost);
            Assert.AreEqual(PurchaseResult.NotAvailable, p.TryBuyNextAquarium());
        }

        [Test]
        public void Select_OnlyOwnedAquariums()
        {
            var p = NewProgress();

            Assert.IsFalse(p.Select(1));
            Assert.AreEqual(0, p.SelectedIndex);
        }

        [Test]
        public void Select_GoesBackToAnEarlierAquarium()
        {
            var p = NewProgress();
            MaxOut(p, 0);
            p.TryBuyNextAquarium();

            Assert.IsTrue(p.Select(0));
            Assert.AreEqual(0, p.SelectedIndex);
        }

        [Test]
        public void Restore_ClampsImpossibleValues()
        {
            var p = NewProgress();

            p.Restore(-50, 99, 99, new[] { new[] { 99, -4 } });

            Assert.AreEqual(0, p.Coins);
            Assert.AreEqual(3, p.OwnedCount);
            Assert.AreEqual(2, p.SelectedIndex);
            Assert.AreEqual(3, p.LevelOf(0, 0));
            Assert.AreEqual(0, p.LevelOf(0, 1));
        }

        [Test]
        public void Changed_FiresOnEveryChange_ButNotOnRefusals()
        {
            var p = NewProgress();
            int count = 0;
            p.Changed += () => count++;

            p.AddCoins(10);
            p.TryUpgrade(0, 0);
            p.TryUpgrade(0, 0);   // refused: not enough coins left

            Assert.AreEqual(2, count);
        }
    }
}
