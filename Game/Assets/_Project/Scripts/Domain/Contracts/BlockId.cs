namespace Minecraft.Domain.Contracts
{
    /// <summary>
    /// Bảng mã định danh duy nhất cho các loại khối (Block) trong thế giới Voxel.
    /// <para>
    /// <b>Hợp đồng giữa 2 phân hệ (Tuần T0):</b>
    /// <list type="bullet">
    ///   <item>Sử dụng kiểu cơ sở <c>byte</c> (0..255) để mảng 3 chiều trong mỗi <c>Chunk</c> (16×16×64 = 16.384 ô) chỉ tốn đúng 16 KB bộ nhớ RAM.</item>
    ///   <item><see cref="Air"/> luôn có giá trị <c>0</c> để khi cấp phát mảng <c>new BlockId[...]</c> trong C#, toàn bộ Chunk mặc định là không khí.</item>
    ///   <item>Thành viên A (World) và Thành viên B (Survival) dùng chung bảng mã này khi đặt/phá block và lưu/tải file JSON.</item>
    /// </list>
    /// </para>
    /// </summary>
    public enum BlockId : byte
    {
        /// <summary>Không khí (ô trống, đi xuyên qua được, không sinh lưới Mesh).</summary>
        Air = 0,

        /// <summary>Đá nền ở đáy thế giới (Y = 0), không thể bị phá hủy ở chế độ Survival (Quy tắc R1).</summary>
        Bedrock = 1,

        /// <summary>Đá tự nhiên dưới lòng đất, khi phá rơi ra Cobblestone.</summary>
        Stone = 2,

        /// <summary>Đất thường nằm ngay dưới lớp cỏ.</summary>
        Dirt = 3,

        /// <summary>Khối đất phủ cỏ trên bề mặt địa hình.</summary>
        Grass = 4,

        /// <summary>Thân cây gỗ tự nhiên, nguyên liệu cơ bản để chế tạo ván gỗ.</summary>
        Wood = 5,

        /// <summary>Tán lá cây trên đỉnh thân gỗ.</summary>
        Leaves = 6,

        /// <summary>Khối cát ở vùng thấp hoặc ven bờ.</summary>
        Sand = 7,

        /// <summary>Ván gỗ đã qua chế tạo từ thân gỗ.</summary>
        Planks = 8,

        /// <summary>Bàn chế tạo (Crafting Table) hỗ trợ mở rộng công thức chế tạo.</summary>
        CraftingTable = 9,

        /// <summary>Đá cuội thu được sau khi khai thác khối Stone.</summary>
        Cobblestone = 10,

        /// <summary>Quặng than phân bố phổ biến trong lòng đất.</summary>
        CoalOre = 11,

        /// <summary>Quặng sắt phân bố ở tầng đá trung bình.</summary>
        IronOre = 12,

        /// <summary>Quặng vàng phân bố ở tầng đá sâu.</summary>
        GoldOre = 13,

        /// <summary>Quặng kim cương quý hiếm ở sát tầng đá nền.</summary>
        DiamondOre = 14,

        /// <summary>Khối gạch nung xây dựng công trình.</summary>
        Bricks = 15,

        /// <summary>Khối kính trong suốt cho phép ánh sáng xuyên qua.</summary>
        Glass = 16,

        /// <summary>Tủ sách trang trí nội thất (mặt trên/dưới là ván gỗ, mặt bên là kệ sách).</summary>
        Bookshelf = 17,

        /// <summary>Đá phát sáng (Glowstone) trang trí ban đêm.</summary>
        Glowstone = 18,

        /// <summary>Khối tuyết trắng phủ trên các đỉnh núi cao.</summary>
        Snow = 19,

        /// <summary>Khối mặt nước ở các vùng hồ trũng thấp.</summary>
        Water = 20
    }
}

