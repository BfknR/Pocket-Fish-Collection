using NUnit.Framework;
using PocketFish.Game.UI;
using UnityEngine;

namespace PocketFish.Game.Tests
{
    public sealed class CollapsingArtMathTests
    {
        private static readonly ArtPose Large = new ArtPose(600f, new Vector2(540f, -420f));
        private static readonly ArtPose Thumb = new ArtPose(160f, new Vector2(104f, -104f));

        [Test]
        public void Progress_IsZeroAtTop_AndClampsOverscroll()
        {
            Assert.AreEqual(0f, CollapsingArtMath.Progress(0f, 440f));
            Assert.AreEqual(0f, CollapsingArtMath.Progress(-50f, 440f));
        }

        [Test]
        public void Progress_ReachesOneAtCollapseDistance_AndStaysThere()
        {
            Assert.AreEqual(0.5f, CollapsingArtMath.Progress(220f, 440f), 0.0001f);
            Assert.AreEqual(1f, CollapsingArtMath.Progress(440f, 440f));
            Assert.AreEqual(1f, CollapsingArtMath.Progress(5000f, 440f));
        }

        [Test]
        public void Progress_WithNoDistance_IsFullyCollapsed()
        {
            Assert.AreEqual(1f, CollapsingArtMath.Progress(0f, 0f));
        }

        [Test]
        public void Evaluate_AtZero_IsLarge()
        {
            ArtPose pose = CollapsingArtMath.Evaluate(0f, Large, Thumb);
            Assert.AreEqual(600f, pose.Size);
            Assert.AreEqual(Large.Center, pose.Center);
        }

        [Test]
        public void Evaluate_AtFullProgress_ReachesThumbnailSizeAndPosition()
        {
            ArtPose pose = CollapsingArtMath.Evaluate(1f, Large, Thumb);
            Assert.AreEqual(160f, pose.Size);
            Assert.AreEqual(Thumb.Center, pose.Center);
        }

        [Test]
        public void Evaluate_Midway_IsBetween()
        {
            ArtPose pose = CollapsingArtMath.Evaluate(0.5f, Large, Thumb);
            Assert.AreEqual(380f, pose.Size, 0.0001f);
        }
    }
}
