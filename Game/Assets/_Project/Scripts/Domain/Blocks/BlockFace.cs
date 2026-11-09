namespace Minecraft.Domain.Blocks
{
    /// <summary>
    /// Định danh 6 hướng mặt của một khối lập phương Voxel trong không gian 3 chiều.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Tối ưu sinh lưới (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item>Dùng cho thuật toán loại bỏ mặt khuất (Face Culling): Mỗi khối chỉ vẽ mặt khi khối kề theo hướng mặt đó là không khí hoặc khối trong suốt.</item>
    ///   <item>Hỗ trợ ánh xạ texture riêng cho từng mặt khối (ví dụ: khối Cỏ có mặt Top màu cỏ, mặt Bottom màu đất, 4 mặt xung quanh màu đất viền cỏ).</item>
    /// </list>
    /// </para>
    /// </summary>
    public enum BlockFace : byte
    {
        /// <summary>Mặt trên (hướng trục +Y).</summary>
        Top = 0,

        /// <summary>Mặt dưới (hướng trục -Y).</summary>
        Bottom = 1,

        /// <summary>Mặt phía Bắc (hướng trục +Z).</summary>
        North = 2,

        /// <summary>Mặt phía Nam (hướng trục -Z).</summary>
        South = 3,

        /// <summary>Mặt phía Đông (hướng trục +X).</summary>
        East = 4,

        /// <summary>Mặt phía Tây (hướng trục -X).</summary>
        West = 5
    }
}
