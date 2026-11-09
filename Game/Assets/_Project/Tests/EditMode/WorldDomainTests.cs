using System;
using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using Minecraft.Domain.Blocks;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.World;

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

        [Test]
        public void Chunk_GetAndSetBlock_EncapsulatesDataAndMarksDirty()
        {
            ChunkCoord coord = new ChunkCoord(2, -3);
            Chunk chunk = new Chunk(coord);

            Assert.That(chunk.Coordinate, Is.EqualTo(coord));
            Assert.That(chunk.IsDirty, Is.False);
            Assert.That(chunk.GetBlock(0, 0, 0), Is.EqualTo(BlockId.Air));

            bool changed = chunk.SetBlock(5, 12, 7, BlockId.Stone);
            Assert.That(changed, Is.True);
            Assert.That(chunk.IsDirty, Is.True);
            Assert.That(chunk.GetBlock(5, 12, 7), Is.EqualTo(BlockId.Stone));

            // Đặt cùng loại block thì trả về false và không đổi trạng thái
            chunk.MarkClean();
            Assert.That(chunk.IsDirty, Is.False);
            Assert.That(chunk.SetBlock(5, 12, 7, BlockId.Stone), Is.False);
            Assert.That(chunk.IsDirty, Is.False);
        }

        [Test]
        public void Chunk_OutOfBoundsAccess_ThrowsArgumentOutOfRangeException()
        {
            Chunk chunk = new Chunk(new ChunkCoord(0, 0));

            Assert.Throws<ArgumentOutOfRangeException>(() => chunk.GetBlock(-1, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => chunk.GetBlock(0, -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => chunk.GetBlock(0, 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => chunk.GetBlock(16, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => chunk.GetBlock(0, 64, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => chunk.GetBlock(0, 0, 16));

            Assert.Throws<ArgumentOutOfRangeException>(() => chunk.SetBlock(-1, 0, 0, BlockId.Stone));
            Assert.Throws<ArgumentOutOfRangeException>(() => chunk.SetBlock(0, 64, 0, BlockId.Stone));
        }

        [Test]
        public void FlatTerrainGenerator_GeneratesCorrectLayers()
        {
            ITerrainGenerator flatGen = new FlatTerrainGenerator(surfaceHeight: 16);

            // Y = 0 là Bedrock
            Assert.That(flatGen.GetBlockAt(new BlockPos(5, 0, 8), 12345), Is.EqualTo(BlockId.Bedrock));
            // 1 <= Y < 16 là Dirt
            Assert.That(flatGen.GetBlockAt(new BlockPos(5, 1, 8), 12345), Is.EqualTo(BlockId.Dirt));
            Assert.That(flatGen.GetBlockAt(new BlockPos(5, 15, 8), 12345), Is.EqualTo(BlockId.Dirt));
            // Y = 16 là Grass
            Assert.That(flatGen.GetBlockAt(new BlockPos(5, 16, 8), 12345), Is.EqualTo(BlockId.Grass));
            // Y > 16 là Air
            Assert.That(flatGen.GetBlockAt(new BlockPos(5, 17, 8), 12345), Is.EqualTo(BlockId.Air));
        }

        [Test]
        public void PerlinTerrainGenerator_DeterministicAndWithinHeightRange()
        {
            ITerrainGenerator perlinGen = new PerlinTerrainGenerator(minHeight: 16, maxHeight: 40, scale: 0.05f);
            int seed = 20261109;

            // Tính tất định: cùng tọa độ và seed sinh cùng khối
            BlockPos samplePos = new BlockPos(32, 20, -15);
            BlockId block1 = perlinGen.GetBlockAt(samplePos, seed);
            BlockId block2 = perlinGen.GetBlockAt(samplePos, seed);
            Assert.That(block1, Is.EqualTo(block2));

            // Đáy Y = 0 luôn là Bedrock
            Assert.That(perlinGen.GetBlockAt(new BlockPos(32, 0, -15), seed), Is.EqualTo(BlockId.Bedrock));

            // Đỉnh Y >= 45 luôn là Air (vì maxHeight = 40)
            Assert.That(perlinGen.GetBlockAt(new BlockPos(32, 45, -15), seed), Is.EqualTo(BlockId.Air));
        }
    }
}
