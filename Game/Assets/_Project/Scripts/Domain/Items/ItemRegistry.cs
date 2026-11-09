using System;
using System.Collections.Generic;
using Minecraft.Domain.Blocks;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.Items
{
    /// <summary>
    /// Sổ đăng ký và quản lý danh mục toàn bộ các vật phẩm (<see cref="Item"/>) trong trò chơi.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Đóng gói (Encapsulation - Tiêu chí 13):</b> Lưu danh mục trong <c>Dictionary&lt;ItemId, Item&gt;</c> private, bảo vệ dữ liệu cấu hình vật phẩm.</item>
    ///   <item><b>Liskov Substitution Principle (LSP - Tiêu chí 14):</b> Bảng quản lý lưu kiểu cha <see cref="Item"/>, mọi lớp con <see cref="BlockItem"/>, <see cref="ToolItem"/>, <see cref="FoodItem"/> đều hoạt động hoàn hảo khi được truy xuất qua kiểu <see cref="Item"/>.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class ItemRegistry
    {
        private readonly Dictionary<ItemId, Item> _items = new Dictionary<ItemId, Item>();

        /// <summary>
        /// Đăng ký một định nghĩa vật phẩm mới vào hệ thống.
        /// </summary>
        public void Register(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (_items.ContainsKey(item.Id))
            {
                throw new ArgumentException($"Vật phẩm ID '{item.Id}' đã được đăng ký trước đó.", nameof(item));
            }

            _items.Add(item.Id, item);
        }

        /// <summary>
        /// Lấy đối tượng vật phẩm theo mã định danh.
        /// </summary>
        /// <exception cref="KeyNotFoundException">Ném ra nếu ID chưa được đăng ký.</exception>
        public Item Get(ItemId id)
        {
            if (_items.TryGetValue(id, out Item item))
            {
                return item;
            }

            throw new KeyNotFoundException($"Không tìm thấy vật phẩm với mã '{id}'.");
        }

        /// <summary>Bí danh lấy đối tượng vật phẩm theo mã định danh.</summary>
        public Item GetItem(ItemId id) => Get(id);

        /// <summary>
        /// Thử lấy vật phẩm theo mã định danh.
        /// </summary>
        public bool TryGet(ItemId id, out Item item)
        {
            return _items.TryGetValue(id, out item);
        }

        /// <summary>
        /// Kiểm tra vật phẩm đã tồn tại trong Registry hay chưa.
        /// </summary>
        public bool Contains(ItemId id)
        {
            return _items.ContainsKey(id);
        }

        /// <summary>
        /// Tạo Registry mặc định đăng ký đầy đủ các vật phẩm khối, công cụ và thức ăn cốt lõi.
        /// </summary>
        /// <param name="blockRegistry">Danh mục khối dùng để đối sánh thuộc tính khối tương ứng.</param>
        public static ItemRegistry CreateDefault(BlockRegistry blockRegistry)
        {
            if (blockRegistry == null)
            {
                throw new ArgumentNullException(nameof(blockRegistry));
            }

            ItemRegistry registry = new ItemRegistry();

            // 1. Đăng ký các vật phẩm khối (BlockItem: dải ID 1..99 trùng khớp BlockId)
            registry.Register(new BlockItem(ItemId.Bedrock, "Bedrock", BlockId.Bedrock));
            registry.Register(new BlockItem(ItemId.Stone, "Stone", BlockId.Stone));
            registry.Register(new BlockItem(ItemId.Dirt, "Dirt", BlockId.Dirt));
            registry.Register(new BlockItem(ItemId.Grass, "Grass Block", BlockId.Grass));
            registry.Register(new BlockItem(ItemId.Wood, "Wood Log", BlockId.Wood));
            registry.Register(new BlockItem(ItemId.Leaves, "Leaves", BlockId.Leaves));
            registry.Register(new BlockItem(ItemId.Sand, "Sand", BlockId.Sand));
            registry.Register(new BlockItem(ItemId.Planks, "Wood Planks", BlockId.Planks));
            registry.Register(new BlockItem(ItemId.CraftingTable, "Crafting Table", BlockId.CraftingTable));
            registry.Register(new BlockItem(ItemId.Cobblestone, "Cobblestone", BlockId.Cobblestone));

            // 2. Đăng ký nguyên liệu và công cụ (ToolItem: dải ID 100..199)
            // Gậy gỗ (Stick - nguyên liệu thường, MaxStackSize = 64)
            registry.Register(new BlockItem(ItemId.Stick, "Stick", BlockId.Air)); // Stick dùng cơ chế stack 64

            // Công cụ chuyên dụng (MaxStackSize = 1)
            registry.Register(new ToolItem(ItemId.WoodenPickaxe, "Wooden Pickaxe", ToolType.Pickaxe, miningSpeedMultiplier: 2.0f, attackDamage: 2));
            registry.Register(new ToolItem(ItemId.StonePickaxe, "Stone Pickaxe", ToolType.Pickaxe, miningSpeedMultiplier: 4.0f, attackDamage: 3));
            registry.Register(new ToolItem(ItemId.WoodenSword, "Wooden Sword", ToolType.Sword, miningSpeedMultiplier: 1.0f, attackDamage: 4));
            registry.Register(new ToolItem(ItemId.WoodenAxe, "Wooden Axe", ToolType.Axe, miningSpeedMultiplier: 2.0f, attackDamage: 3));

            // 3. Đăng ký thức ăn (FoodItem: dải ID 200..299, MaxStackSize = 64)
            registry.Register(new FoodItem(ItemId.Apple, "Apple", hungerRestore: 4, healthRestore: 0));
            registry.Register(new FoodItem(ItemId.RawBeef, "Raw Beef", hungerRestore: 3, healthRestore: 0));
            registry.Register(new FoodItem(ItemId.CookedBeef, "Cooked Beef", hungerRestore: 8, healthRestore: 2));

            return registry;
        }
    }
}

