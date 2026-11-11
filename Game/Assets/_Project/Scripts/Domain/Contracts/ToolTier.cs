namespace Minecraft.Domain.Contracts
{
    /// <summary>
    /// Bảng cấp bậc chất liệu của công cụ khai thác (Tool Tier) chuẩn Minecraft Java 1.12.
    /// <para>
    /// <b>Quy tắc phân cấp khai thác:</b>
    /// <list type="bullet">
    ///   <item><see cref="Hand"/>: Tay không hoặc vật phẩm thông thường.</item>
    ///   <item><see cref="Wood"/>: Cúp gỗ (khai thác được Đá, Than).</item>
    ///   <item><see cref="Stone"/>: Cúp đá (khai thác được Sắt).</item>
    ///   <item><see cref="Iron"/>: Cúp sắt (khai thác được Vàng, Kim cương).</item>
    ///   <item><see cref="Diamond"/>: Cúp kim cương (khai thác được toàn bộ quặng và Obsidian).</item>
    /// </list>
    /// </para>
    /// </summary>
    public enum ToolTier : byte
    {
        /// <summary>Tay không (không có công cụ chuyên dụng).</summary>
        Hand = 0,

        /// <summary>Cấp độ Gỗ (Tier 1).</summary>
        Wood = 1,

        /// <summary>Cấp độ Đá (Tier 2).</summary>
        Stone = 2,

        /// <summary>Cấp độ Sắt (Tier 3).</summary>
        Iron = 3,

        /// <summary>Cấp độ Kim cương (Tier 4).</summary>
        Diamond = 4
    }
}
