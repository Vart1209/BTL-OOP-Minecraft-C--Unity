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
    public class VoxelInteractionTests
    {
        [Test]
        public void VoxelRaycaster_HitsSolidBlockWithinRange_ReturnsCorrectHitInfo()
        {
            BlockRegistry registry = BlockRegistry.CreateDefault();
            FlatTerrainGenerator flatGen = new FlatTerrainGenerator(surfaceHeight: 1); // Y=0 Bedrock, Y>0 Air
            World.World world = new World.World(flatGen);
            world.GenerateChunk(new ChunkCoord(0, 0));

            // Đặt 1 khối đá tại (5, 5, 5)
            world.SetBlock(new BlockPos(5, 5, 5), BlockId.Stone);

            VoxelRaycaster raycaster = new VoxelRaycaster(world, registry);

            // Bắn tia từ (5.5, 5.5, 2.5) hướng theo trục +Z về phía (5, 5, 5)
            // Khoảng cách khoảng 2.0 block (trong tầm 5.0 block theo Quy tắc R7)
            Vector3 origin = new Vector3(5.5f, 5.5f, 2.5f);
            Vector3 direction = new Vector3(0f, 0f, 1f);

            VoxelRaycastHit hit = raycaster.Raycast(origin, direction, maxDistance: 5.0f);

            Assert.That(hit.IsHit, Is.True);
            Assert.That(hit.BlockPosition, Is.EqualTo(new BlockPos(5, 5, 5)));
            // Ô kề mặt trước là (5, 5, 4) - mặt Nam của khối (South Face)
            Assert.That(hit.AdjacentPosition, Is.EqualTo(new BlockPos(5, 5, 4)));
            Assert.That(hit.HitFace, Is.EqualTo(BlockFace.South));
            Assert.That(hit.Distance, Is.GreaterThan(1.0f).And.LessThan(3.0f));
        }

        [Test]
        public void VoxelRaycaster_BeyondMaxDistance_ReturnsMiss()
        {
            BlockRegistry registry = BlockRegistry.CreateDefault();
            FlatTerrainGenerator flatGen = new FlatTerrainGenerator(surfaceHeight: 1);
            World.World world = new World.World(flatGen);
            world.GenerateChunk(new ChunkCoord(0, 0));

            // Khối đá đặt tại (5, 5, 15)
            world.SetBlock(new BlockPos(5, 5, 15), BlockId.Stone);

            VoxelRaycaster raycaster = new VoxelRaycaster(world, registry);

            // Bắn từ (5.5, 5.5, 2.5) với tầm với tối đa 5.0f (Quy tắc R7) -> Khoảng cách thực tế là 9.5f > 5.0f
            Vector3 origin = new Vector3(5.5f, 5.5f, 2.5f);
            Vector3 direction = new Vector3(0f, 0f, 1f);

            VoxelRaycastHit hit = raycaster.Raycast(origin, direction, maxDistance: 5.0f);

            Assert.That(hit.IsHit, Is.False);
        }

        [Test]
        public void VoxelRaycaster_InvalidMaxDistance_ThrowsArgumentOutOfRangeException()
        {
            BlockRegistry registry = BlockRegistry.CreateDefault();
            FlatTerrainGenerator flatGen = new FlatTerrainGenerator(surfaceHeight: 1);
            World.World world = new World.World(flatGen);
            VoxelRaycaster raycaster = new VoxelRaycaster(world, registry);

            Assert.Throws<ArgumentOutOfRangeException>(() => raycaster.Raycast(Vector3.Zero, Vector3.UnitX, -1f));
            Assert.Throws<ArgumentOutOfRangeException>(() => raycaster.Raycast(Vector3.Zero, Vector3.UnitX, 0f));
        }
    }
}
