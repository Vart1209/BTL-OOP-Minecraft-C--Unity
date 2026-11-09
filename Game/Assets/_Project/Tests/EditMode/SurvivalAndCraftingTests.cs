using System;
using System.Numerics;
using NUnit.Framework;
using Minecraft.Application.Survival;
using Minecraft.Domain.Blocks;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.Crafting;
using Minecraft.Domain.Entities;
using Minecraft.Domain.Items;

namespace Minecraft.Domain.Tests
{
    [TestFixture]
    public class SurvivalAndCraftingTests
    {
        private sealed class TestGameMode : IGameMode
        {
            public string ModeName { get; set; } = "Survival";
            public bool CanTakeDamage { get; set; } = true;
            public bool ConsumesItemsOnPlace { get; set; } = true;
            public bool InstantBreak { get; set; } = false;
            public bool CanFly { get; set; } = false;

            public bool CanBreakBlock(BlockId blockId) => blockId != BlockId.Bedrock && blockId != BlockId.Air;
        }

        [Test]
        public void Health_TakeDamageAndDie_EnforcesRuleR9_R12()
        {
            Health health = new Health(maxHealth: 20);
            TestGameMode survival = new TestGameMode { CanTakeDamage = true };
            TestGameMode creative = new TestGameMode { CanTakeDamage = false };

            bool diedEventFired = false;
            int lastCurrent = -1;
            health.HealthChanged += (curr, max) => lastCurrent = curr;
            health.Died += () => diedEventFired = true;

            // 1. Creative miễn trừ sát thương (Quy tắc R12)
            health.TakeDamage(10, creative);
            Assert.That(health.Current, Is.EqualTo(20));

            // 2. Survival nhận sát thương
            health.TakeDamage(5, survival);
            Assert.That(health.Current, Is.EqualTo(15));
            Assert.That(lastCurrent, Is.EqualTo(15));
            Assert.That(diedEventFired, Is.False);

            // 3. Máu về 0 bắn sự kiện Died (Quy tắc R9)
            health.TakeDamage(20, survival);
            Assert.That(health.Current, Is.EqualTo(0));
            Assert.That(health.IsAlive, Is.False);
            Assert.That(diedEventFired, Is.True);
        }

        [Test]
        public void Hunger_EatAndStarvationRegeneration_EnforcesRuleR8()
        {
            Health health = new Health(maxHealth: 20);
            health.TakeDamage(5, new TestGameMode { CanTakeDamage = true }); // Máu còn 15
            Hunger hunger = new Hunger(maxHunger: 20);
            TestGameMode survival = new TestGameMode { CanTakeDamage = true };

            // Ăn thức ăn tăng điểm đói (không vượt Max)
            hunger.Reduce(10); // Đói còn 10
            Assert.That(hunger.Current, Is.EqualTo(10));
            hunger.Eat(5);
            Assert.That(hunger.Current, Is.EqualTo(15));

            // Điểm đói >= 18: Tick đủ 4 giây hồi 1 máu (Quy tắc R8)
            hunger.Eat(5); // Đói = 20 >= 18
            hunger.Tick(4.1f, health, survival);
            Assert.That(health.Current, Is.EqualTo(16)); // 15 + 1 = 16

            // Điểm đói == 0: Tick đủ 4 giây mất 1 máu (Quy tắc R8)
            hunger.Reduce(20); // Đói = 0
            Assert.That(hunger.Current, Is.EqualTo(0));
            hunger.Tick(4.1f, health, survival);
            Assert.That(health.Current, Is.EqualTo(15)); // 16 - 1 = 15
        }

        [Test]
        public void Player_CompositionAndRespawn_EnforcesRuleR9()
        {
            Vector3 spawn = new Vector3(0f, 65f, 0f);
            TestGameMode survival = new TestGameMode { CanTakeDamage = true };
            Player player = new Player(spawn, survival);

            // Kiểm tra quan hệ Hợp thành (Composition): Player sở hữu Health, Hunger, Inventory
            Assert.That(player.Health, Is.Not.Null);
            Assert.That(player.Hunger, Is.Not.Null);
            Assert.That(player.Inventory, Is.Not.Null);
            Assert.That(player.Position, Is.EqualTo(spawn));
            Assert.That(player.CurrentGameMode, Is.SameAs(survival));

            // Người chơi di chuyển và nhận sát thương tử vong
            player.MoveTo(new Vector3(100f, 50f, 200f));
            player.Health.TakeDamage(25, survival);
            Assert.That(player.IsAlive, Is.False);

            // Hồi sinh (Respawn theo Quy tắc R9): về vị trí spawn, máu và đói đầy lại
            player.Respawn();
            Assert.That(player.Position, Is.EqualTo(spawn));
            Assert.That(player.Health.Current, Is.EqualTo(player.Health.Max));
            Assert.That(player.Hunger.Current, Is.EqualTo(player.Hunger.Max));
            Assert.That(player.IsAlive, Is.True);
        }

        [Test]
        public void CraftingService_TryCraft_EnforcesRuleR10_AllOrNothing()
        {
            BlockRegistry blockRegistry = BlockRegistry.CreateDefault();
            ItemRegistry itemRegistry = ItemRegistry.CreateDefault(blockRegistry);
            RecipeBook recipeBook = RecipeBook.CreateDefault();
            CraftingService craftingService = new CraftingService(itemRegistry);

            Inventory inventory = new Inventory();

            // Công thức 1: 1 Wood -> 4 Planks
            Recipe woodToPlanks = recipeBook.Recipes[0];
            Assert.That(woodToPlanks.OutputItemId, Is.EqualTo(ItemId.Planks));
            Assert.That(woodToPlanks.OutputCount, Is.EqualTo(4));

            // Thất bại vì chưa có Wood
            Assert.That(craftingService.CanCraft(woodToPlanks, inventory), Is.False);
            Assert.That(craftingService.TryCraft(woodToPlanks, inventory), Is.False);

            // Cho 1 Wood vào inventory -> chế tạo thành công
            Item wood = itemRegistry.Get(ItemId.Wood);
            inventory.AddItem(wood, 1);
            Assert.That(craftingService.CanCraft(woodToPlanks, inventory), Is.True);

            bool craftSuccess = craftingService.TryCraft(woodToPlanks, inventory);
            Assert.That(craftSuccess, Is.True);
            Assert.That(inventory.CountItem(ItemId.Wood), Is.EqualTo(0));
            Assert.That(inventory.CountItem(ItemId.Planks), Is.EqualTo(4));

            // Công thức 3: 3 Planks + 2 Wood -> 1 WoodenPickaxe
            Recipe pickaxeRecipe = recipeBook.Recipes[2];
            // Hiện có 4 Planks nhưng có 0 Wood (thiếu 2 Wood)
            // Quy tắc R10: "đủ hết hoặc không làm gì" -> không được trừ 3 Planks
            Assert.That(craftingService.CanCraft(pickaxeRecipe, inventory), Is.False);
            bool craftPickaxeFail = craftingService.TryCraft(pickaxeRecipe, inventory);
            Assert.That(craftPickaxeFail, Is.False);
            Assert.That(inventory.CountItem(ItemId.Planks), Is.EqualTo(4)); // Vẫn còn nguyên 4 Planks

            // Thêm đủ 2 Wood -> chế tạo thành công
            inventory.AddItem(wood, 2);
            bool craftPickaxeSuccess = craftingService.TryCraft(pickaxeRecipe, inventory);
            Assert.That(craftPickaxeSuccess, Is.True);
            Assert.That(inventory.CountItem(ItemId.Planks), Is.EqualTo(1)); // 4 - 3 = 1
            Assert.That(inventory.CountItem(ItemId.Wood), Is.EqualTo(0));   // 2 - 2 = 0
            Assert.That(inventory.CountItem(ItemId.WoodenPickaxe), Is.EqualTo(1));
        }

        [Test]
        public void SurvivalService_TryEatSelectedFood_RestoresHungerAndConsumesItem()
        {
            BlockRegistry blockRegistry = BlockRegistry.CreateDefault();
            ItemRegistry itemRegistry = ItemRegistry.CreateDefault(blockRegistry);
            SurvivalService survivalService = new SurvivalService();

            Vector3 spawn = Vector3.Zero;
            TestGameMode survival = new TestGameMode { CanTakeDamage = true, ConsumesItemsOnPlace = true };
            Player player = new Player(spawn, survival);

            Item apple = itemRegistry.Get(ItemId.Apple);
            player.Inventory.AddItem(apple, 3);
            player.Inventory.SelectHotbarSlot(0); // Đang cầm Apple

            // Giảm độ đói của người chơi xuống 10 (đói)
            player.Hunger.Reduce(10);
            Assert.That(player.Hunger.Current, Is.EqualTo(10));

            // Ăn 1 quả táo (Apple hồi 4 điểm đói)
            bool eatSuccess = survivalService.TryEatSelectedFood(player);
            Assert.That(eatSuccess, Is.True);
            Assert.That(player.Hunger.Current, Is.EqualTo(14)); // 10 + 4 = 14
            Assert.That(player.Inventory.CountItem(ItemId.Apple), Is.EqualTo(2)); // 3 - 1 = 2
        }
    }
}
