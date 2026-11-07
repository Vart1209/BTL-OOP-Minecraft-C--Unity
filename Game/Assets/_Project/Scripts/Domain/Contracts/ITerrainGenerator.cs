namespace Minecraft.Domain.Contracts
{
    /// <summary>
    /// Hợp đồng chiến lược sinh địa hình (Terrain Generation Strategy).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Design Pattern áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Strategy Pattern (Tiêu chí 9, 10, 11, 12):</b> Đóng gói thuật toán sinh địa hình thành một họ các lớp thực thi <see cref="ITerrainGenerator"/> (ví dụ: <c>PerlinTerrainGenerator</c> cho game thực tế, <c>FlatTerrainGenerator</c> cho kiểm thử tự động và chế độ Sandbox).</item>
    ///   <item><b>Open/Closed Principle &amp; Liskov Substitution Principle (Tiêu chí 14, 16):</b> Lớp <c>World</c> chỉ gọi <see cref="GetBlockAt"/> thông qua interface mà không cần biết bên dưới dùng nhiễu Perlin hay địa hình phẳng; khi vấn đáp có thể mở rộng thêm <c>DesertTerrainGenerator</c> mà không phải sửa đổi code của lớp <c>World</c>.</item>
    /// </list>
    /// </para>
    /// </summary>
    public interface ITerrainGenerator
    {
        /// <summary>
        /// Xác định loại khối tự nhiên tại một tọa độ toàn cục dựa trên hạt giống (seed) của thế giới.
        /// </summary>
        /// <param name="worldPosition">Tọa độ toàn cục (X, Y, Z) của khối cần tính toán.</param>
        /// <param name="seed">Hạt giống ngẫu nhiên của thế giới để đảm bảo cùng một seed luôn sinh ra cùng một địa hình.</param>
        /// <returns>Mã loại khối (<see cref="BlockId"/>) được sinh tại vị trí đó.</returns>
        BlockId GetBlockAt(BlockPos worldPosition, int seed);
    }
}

