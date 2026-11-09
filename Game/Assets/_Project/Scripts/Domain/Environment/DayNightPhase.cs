namespace Minecraft.Domain.Environment
{
    /// <summary>
    /// Các giai đoạn trong chu kỳ Ngày/Đêm của thế giới Voxel.
    /// </summary>
    public enum DayNightPhase
    {
        /// <summary>Ban ngày (mặt trời chiếu sáng, an toàn).</summary>
        Day = 0,

        /// <summary>Hoàng hôn (mặt trời lặn, ánh sáng giảm dần).</summary>
        Sunset = 1,

        /// <summary>Ban đêm (bóng tối bao phủ, quái vật bắt đầu xuất hiện).</summary>
        Night = 2,

        /// <summary>Bình minh (mặt trời mọc trở lại).</summary>
        Sunrise = 3
    }
}
