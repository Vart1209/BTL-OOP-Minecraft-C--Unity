using System;
using NUnit.Framework;
using Minecraft.Application.World;
using Minecraft.Domain.Blocks;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.GameModes;
using Minecraft.Domain.Items;
using Minecraft.Domain.World;

namespace Minecraft.Domain.Tests
{
    [TestFixture]
    public class TieredMiningAndDurabilityApplicationTests
    {
        private BlockRegistry _blockRegistry;
        private ItemRegistry _itemRegistry;
        private World.World _world;
        private BlockInteractionService _service;

        [SetUp]
        public void SetUp()
        {
            _blockRegistry = BlockRegistry.CreateDefault();
            _itemRegistry = ItemRegistry.CreateDefault(_blockRegistry);
            ITerrainGenerator flatGen = new FlatTerrainGenerator(surfaceHeight: 10);
            _world = new World.World(flatGen, seed: 100);
            _service = new BlockInteractionService(_world, _world, _blockRegistry, _itemRegistry);
        }

        [Test]
        public void MiningIronOre_WithHand_BreaksBlockButDropsNothing()
        {
            BlockPos orePos = new BlockPos(0, 5, 0);
            _world.SetBlock(orePos, BlockId.IronOre);

            Inventory inventory = new Inventory();
            inventory.SelectHotbarSlot(0); // Hand is empty
            IGameMode survival = new SurvivalMode();

            bool broken = _service.TryBreakBlock(orePos, inventory, survival, out ItemId droppedItem, out bool toolBroke);

            Assert.That(broken, Is.True);
            Assert.That(_world.GetBlock(orePos), Is.EqualTo(BlockId.Air));
            Assert.That(droppedItem, Is.EqualTo(ItemId.None));
            Assert.That(inventory.CountItem(ItemId.IronOre), Is.EqualTo(0));
            Assert.That(toolBroke, Is.False);
        }

        [Test]
        public void MiningIronOre_WithStonePickaxe_HarvestsOreAndDamagesPickaxe()
        {
            BlockPos orePos = new BlockPos(0, 5, 0);
            _world.SetBlock(orePos, BlockId.IronOre);

            Inventory inventory = new Inventory();
            Item stonePick = _itemRegistry.Get(ItemId.StonePickaxe);
            inventory.AddItem(stonePick, 1);
            inventory.SelectHotbarSlot(0);

            ItemStack held = inventory.GetSelectedSlot();
            int initialDurability = held.Durability;
            Assert.That(initialDurability, Is.EqualTo(131));

            IGameMode survival = new SurvivalMode();
            bool broken = _service.TryBreakBlock(orePos, inventory, survival, out ItemId droppedItem, out bool toolBroke);

            Assert.That(broken, Is.True);
            Assert.That(_world.GetBlock(orePos), Is.EqualTo(BlockId.Air));
            Assert.That(droppedItem, Is.EqualTo(ItemId.IronOre));
            Assert.That(inventory.CountItem(ItemId.IronOre), Is.EqualTo(1));
            Assert.That(toolBroke, Is.False);

            // Cúp đá bị trừ 1 độ bền
            ItemStack afterTool = inventory.GetSlot(0);
            Assert.That(afterTool.Durability, Is.EqualTo(130));
        }

        [Test]
        public void ToolBreaks_WhenDurabilityReachesZero_ClearsSlot()
        {
            BlockPos stonePos = new BlockPos(0, 5, 0);
            _world.SetBlock(stonePos, BlockId.Stone);

            Inventory inventory = new Inventory();
            ToolItem woodPick = (ToolItem)_itemRegistry.Get(ItemId.WoodenPickaxe);
            // Tạo cúp gỗ chỉ còn đúng 1 độ bền
            ItemStack nearBrokenPick = new ItemStack(woodPick, 1, currentDurability: 1);
            inventory.SetSlot(0, nearBrokenPick);
            inventory.SelectHotbarSlot(0);

            IGameMode survival = new SurvivalMode();
            bool broken = _service.TryBreakBlock(stonePos, inventory, survival, out ItemId droppedItem, out bool toolBroke);

            Assert.That(broken, Is.True);
            Assert.That(droppedItem, Is.EqualTo(ItemId.Cobblestone));
            Assert.That(toolBroke, Is.True);
            Assert.That(inventory.GetSlot(0).IsEmpty, Is.True);
        }
    }
}
