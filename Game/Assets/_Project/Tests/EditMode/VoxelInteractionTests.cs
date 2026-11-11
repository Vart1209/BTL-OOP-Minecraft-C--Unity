using System;
using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using Minecraft.Application.World;
using Minecraft.Domain.Blocks;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.Items;
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

        private sealed class TestGameMode : IGameMode
        {
            public string ModeName { get; set; } = "Survival";
            public bool CanTakeDamage { get; set; } = true;
            public bool ConsumesItemsOnPlace { get; set; } = true;
            public bool InstantBreak { get; set; } = false;
            public bool CanFly { get; set; } = false;

            public bool CanBreakBlock(BlockId blockId)
            {
                if (ModeName == "Survival") return blockId != BlockId.Bedrock && blockId != BlockId.Air;
                return blockId != BlockId.Air;
            }
        }

        [Test]
        public void BlockInteractionService_TryBreakBlock_EnforcesRuleR1_R5_R12()
        {
            BlockRegistry blockRegistry = BlockRegistry.CreateDefault();
            ItemRegistry itemRegistry = ItemRegistry.CreateDefault(blockRegistry);
            FlatTerrainGenerator flatGen = new FlatTerrainGenerator(surfaceHeight: 1); // Y=0 Bedrock, Y>0 Air
            World.World world = new World.World(flatGen);
            world.GenerateChunk(new ChunkCoord(0, 0));

            BlockInteractionService service = new BlockInteractionService(world, world, blockRegistry, itemRegistry);
            Inventory inventory = new Inventory();
            TestGameMode survivalMode = new TestGameMode { ModeName = "Survival", ConsumesItemsOnPlace = true };
            TestGameMode creativeMode = new TestGameMode { ModeName = "Creative", ConsumesItemsOnPlace = false, InstantBreak = true };

            // 1. Quy tắc R1: Survival không thể phá Bedrock (tại 0, 0, 0)
            bool breakBedrockSurvival = service.TryBreakBlock(new BlockPos(0, 0, 0), inventory, survivalMode);
            Assert.That(breakBedrockSurvival, Is.False);
            Assert.That(world.GetBlock(new BlockPos(0, 0, 0)), Is.EqualTo(BlockId.Bedrock));

            // 2. Quy tắc R12: Creative phá được Bedrock
            bool breakBedrockCreative = service.TryBreakBlock(new BlockPos(0, 0, 0), inventory, creativeMode);
            Assert.That(breakBedrockCreative, Is.True);
            Assert.That(world.GetBlock(new BlockPos(0, 0, 0)), Is.EqualTo(BlockId.Air));

            // 3. Quy tắc R5: Phá Stone rơi Cobblestone vào Inventory ở Survival (cần có cúp gỗ trở lên theo phân cấp khai thác)
            world.SetBlock(new BlockPos(5, 5, 5), BlockId.Stone);
            inventory.AddItem(itemRegistry.Get(ItemId.WoodenPickaxe), 1);
            bool breakStone = service.TryBreakBlock(new BlockPos(5, 5, 5), inventory, survivalMode);
            Assert.That(breakStone, Is.True);
            Assert.That(world.GetBlock(new BlockPos(5, 5, 5)), Is.EqualTo(BlockId.Air));
            Assert.That(inventory.CountItem(ItemId.Cobblestone), Is.EqualTo(1));
        }

        [Test]
        public void BlockInteractionService_TryPlaceBlock_EnforcesRuleR2_R12()
        {
            BlockRegistry blockRegistry = BlockRegistry.CreateDefault();
            ItemRegistry itemRegistry = ItemRegistry.CreateDefault(blockRegistry);
            FlatTerrainGenerator flatGen = new FlatTerrainGenerator(surfaceHeight: 1);
            World.World world = new World.World(flatGen);
            world.GenerateChunk(new ChunkCoord(0, 0));

            BlockInteractionService service = new BlockInteractionService(world, world, blockRegistry, itemRegistry);
            Inventory inventory = new Inventory();
            TestGameMode survivalMode = new TestGameMode { ModeName = "Survival", ConsumesItemsOnPlace = true };
            TestGameMode creativeMode = new TestGameMode { ModeName = "Creative", ConsumesItemsOnPlace = false };

            Item woodItem = itemRegistry.Get(ItemId.Wood);
            inventory.AddItem(woodItem, 2);
            inventory.SelectHotbarSlot(0); // Đang cầm Wood

            BlockPos playerFeet = new BlockPos(3, 1, 3);
            BlockPos playerHead = new BlockPos(3, 2, 3);

            // Quy tắc R2: Không cho phép đặt đè lên chân hoặc đầu người chơi
            bool placeOnFeet = service.TryPlaceBlock(playerFeet, playerFeet, playerHead, inventory, survivalMode);
            Assert.That(placeOnFeet, Is.False);
            bool placeOnHead = service.TryPlaceBlock(playerHead, playerFeet, playerHead, inventory, survivalMode);
            Assert.That(placeOnHead, Is.False);

            // Đặt vào vị trí hợp lệ ở Survival -> thành công và trừ 1 Wood
            BlockPos validPos = new BlockPos(4, 2, 3); // Y=2 là Air (vì surfaceHeight=1)
            bool placeSuccessSurvival = service.TryPlaceBlock(validPos, playerFeet, playerHead, inventory, survivalMode);
            Assert.That(placeSuccessSurvival, Is.True);
            Assert.That(world.GetBlock(validPos), Is.EqualTo(BlockId.Wood));
            Assert.That(inventory.CountItem(ItemId.Wood), Is.EqualTo(1)); // 2 - 1 = 1

            // Đặt ở Creative -> thành công và KHÔNG trừ item
            BlockPos validPos2 = new BlockPos(4, 3, 3);
            bool placeSuccessCreative = service.TryPlaceBlock(validPos2, playerFeet, playerHead, inventory, creativeMode);
            Assert.That(placeSuccessCreative, Is.True);
            Assert.That(world.GetBlock(validPos2), Is.EqualTo(BlockId.Wood));
            Assert.That(inventory.CountItem(ItemId.Wood), Is.EqualTo(1)); // Giữ nguyên 1
        }

        [Test]
        public void ChunkLoadService_UpdatesActiveChunksCorrectly()
        {
            FlatTerrainGenerator flatGen = new FlatTerrainGenerator(surfaceHeight: 1);
            World.World world = new World.World(flatGen);
            ChunkLoadService chunkService = new ChunkLoadService(world, renderDistanceInChunks: 1);

            // Ban đầu chưa có chunk nào
            Assert.That(chunkService.LoadedChunks.Count, Is.EqualTo(0));

            // Cập nhật tại (0, 0) với bán kính 1 -> tải khối hộp 3x3 = 9 chunk
            ChunkLoadResult result1 = chunkService.UpdateActiveChunks(new ChunkCoord(0, 0));
            Assert.That(result1.NewlyLoaded.Count, Is.EqualTo(9));
            Assert.That(result1.Unloaded.Count, Is.EqualTo(0));
            Assert.That(chunkService.LoadedChunks.Count, Is.EqualTo(9));

            // Người chơi đứng yên gọi lại -> không sinh mới và không gỡ chunk
            ChunkLoadResult resultSame = chunkService.UpdateActiveChunks(new ChunkCoord(0, 0));
            Assert.That(resultSame.NewlyLoaded.Count, Is.EqualTo(0));
            Assert.That(resultSame.Unloaded.Count, Is.EqualTo(0));

            // Người chơi di chuyển rất xa tới (10, 10) -> gỡ 9 chunk cũ, nạp 9 chunk mới
            ChunkLoadResult resultFar = chunkService.UpdateActiveChunks(new ChunkCoord(10, 10));
            Assert.That(resultFar.NewlyLoaded.Count, Is.EqualTo(9));
            Assert.That(resultFar.Unloaded.Count, Is.EqualTo(9));
            Assert.That(chunkService.LoadedChunks.Count, Is.EqualTo(9));
        }
    }
}
