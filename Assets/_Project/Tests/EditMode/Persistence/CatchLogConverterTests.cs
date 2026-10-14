using System;
using NUnit.Framework;
using PocketFish.Domain.Catch;
using PocketFish.Domain.Collection;
using PocketFish.Game.Persistence;

namespace PocketFish.Game.Tests
{
    public sealed class CatchLogConverterTests
    {
        private static readonly DateTime T0 = new DateTime(2026, 3, 4, 5, 6, 7, DateTimeKind.Utc);

        [Test]
        public void RoundTrip_KeepsEveryField()
        {
            var original = new CatchRecord("catfish", Quality.Great, "golden", T0);
            Assert.IsTrue(CatchLogConverter.TryToRecord(CatchLogConverter.ToEntry(original), out CatchRecord back));
            Assert.AreEqual("catfish", back.SpeciesId);
            Assert.AreEqual(Quality.Great, back.Quality);
            Assert.AreEqual("golden", back.VariantId);
            Assert.AreEqual(T0, back.CaughtUtc);
            Assert.AreEqual(DateTimeKind.Utc, back.CaughtUtc.Kind);
        }

        [Test]
        public void RoundTrip_KeepsNullVariant()
        {
            var original = new CatchRecord("catfish", Quality.Normal, null, T0);
            Assert.IsTrue(CatchLogConverter.TryToRecord(CatchLogConverter.ToEntry(original), out CatchRecord back));
            Assert.IsNull(back.VariantId);
        }

        [Test]
        public void UnknownQuality_IsRejected()
        {
            var entry = new CatchLogEntry { SpeciesId = "catfish", Quality = 99, CaughtUtcTicks = T0.Ticks };
            Assert.IsFalse(CatchLogConverter.TryToRecord(entry, out _));
        }

        [Test]
        public void MissingSpeciesIdOrEntry_IsRejected()
        {
            Assert.IsFalse(CatchLogConverter.TryToRecord(null, out _));
            Assert.IsFalse(CatchLogConverter.TryToRecord(new CatchLogEntry { CaughtUtcTicks = T0.Ticks }, out _));
        }

        [Test]
        public void OutOfRangeTicks_AreRejected()
        {
            var entry = new CatchLogEntry { SpeciesId = "catfish", Quality = 0, CaughtUtcTicks = -5 };
            Assert.IsFalse(CatchLogConverter.TryToRecord(entry, out _));
        }
    }
}
