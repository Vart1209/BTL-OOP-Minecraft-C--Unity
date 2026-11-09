using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.GameModes
{
    /// <summary>
    /// Chế độ chơi Phiêu lưu (Adventure Mode - Bài mở rộng luyện thi vấn đáp).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Design Pattern áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>State Pattern (Tiêu chí 9, 10, 11, 12):</b> Minh chứng khả năng mở rộng kiến trúc không cần sửa mã nguồn hiện có (Open-Closed Principle). Khi thêm chế độ Phiêu lưu, chỉ cần thêm lớp <see cref="AdventureMode"/> mới mà không làm vỡ logic của người chơi hay thế giới.</item>
    ///   <item><b>Quy tắc phiêu lưu:</b> Người chơi chịu sát thương bình thường nhưng bị cấm hoàn toàn hành vi phá hoại địa hình (không được phá bất kỳ khối nào).</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class AdventureMode : IGameMode
    {
        /// <inheritdoc />
        public string ModeName => "Adventure";

        /// <inheritdoc />
        public bool CanTakeDamage => true;

        /// <inheritdoc />
        public bool ConsumesItemsOnPlace => true;

        /// <inheritdoc />
        public bool InstantBreak => false;

        /// <inheritdoc />
        public bool CanFly => false;

        /// <inheritdoc />
        public bool CanBreakBlock(BlockId blockId)
        {
            // Trong chế độ Phiêu lưu, người chơi không được phép phá bất kỳ khối nào để giữ nguyên bản đồ
            return false;
        }
    }
}
