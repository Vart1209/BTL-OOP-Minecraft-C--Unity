namespace Minecraft.Domain.Contracts
{
    /// <summary>
    /// Hợp đồng giao diện ghi/cập nhật (Write Interface) cho phép thay đổi khối trong thế giới Voxel.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Interface Segregation Principle (ISP - Tiêu chí 15):</b> Chỉ những dịch vụ điều phối Use Case có quyền thay đổi thế giới (như <c>BlockInteractionService</c>, <c>SaveGameService</c>) mới được tiêm (inject) <see cref="IWorldWriter"/>.</item>
    ///   <item><b>Đóng gói quy tắc nghiệp vụ (Tiêu chí 3, 13):</b> Hàm <see cref="SetBlock"/> trả về <c>bool</c> để từ chối các thao tác vi phạm biên độ cao (Quy tắc R3: <c>Y &lt; 0</c> hoặc <c>Y &gt;= MaxHeight</c>) hoặc khi khối mới trùng hệt khối hiện có.</item>
    /// </list>
    /// </para>
    /// </summary>
    public interface IWorldWriter
    {
        /// <summary>
        /// Đặt hoặc thay thế một khối tại tọa độ toàn cục chỉ định.
        /// Nếu thay đổi thành công, đối tượng thực thi phải phát sự kiện <see cref="IWorldReader.BlockChanged"/>.
        /// </summary>
        /// <param name="position">Tọa độ toàn cục nơi cần đặt hoặc xóa khối (đặt <see cref="BlockId.Air"/> để xóa khối).</param>
        /// <param name="blockId">Mã định danh khối mới cần gán.</param>
        /// <returns><c>true</c> nếu khối được cập nhật thành công; <c>false</c> nếu tọa độ ngoài giới hạn hoặc khối không thay đổi.</returns>
        bool SetBlock(BlockPos position, BlockId blockId);
    }
}

