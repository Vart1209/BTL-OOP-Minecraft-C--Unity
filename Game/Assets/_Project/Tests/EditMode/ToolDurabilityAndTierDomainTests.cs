using System;
using NUnit.Framework;
using Minecraft.Domain.Blocks;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.Items;

namespace Minecraft.Domain.Tests
{
    [TestFixture]
    public class ToolDurabilityAndTierDomainTests
    {
        [Test]
        public void ToolItem_DurabilityAndTier_AreConfiguredCorrectly()
        {
            ToolItem woodPick = new ToolItem(
                id: ItemId.WoodenPickaxe,
                name: "Wooden Pickaxe",
                toolCategory: ToolType.Pickaxe,
                miningSpeedMultiplier: 2.0f,
                attackDamage: 2,
                tier: ToolTier.Wood,
                maxDurability: 59
            );

            Assert.That(woodPick.Tier, Is.EqualTo(ToolTier.Wood));
            Assert.That(woodPick.MaxDurability, Is.EqualTo(59));

            ItemStack stack = new ItemStack(woodPick, 1);
            Assert.That(stack.Durability, Is.EqualTo(59));
            Assert.That(stack.MaxDurability, Is.EqualTo(59));

            // Trừ 10 độ bền
            bool damaged = stack.TryDamageTool(10, out bool isBroken);
            Assert.That(damaged, Is.True);
            Assert.That(isBroken, Is.False);
            Assert.That(stack.Durability, Is.EqualTo(49));

            // Trừ nốt 49 độ bền -> gãy cúp
            bool damagedToBreak = stack.TryDamageTool(49, out bool finalBreak);
            Assert.That(damagedToBreak, Is.True);
            Assert.That(finalBreak, Is.True);
            Assert.That(stack.IsEmpty, Is.True);
            Assert.That(stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void BlockType_RequiredToolTier_IsConfiguredInRegistry()
        {
            BlockRegistry registry = BlockRegistry.CreateDefault();

            // Khối mềm không cần công cụ (Tier Hand)
            Assert.That(registry.Get(BlockId.Dirt).RequiredToolTier, Is.EqualTo(ToolTier.Hand));
            Assert.That(registry.Get(BlockId.Wood).RequiredToolTier, Is.EqualTo(ToolTier.Hand));

            // Đá và Than cần tối thiểu Cúp Gỗ (Tier Wood)
            Assert.That(registry.Get(BlockId.Stone).RequiredToolTier, Is.EqualTo(ToolTier.Wood));
            Assert.That(registry.Get(BlockId.CoalOre).RequiredToolTier, Is.EqualTo(ToolTier.Wood));

            // Sắt cần tối thiểu Cúp Đá (Tier Stone)
            Assert.That(registry.Get(BlockId.IronOre).RequiredToolTier, Is.EqualTo(ToolTier.Stone));

            // Vàng và Kim cương cần tối thiểu Cúp Sắt/Kim cương (Tier Iron)
            Assert.That(registry.Get(BlockId.GoldOre).RequiredToolTier, Is.EqualTo(ToolTier.Iron));
            Assert.That(registry.Get(BlockId.DiamondOre).RequiredToolTier, Is.EqualTo(ToolTier.Iron));
        }
    }
}
