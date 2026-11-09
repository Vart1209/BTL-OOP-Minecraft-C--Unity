using System;
using System.Collections.Generic;
using NUnit.Framework;
using Minecraft.Domain.Blocks;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.Items;
using Minecraft.Domain.World;

namespace Minecraft.Domain.Tests
{
    [TestFixture]
    public class InventoryAndItemsTests
    {
        private sealed class FakeGameMode : IGameMode
        {
            public string ModeName { get; set; } = "Survival";
            public bool CanTakeDamage { get; set; } = true;
            public bool ConsumesItemsOnPlace { get; set; } = true;
            public bool InstantBreak { get; set; } = false;
            public bool CanFly { get; set; } = false;

            public bool CanBreakBlock(BlockId blockId) => blockId != BlockId.Bedrock && blockId != BlockId.Air;
        }

        [Test]
        public void BlockItem_RuleR6AndRuleR2_MaxStack64AndPlacesBlockOnAir()
        {
            BlockRegistry blockRegistry = BlockRegistry.CreateDefault();
            ItemRegistry itemRegistry = ItemRegistry.CreateDefault(blockRegistry);
            Item grassItem = itemRegistry.Get(ItemId.Grass);

            // Quy tắc R6: BlockItem có MaxStackSize = 64
            Assert.That(grassItem.MaxStackSize, Is.EqualTo(64));
            Assert.That(grassItem, Is.InstanceOf<BlockItem>());

            ITerrainGenerator emptyGen = new FlatTerrainGenerator(surfaceHeight: 1);
            World.World world = new World.World(emptyGen);
            world.GenerateChunk(new ChunkCoord(0, 0));

            FakeGameMode survivalMode = new FakeGameMode();

            // Đặt vào vị trí hợp lệ đang là Air (5, 5, 5)
            ItemUseContext context = new ItemUseContext(
                targetBlockPos: new BlockPos(5, 4, 5),
                adjacentPlacePos: new BlockPos(5, 5, 5),
                worldReader: world,
                worldWriter: world,
                gameMode: survivalMode
            );

            bool placeSuccess = grassItem.Use(context);
            Assert.That(placeSuccess, Is.True);
            Assert.That(world.GetBlock(new BlockPos(5, 5, 5)), Is.EqualTo(BlockId.Grass));

            // Quy tắc R2: Không đặt block vào ô đã có block
            bool placeDuplicate = grassItem.Use(context);
            Assert.That(placeDuplicate, Is.False);
        }

        [Test]
        public void ToolItem_RuleR6AndRuleR4_ToolStackIs1AndCalculatesMiningDuration()
        {
            BlockRegistry blockRegistry = BlockRegistry.CreateDefault();
            ItemRegistry itemRegistry = ItemRegistry.CreateDefault(blockRegistry);

            ToolItem pickaxe = (ToolItem)itemRegistry.Get(ItemId.WoodenPickaxe);
            // Quy tắc R6: Công cụ có MaxStackSize = 1
            Assert.That(pickaxe.MaxStackSize, Is.EqualTo(1));
            Assert.That(pickaxe.ToolCategory, Is.EqualTo(ToolType.Pickaxe));
            Assert.That(pickaxe.MiningSpeedMultiplier, Is.GreaterThan(1.0f));

            BlockType stone = blockRegistry.Get(BlockId.Stone);
            BlockType bedrock = blockRegistry.Get(BlockId.Bedrock);
            FakeGameMode survival = new FakeGameMode();
            FakeGameMode creative = new FakeGameMode { InstantBreak = true };

            // Khối đá: có cúp đào nhanh hơn tay không (Hardness / SpeedMultiplier)
            float stoneDurationWithTool = pickaxe.GetMiningDuration(stone, survival);
            float stoneDurationHand = stone.Hardness;
            Assert.That(stoneDurationWithTool, Is.LessThan(stoneDurationHand));

            // Bedrock trong Survival không thể đào (vô cực)
            float bedrockDuration = pickaxe.GetMiningDuration(bedrock, survival);
            Assert.That(float.IsPositiveInfinity(bedrockDuration), Is.True);

            // Creative đào tức thì (0s)
            float creativeDuration = pickaxe.GetMiningDuration(stone, creative);
            Assert.That(creativeDuration, Is.EqualTo(0f));
        }

        [Test]
        public void FoodItem_RuleR6AndRestoreAttributes_MaxStack64AndRestoresHunger()
        {
            BlockRegistry blockRegistry = BlockRegistry.CreateDefault();
            ItemRegistry itemRegistry = ItemRegistry.CreateDefault(blockRegistry);

            FoodItem apple = (FoodItem)itemRegistry.Get(ItemId.Apple);
            Assert.That(apple.MaxStackSize, Is.EqualTo(64));
            Assert.That(apple.HungerRestore, Is.EqualTo(4));
            Assert.That(apple.HealthRestore, Is.EqualTo(0));

            FoodItem cookedBeef = (FoodItem)itemRegistry.Get(ItemId.CookedBeef);
            Assert.That(cookedBeef.HungerRestore, Is.EqualTo(8));
            Assert.That(cookedBeef.HealthRestore, Is.EqualTo(2));
        }

        [Test]
        public void ItemStack_AggregationAndOverflowHandling_ReturnsRemainderCorrectly()
        {
            BlockRegistry blockRegistry = BlockRegistry.CreateDefault();
            ItemRegistry itemRegistry = ItemRegistry.CreateDefault(blockRegistry);
            Item dirt = itemRegistry.Get(ItemId.Dirt);

            ItemStack stack = new ItemStack(dirt, 60);
            Assert.That(stack.Count, Is.EqualTo(60));
            Assert.That(stack.Definition, Is.SameAs(dirt));

            // Thêm 10 vào stack hiện có 60 (MaxStack = 64) -> thêm được 4, dư 6
            int remainder = stack.Add(10);
            Assert.That(stack.Count, Is.EqualTo(64));
            Assert.That(remainder, Is.EqualTo(6));

            // Trừ 20 khỏi stack -> còn 44
            int removed = stack.Remove(20);
            Assert.That(removed, Is.EqualTo(20));
            Assert.That(stack.Count, Is.EqualTo(44));
        }

        [Test]
        public void Inventory_AddItemAndAutoSplit_FillsExistingStackThenEmptySlots()
        {
            BlockRegistry blockRegistry = BlockRegistry.CreateDefault();
            ItemRegistry itemRegistry = ItemRegistry.CreateDefault(blockRegistry);
            Item stone = itemRegistry.Get(ItemId.Stone);

            Inventory inventory = new Inventory();
            bool eventFired = false;
            inventory.InventoryChanged += () => eventFired = true;

            // Thêm 70 khối Stone (vượt quá 64) -> ô 0 có 64, ô 1 có 6
            int remainder = inventory.AddItem(stone, 70);
            Assert.That(remainder, Is.EqualTo(0));
            Assert.That(eventFired, Is.True);
            Assert.That(inventory.GetSlot(0).Count, Is.EqualTo(64));
            Assert.That(inventory.GetSlot(1).Count, Is.EqualTo(6));
            Assert.That(inventory.CountItem(ItemId.Stone), Is.EqualTo(70));

            // Thêm tiếp 10 khối Stone -> dồn vào ô 1 (6 + 10 = 16)
            inventory.AddItem(stone, 10);
            Assert.That(inventory.GetSlot(1).Count, Is.EqualTo(16));
            Assert.That(inventory.CountItem(ItemId.Stone), Is.EqualTo(80));
        }

        [Test]
        public void Inventory_HotbarSelectionAndAtomicRemove_SatisfiesRuleR10()
        {
            BlockRegistry blockRegistry = BlockRegistry.CreateDefault();
            ItemRegistry itemRegistry = ItemRegistry.CreateDefault(blockRegistry);
            Item wood = itemRegistry.Get(ItemId.Wood);

            Inventory inventory = new Inventory();
            inventory.AddItem(wood, 5);

            // Chọn ô Hotbar
            inventory.SelectHotbarSlot(0);
            Assert.That(inventory.SelectedHotbarIndex, Is.EqualTo(0));
            Assert.That(inventory.GetSelectedSlot().Definition.Id, Is.EqualTo(ItemId.Wood));

            // Chọn ô ngoài [0..8] bị từ chối
            Assert.Throws<ArgumentOutOfRangeException>(() => inventory.SelectHotbarSlot(9));
            Assert.Throws<ArgumentOutOfRangeException>(() => inventory.SelectHotbarSlot(-1));

            // Quy tắc R10: "đủ hết hoặc không làm gì"
            // Muốn trừ 10 nhưng chỉ có 5 -> trả về false và không trừ cái nào
            bool removeFail = inventory.RemoveItem(ItemId.Wood, 10);
            Assert.That(removeFail, Is.False);
            Assert.That(inventory.CountItem(ItemId.Wood), Is.EqualTo(5));

            // Trừ 3 hợp lệ -> còn 2
            bool removeSuccess = inventory.RemoveItem(ItemId.Wood, 3);
            Assert.That(removeSuccess, Is.True);
            Assert.That(inventory.CountItem(ItemId.Wood), Is.EqualTo(2));
        }
    }
}
