using NUnit.Framework;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.GameModes;

namespace Minecraft.Domain.Tests
{
    [TestFixture]
    public class GameModeTests
    {
        [Test]
        public void SurvivalMode_EnforcesSurvivalRulesAndLimits()
        {
            IGameMode mode = new SurvivalMode();

            Assert.AreEqual("Survival", mode.ModeName);
            Assert.IsTrue(mode.CanTakeDamage);
            Assert.IsTrue(mode.ConsumesItemsOnPlace);
            Assert.IsFalse(mode.InstantBreak);
            Assert.IsFalse(mode.CanFly);

            // Rule R1: Bedrock cannot be broken in Survival
            Assert.IsFalse(mode.CanBreakBlock(BlockId.Bedrock));
            Assert.IsTrue(mode.CanBreakBlock(BlockId.Stone));
            Assert.IsTrue(mode.CanBreakBlock(BlockId.Dirt));
            Assert.IsTrue(mode.CanBreakBlock(BlockId.Wood));
        }

        [Test]
        public void CreativeMode_EnforcesCreativePrivileges()
        {
            IGameMode mode = new CreativeMode();

            Assert.AreEqual("Creative", mode.ModeName);
            // Rule R12: Creative has invulnerability, unlimited blocks, instant break, fly, breaks bedrock
            Assert.IsFalse(mode.CanTakeDamage);
            Assert.IsFalse(mode.ConsumesItemsOnPlace);
            Assert.IsTrue(mode.InstantBreak);
            Assert.IsTrue(mode.CanFly);

            Assert.IsTrue(mode.CanBreakBlock(BlockId.Bedrock));
            Assert.IsTrue(mode.CanBreakBlock(BlockId.Stone));
        }

        [Test]
        public void AdventureMode_PreventsBreakingAnyBlock()
        {
            IGameMode mode = new AdventureMode();

            Assert.AreEqual("Adventure", mode.ModeName);
            Assert.IsTrue(mode.CanTakeDamage);
            Assert.IsTrue(mode.ConsumesItemsOnPlace);
            Assert.IsFalse(mode.InstantBreak);
            Assert.IsFalse(mode.CanFly);

            // Adventure expansion: cannot break any block
            Assert.IsFalse(mode.CanBreakBlock(BlockId.Bedrock));
            Assert.IsFalse(mode.CanBreakBlock(BlockId.Stone));
            Assert.IsFalse(mode.CanBreakBlock(BlockId.Dirt));
            Assert.IsFalse(mode.CanBreakBlock(BlockId.Wood));
        }
    }
}
