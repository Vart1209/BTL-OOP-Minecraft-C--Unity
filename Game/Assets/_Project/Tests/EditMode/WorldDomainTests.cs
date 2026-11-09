using System;
using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using Minecraft.Domain.Blocks;
using Minecraft.Domain.Contracts;
// using Minecraft.Domain.World;

namespace Minecraft.Domain.Tests
{
    [TestFixture]
    public class WorldDomainTests
    {
        [Test]
        public void BlockType_PropertiesAndTextureMapping_AreEncapsulatedCorrectly()
        {
            BlockType grass = new BlockType(
                id: BlockId.Grass,
                name: "Grass Block",
                isSolid: true,
                isTransparent: false,
                hardness: 0.6f,
                dropItemId: ItemId.Dirt,
                topTextureIndex: 0,
                bottomTextureIndex: 2,
                sideTextureIndex: 3
            );

            Assert.That(grass.Id, Is.EqualTo(BlockId.Grass));
            Assert.That(grass.Name, Is.EqualTo("Grass Block"));
            Assert.That(grass.IsSolid, Is.True);
            Assert.That(grass.IsTransparent, Is.False);
            Assert.That(grass.Hardness, Is.EqualTo(0.6f));
            Assert.That(grass.DropItemId, Is.EqualTo(ItemId.Dirt));

            Assert.That(grass.GetTextureIndex(BlockFace.Top), Is.EqualTo(0));
            Assert.That(grass.GetTextureIndex(BlockFace.Bottom), Is.EqualTo(2));
            Assert.That(grass.GetTextureIndex(BlockFace.North), Is.EqualTo(3));
            Assert.That(grass.GetTextureIndex(BlockFace.South), Is.EqualTo(3));
            Assert.That(grass.GetTextureIndex(BlockFace.East), Is.EqualTo(3));
            Assert.That(grass.GetTextureIndex(BlockFace.West), Is.EqualTo(3));
        }

        [Test]
        public void BlockType_InvalidHardness_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new BlockType(
                id: BlockId.Stone,
                name: "Stone",
                isSolid: true,
                isTransparent: false,
                hardness: -2.0f,
                dropItemId: ItemId.Cobblestone,
                topTextureIndex: 1,
                bottomTextureIndex: 1,
                sideTextureIndex: 1
            ));
        }

        [Test]
        public void BlockRegistry_DefaultRegistry_SatisfiesRuleR5DropsAndRegistersCoreBlocks()
        {
            BlockRegistry registry = BlockRegistry.CreateDefault();

            // Phá Stone rơi Cobblestone (Quy tắc R5)
            BlockType stone = registry.Get(BlockId.Stone);
            Assert.That(stone.DropItemId, Is.EqualTo(ItemId.Cobblestone));

            // Phá Grass rơi Dirt (Quy tắc R5)
            BlockType grass = registry.Get(BlockId.Grass);
            Assert.That(grass.DropItemId, Is.EqualTo(ItemId.Dirt));

            // Phá Leaves rơi None / Air (Quy tắc R5)
            BlockType leaves = registry.Get(BlockId.Leaves);
            Assert.That(leaves.DropItemId, Is.EqualTo(ItemId.None));
            Assert.That(leaves.IsTransparent, Is.True);

            // Bedrock không thể phá trong Survival (Hardness = -1f)
            BlockType bedrock = registry.Get(BlockId.Bedrock);
            Assert.That(bedrock.Hardness, Is.EqualTo(-1.0f));

            // Đảm bảo Air, Wood, Planks, CraftingTable đều có trong registry
            Assert.That(registry.Contains(BlockId.Air), Is.True);
            Assert.That(registry.Contains(BlockId.Wood), Is.True);
            Assert.That(registry.Contains(BlockId.Planks), Is.True);
            Assert.That(registry.Contains(BlockId.CraftingTable), Is.True);
            Assert.That(registry.Get(BlockId.Wood).DropItemId, Is.EqualTo(ItemId.Wood));
        }
    }
}
