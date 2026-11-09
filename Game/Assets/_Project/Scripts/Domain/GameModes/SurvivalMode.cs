using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.GameModes
{
    /// <summary>
    /// Chế độ chơi Sinh tồn (Survival Mode).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Design Pattern áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>State Pattern (Tiêu chí 9, 10, 11):</b> Thực thi hợp đồng <see cref="IGameMode"/> đại diện cho trạng thái sinh tồn tiêu chuẩn của người chơi.</item>
    ///   <item><b>Phân quyền tác nhân (Tiêu chí 4 barem [S2]):</b> Người chơi chịu sát thương, tốn tài nguyên khi đặt khối, đào khối theo thời gian và không được phá Bedrock (Quy tắc R1).</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class SurvivalMode : IGameMode
    {
        /// <inheritdoc />
        public string ModeName => "Survival";

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
            // Quy tắc R1: Trong chế độ Sinh tồn, khối Đá nền (Bedrock) không thể bị phá hủy
            return blockId != BlockId.Bedrock;
        }
    }
}
