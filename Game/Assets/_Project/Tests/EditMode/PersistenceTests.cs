using System.Collections.Generic;
using System.IO;
using System.Numerics;
using NUnit.Framework;
using Minecraft.Application.World;
using Minecraft.Domain.Blocks;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.Entities;
using Minecraft.Domain.GameModes;
using Minecraft.Domain.Items;
using Minecraft.Domain.Persistence;
using Minecraft.Domain.World;
using Minecraft.Infrastructure.Persistence;

namespace Minecraft.Domain.Tests
{
    [TestFixture]
    public class PersistenceTests
    {
        private string _tempDir;
        private ItemRegistry _itemRegistry;
        private BlockRegistry _blockRegistry;

        [SetUp]
        public void SetUp()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "MinecraftPersistenceTests_" + Path.GetRandomFileName());
            Directory.CreateDirectory(_tempDir);

            _blockRegistry = BlockRegistry.CreateDefault();
            _itemRegistry = ItemRegistry.CreateDefault(_blockRegistry);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_tempDir))
            {
                Directory.Delete(_tempDir, true);
            }
        }

        [Test]
        public void JsonWorldRepository_SaveAndLoad_PreservesDtoIntegrity()
        {
            string savePath = Path.Combine(_tempDir, "world_save.json");
            IWorldRepository repo = new JsonWorldRepository();

            var saveDto = new WorldSaveDto
            {
                WorldSeed = 20261107,
                SaveName = "SurvivalWorld_01",
                Player = new PlayerSaveDto
                {
                    PositionX = 10.5f,
                    PositionY = 20.0f,
                    PositionZ = -5.2f,
                    CurrentHealth = 15,
                    CurrentHunger = 18,
                    GameMode = "Survival",
                    InventorySlots = new List<InventorySlotDto>
                    {
                        new InventorySlotDto { SlotIndex = 0, ItemId = "Wood", Count = 16 },
                        new InventorySlotDto { SlotIndex = 1, ItemId = "WoodenPickaxe", Count = 1 }
                    }
                },
                ModifiedBlocks = new List<BlockChangeDto>
                {
                    new BlockChangeDto { X = 0, Y = 1, Z = 0, BlockId = "Stone" },
                    new BlockChangeDto { X = 5, Y = 2, Z = 5, BlockId = "CraftingTable" }
                }
            };

            bool saveSuccess = repo.SaveWorld(savePath, saveDto);
            Assert.IsTrue(saveSuccess);
            Assert.IsTrue(File.Exists(savePath));

            bool loadSuccess = repo.TryLoadWorld(savePath, out WorldSaveDto loadedDto);
            Assert.IsTrue(loadSuccess);
            Assert.IsNotNull(loadedDto);
            Assert.AreEqual(20261107, loadedDto.WorldSeed);
            Assert.AreEqual("SurvivalWorld_01", loadedDto.SaveName);
            Assert.AreEqual(10.5f, loadedDto.Player.PositionX);
            Assert.AreEqual(15, loadedDto.Player.CurrentHealth);
            Assert.AreEqual(18, loadedDto.Player.CurrentHunger);
            Assert.AreEqual("Survival", loadedDto.Player.GameMode);
            Assert.AreEqual(2, loadedDto.Player.InventorySlots.Count);
            Assert.AreEqual(2, loadedDto.ModifiedBlocks.Count);
        }

        [Test]
        public void JsonWorldRepository_CorruptedFile_ReturnsFalseGracefullyWithoutCrashing()
        {
            string savePath = Path.Combine(_tempDir, "corrupted_save.json");
            File.WriteAllText(savePath, "INVALID_JSON_DATA_%%%");

            IWorldRepository repo = new JsonWorldRepository();
            bool loadSuccess = repo.TryLoadWorld(savePath, out WorldSaveDto loadedDto);

            // Bọc try-catch Tiêu chí 19: Trả về false thay vì để crash
            Assert.IsFalse(loadSuccess);
            Assert.IsNull(loadedDto);
            Assert.IsNotNull(repo.LastError);
        }

        [Test]
        public void WorldPersistenceService_SaveAndLoad_FullyRestoresWorldAndPlayerState()
        {
            string savePath = Path.Combine(_tempDir, "game_save.json");
            IWorldRepository repo = new JsonWorldRepository();
            var service = new WorldPersistenceService(repo, _itemRegistry);

            var generator = new FlatTerrainGenerator(surfaceHeight: 1);
            var originalWorld = new World.World(generator, seed: 12345);
            var originalPlayer = new Player(new Vector3(3f, 2f, 3f), new SurvivalMode(), maxHealth: 20, maxHunger: 20);

            // Sinh chunk và thay đổi block
            originalWorld.GenerateChunk(new ChunkCoord(0, 0));
            var modifiedPos1 = new BlockPos(0, 2, 0);
            var modifiedPos2 = new BlockPos(1, 2, 1);
            originalWorld.SetBlock(modifiedPos1, BlockId.Stone);
            originalWorld.SetBlock(modifiedPos2, BlockId.CraftingTable);

            // Điều chỉnh trạng thái player
            originalPlayer.Health.TakeDamage(6); // còn 14 máu
            originalPlayer.Hunger.ConsumeHunger(5); // còn 15 đói
            originalPlayer.Inventory.AddItem(_itemRegistry.GetItem(ItemId.Wood), 8);

            var modifiedBlocks = new Dictionary<BlockPos, BlockId>
            {
                { modifiedPos1, BlockId.Stone },
                { modifiedPos2, BlockId.CraftingTable }
            };

            // Use Case UC05: Lưu thế giới
            bool saveResult = service.SaveGame(savePath, 12345, "MyTestWorld", originalWorld, originalPlayer, modifiedBlocks);
            Assert.IsTrue(saveResult);

            // Tạo thế giới mới và người chơi mới để khôi phục
            var newWorld = new World.World(generator, seed: 12345);
            newWorld.GenerateChunk(new ChunkCoord(0, 0));
            var newPlayer = new Player(Vector3.Zero, new CreativeMode(), maxHealth: 20, maxHunger: 20);

            // Use Case UC06: Tải thế giới
            bool loadResult = service.LoadGame(savePath, newWorld, newPlayer, out Dictionary<BlockPos, BlockId> restoredModifiedBlocks, out IGameMode loadedMode);

            Assert.IsTrue(loadResult);
            Assert.AreEqual("Survival", loadedMode.ModeName);
            Assert.AreEqual(new Vector3(3f, 2f, 3f), newPlayer.Position);
            Assert.AreEqual(14, newPlayer.Health.CurrentHealth);
            Assert.AreEqual(15, newPlayer.Hunger.CurrentHunger);
            Assert.AreEqual(8, newPlayer.Inventory.GetSlot(0).Count);
            Assert.AreEqual(ItemId.Wood, newPlayer.Inventory.GetSlot(0).Item.Id);

            // Kiểm tra các block bị sửa đổi đã được đặt lại đúng
            Assert.AreEqual(BlockId.Stone, newWorld.GetBlock(modifiedPos1));
            Assert.AreEqual(BlockId.CraftingTable, newWorld.GetBlock(modifiedPos2));
            Assert.AreEqual(2, restoredModifiedBlocks.Count);
        }
    }
}
