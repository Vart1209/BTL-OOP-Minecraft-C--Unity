namespace Minecraft.Domain.Contracts
{
    /// <summary>
    /// Bảng mã định danh duy nhất cho các loại vật phẩm (Item) trong túi đồ (Inventory), Hotbar và công thức chế tạo (Recipe).
    /// <para>
    /// <b>Quy ước phân dải ID giữa 2 phân hệ:</b>
    /// <list type="bullet">
    ///   <item><b>0:</b> <see cref="None"/> (ô trống).</item>
    ///   <item><b>1 .. 99:</b> Các vật phẩm dạng khối (<c>BlockItem</c>) có mã số trùng khớp 1-1 với <see cref="BlockId"/> để chuyển đổi trực tiếp khi đặt/phá khối.</item>
    ///   <item><b>100 .. 199:</b> Nguyên liệu trung gian và công cụ/vũ khí (<c>ToolItem</c> - Quy tắc R4, R6: stack tối đa 1).</item>
    ///   <item><b>200 .. 299:</b> Thức ăn hồi phục thanh đói (<c>FoodItem</c> - Quy tắc R8).</item>
    /// </list>
    /// </para>
    /// </summary>
    public enum ItemId : ushort
    {
        /// <summary>Không có vật phẩm (ô trống trong Inventory).</summary>
        None = 0,

        // --- Dải 1..99: Vật phẩm khối (khớp trực tiếp với BlockId) ---
        Bedrock = 1,
        Stone = 2,
        Dirt = 3,
        Grass = 4,
        Wood = 5,
        Leaves = 6,
        Sand = 7,
        Planks = 8,
        CraftingTable = 9,
        Cobblestone = 10,

        // --- Dải 100..199: Nguyên liệu & Công cụ (ToolItem) ---
        /// <summary>Que gỗ dùng làm cán công cụ.</summary>
        Stick = 100,

        /// <summary>Cúp gỗ giúp đào đá nhanh hơn tay không (Quy tắc R4).</summary>
        WoodenPickaxe = 101,

        /// <summary>Cúp đá có tốc độ đào và độ bền cao hơn cúp gỗ.</summary>
        StonePickaxe = 102,

        /// <summary>Kiếm gỗ dùng chiến đấu với Zombie/Mob (Use Case của B).</summary>
        WoodenSword = 103,

        /// <summary>Rìu gỗ hỗ trợ chặt các khối gỗ nhanh hơn.</summary>
        WoodenAxe = 104,

        // --- Dải 200..299: Thức ăn (FoodItem) ---
        /// <summary>Táo rơi ra từ tán lá cây, ăn để hồi điểm đói.</summary>
        Apple = 200,

        /// <summary>Thịt bò sống thu được từ Mob hiền lành (Cow).</summary>
        RawBeef = 201,

        /// <summary>Thịt bò chín hồi nhiều điểm đói hơn thịt sống.</summary>
        CookedBeef = 202
    }
}

