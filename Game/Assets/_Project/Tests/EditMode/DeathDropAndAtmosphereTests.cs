using System;
using System.Numerics;
using NUnit.Framework;
using Minecraft.Application.Survival;
using Minecraft.Domain.Blocks;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.Entities;
using Minecraft.Domain.GameModes;
using Minecraft.Domain.Items;

namespace Minecraft.Tests.EditMode
{
    /// <summary>
    /// Bộ kiểm thử đơn vị cho Phân hệ Rơi vật phẩm khi chết &amp; Thu nhặt (<see cref="DroppedItemEntity"/>, <see cref="DeathDropService"/> — Tuần T4).
    /// Kiểm tra: Cây kế thừa <c>DroppedItemEntity : Entity</c>, tự động hết hạn sau <c>DespawnTimeSeconds</c>,
    /// rơi toàn bộ đồ và xóa túi đồ ở chế độ Survival nhưng giữ nguyên đồ ở Creative, và tự động nhặt lại đồ trong bán kính 2.0f.
    /// </summary>
    [TestFixture]
    public sealed class DeathDropAndAtmosphereTests
    {
        private BlockRegistry _blockRegistry;
        private ItemRegistry _itemRegistry;

        [SetUp]
        public void SetUp()
        {
            _blockRegistry = BlockRegistry.CreateDefault();
            _itemRegistry = ItemRegistry.CreateDefault(_blockRegistry);
        }

        [Test]
        public void DroppedItemEntity_InheritsEntity_ValidatesPayload_AndExpiresAfterDespawnTime()
        {
            Item dirtItem = _itemRegistry.Get(ItemId.Dirt);
            var stack = new ItemStack(dirtItem, 16);
            var dropPos = new Vector3(10f, 5f, -3f);

            Entity entityRef = new DroppedItemEntity(dropPos, stack, despawnTimeSeconds: 10f);
            var dropped = (DroppedItemEntity)entityRef;

            Assert.AreEqual(dropPos, dropped.Position);
            Assert.IsTrue(dropped.IsAlive);
            Assert.IsFalse(dropped.IsExpired);
            Assert.AreEqual(16, dropped.Payload.Count);
            Assert.AreEqual(ItemId.Dirt, dropped.Payload.Definition.Id);

            // Tick 5 giây -> chưa hết hạn
            dropped.Tick(5f);
            Assert.AreEqual(5f, dropped.ElapsedSeconds, 0.001f);
            Assert.IsFalse(dropped.IsExpired);
            Assert.IsTrue(dropped.IsAlive);

            // Tick thêm 5 giây -> đủ 10 giây, tự động hết hạn và đánh dấu không còn sống (IsAlive == false)
            dropped.Tick(5f);
            Assert.IsTrue(dropped.IsExpired);
            Assert.IsFalse(dropped.IsAlive);

            // Kiểm tra từ chối ItemStack rỗng
            Assert.Throws<ArgumentException>(() => new DroppedItemEntity(dropPos, ItemStack.Empty));
        }

        [Test]
        public void DeathDropService_DropInventoryOnDeath_InSurvival_DropsAllItems_ClearsInventory_AndRespawnsPlayer()
        {
            var spawnPoint = new Vector3(0f, 2f, 0f);
            var deathPos = new Vector3(25f, 4f, 18f);
            var player = new Player(spawnPoint, new SurvivalMode());
            player.MoveTo(deathPos);

            player.Inventory.AddItem(_itemRegistry.Get(ItemId.Dirt), 32);
            player.Inventory.AddItem(_itemRegistry.Get(ItemId.Wood), 10);
            player.Inventory.AddItem(_itemRegistry.Get(ItemId.WoodenPickaxe), 1);

            // Giả lập tử vong
            player.Health.TakeDamage(20, player.CurrentGameMode);
            Assert.IsFalse(player.IsAlive);

            var dropService = new DeathDropService();
            int droppedStacks = dropService.DropInventoryOnDeath(player);

            Assert.AreEqual(3, droppedStacks);
            Assert.AreEqual(3, dropService.ActiveDrops.Count);

            // Toàn bộ vật phẩm rơi tại đúng vị trí người chơi chết
            foreach (DroppedItemEntity drop in dropService.ActiveDrops)
            {
                Assert.AreEqual(deathPos, drop.Position);
                Assert.IsTrue(drop.IsAlive);
            }

            // Túi đồ người chơi bị xóa sạch
            Assert.AreEqual(0, player.Inventory.CountItem(ItemId.Dirt));
            Assert.AreEqual(0, player.Inventory.CountItem(ItemId.Wood));
            Assert.AreEqual(0, player.Inventory.CountItem(ItemId.WoodenPickaxe));

            // Người chơi đã được hồi sinh về SpawnPoint với đầy máu
            Assert.AreEqual(spawnPoint, player.Position);
            Assert.IsTrue(player.IsAlive);
            Assert.AreEqual(player.Health.Max, player.Health.Current);
        }

        [Test]
        public void DeathDropService_DropInventoryOnDeath_InCreative_KeepsInventoryIntact_AndRespawnsPlayer()
        {
            var spawnPoint = new Vector3(0f, 2f, 0f);
            var deathPos = new Vector3(15f, 8f, -12f);
            var player = new Player(spawnPoint, new CreativeMode());
            player.MoveTo(deathPos);

            player.Inventory.AddItem(_itemRegistry.Get(ItemId.Stone), 64);
            player.Inventory.AddItem(_itemRegistry.Get(ItemId.Apple), 8);

            var dropService = new DeathDropService();
            int droppedStacks = dropService.DropInventoryOnDeath(player);

            // Creative (ConsumesItemsOnPlace == false) giữ nguyên túi đồ, không rơi ra mặt đất
            Assert.AreEqual(0, droppedStacks);
            Assert.AreEqual(0, dropService.ActiveDrops.Count);
            Assert.AreEqual(64, player.Inventory.CountItem(ItemId.Stone));
            Assert.AreEqual(8, player.Inventory.CountItem(ItemId.Apple));
            Assert.AreEqual(spawnPoint, player.Position);
        }

        [Test]
        public void DeathDropService_CollectNearbyDrops_PicksUpItemsWithinRadius_AndRemovesFromActiveDrops()
        {
            var spawnPoint = new Vector3(0f, 2f, 0f);
            var deathPos = new Vector3(20f, 2f, 20f);
            var player = new Player(spawnPoint, new SurvivalMode());
            player.MoveTo(deathPos);

            player.Inventory.AddItem(_itemRegistry.Get(ItemId.Cobblestone), 24);
            player.Inventory.AddItem(_itemRegistry.Get(ItemId.Apple), 5);

            var dropService = new DeathDropService();
            dropService.DropInventoryOnDeath(player);

            Assert.AreEqual(2, dropService.ActiveDrops.Count);
            Assert.AreEqual(0, player.Inventory.CountItem(ItemId.Cobblestone));

            // Khi người chơi ở SpawnPoint (cách > 28m), chưa nhặt được đồ
            int pickedFar = dropService.CollectNearbyDrops(player, pickupRadius: 2.0f);
            Assert.AreEqual(0, pickedFar);
            Assert.AreEqual(2, dropService.ActiveDrops.Count);

            // Người chơi đi tới gần vị trí rơi đồ trong bán kính 1.5m (<= 2.0f)
            player.MoveTo(new Vector3(19f, 2f, 20f));
            int pickedNear = dropService.CollectNearbyDrops(player, pickupRadius: 2.0f);

            Assert.AreEqual(2, pickedNear);
            Assert.AreEqual(0, dropService.ActiveDrops.Count);
            Assert.AreEqual(24, player.Inventory.CountItem(ItemId.Cobblestone));
            Assert.AreEqual(5, player.Inventory.CountItem(ItemId.Apple));
        }

        [Test]
        public void DeathDropService_Tick_RemovesExpiredDropsAutomatically()
        {
            var dropService = new DeathDropService();
            Item sandItem = _itemRegistry.Get(ItemId.Sand);

            dropService.SpawnDrop(new Vector3(5f, 2f, 5f), new ItemStack(sandItem, 12), despawnTimeSeconds: 10f);
            Assert.AreEqual(1, dropService.ActiveDrops.Count);

            dropService.Tick(4f);
            Assert.AreEqual(1, dropService.ActiveDrops.Count);

            dropService.Tick(7f);
            Assert.AreEqual(0, dropService.ActiveDrops.Count);
        }
    }
}
