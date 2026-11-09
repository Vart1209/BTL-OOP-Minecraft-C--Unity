using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.GameModes
{
    /// <summary>
    /// Chế độ chơi Sáng tạo (Creative Mode).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Design Pattern áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>State Pattern (Tiêu chí 9, 10, 11):</b> Thực thi hợp đồng <see cref="IGameMode"/> đại diện cho trạng thái sáng tạo và quyền năng mở rộng.</item>
    ///   <item><b>Đặc quyền tác nhân (Quy tắc R12, Tiêu chí 4 barem [S2]):</b> Người chơi miễn nhiễm sát thương, khối đặt không bị trừ trong túi đồ, phá khối ngay lập tức (1 chạm), có quyền bay và phá được cả Bedrock.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class CreativeMode : IGameMode
    {
        /// <inheritdoc />
        public string ModeName => "Creative";

        /// <inheritdoc />
        public bool CanTakeDamage => false;

        /// <inheritdoc />
        public bool ConsumesItemsOnPlace => false;

        /// <inheritdoc />
        public bool InstantBreak => true;

        /// <inheritdoc />
        public bool CanFly => true;

        /// <inheritdoc />
        public bool CanBreakBlock(BlockId blockId)
        {
            // Quy tắc R12: Trong chế độ Sáng tạo, người chơi có thể phá mọi khối kể cả Bedrock
            return true;
        }
    }
}
