namespace Minecraft.Domain.Contracts
{
    /// <summary>
    /// Hợp đồng chế độ chơi &amp; phân quyền tác nhân (Game Mode State / Actor Permission Contract).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Design Pattern áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Phân quyền tác nhân (Tiêu chí 4 trong barem [S2]):</b> Thể hiện rõ sự khác biệt quyền hạn giữa hai tác nhân <i>Người chơi Survival</i> và <i>Người chơi Creative</i> ngay tại tầng nghiệp vụ Domain thay vì dùng câu lệnh <c>if/else</c> rải rác ở giao diện UI.</item>
    ///   <item><b>State / Strategy Pattern (Tiêu chí 9, 10, 11, 12):</b> Khi đổi chế độ chơi lúc đang chạy (Use Case Đổi GameMode), đối tượng <c>Player</c> hoặc dịch vụ nghiệp vụ chỉ cần trỏ sang thực thể <see cref="IGameMode"/> mới (<c>SurvivalMode</c>, <c>CreativeMode</c>, hoặc mở rộng thêm <c>AdventureMode</c> khi vấn đáp).</item>
    /// </list>
    /// </para>
    /// </summary>
    public interface IGameMode
    {
        /// <summary>
        /// Tên hiển thị của chế độ chơi (ví dụ: "Survival", "Creative", "Adventure").
        /// </summary>
        string ModeName { get; }

        /// <summary>
        /// Xác định người chơi có bị mất máu khi bị Mob tấn công hoặc khi thanh đói về 0 hay không (Quy tắc R8, R9, R12).
        /// </summary>
        bool CanTakeDamage { get; }

        /// <summary>
        /// Xác định khi đặt khối xuống thế giới có bị trừ số lượng vật phẩm trong túi đồ hay không (Quy tắc R12).
        /// </summary>
        bool ConsumesItemsOnPlace { get; }

        /// <summary>
        /// Xác định người chơi có thể phá vỡ khối ngay lập tức (1 chạm) mà không cần chờ thời gian đào theo độ cứng hay không (Quy tắc R4, R12).
        /// </summary>
        bool InstantBreak { get; }

        /// <summary>
        /// Xác định người chơi có quyền bay tự do trong không trung hay không (Quy tắc R12).
        /// </summary>
        bool CanFly { get; }

        /// <summary>
        /// Kiểm tra người chơi ở chế độ hiện tại có được phép phá hủy loại khối chỉ định hay không.
        /// (Ví dụ: ở Survival không phá được <see cref="BlockId.Bedrock"/> theo Quy tắc R1; ở Creative phá được cả Bedrock; ở Adventure không phá được khối nào).
        /// </summary>
        /// <param name="blockId">Mã loại khối mục tiêu.</param>
        /// <returns><c>true</c> nếu được phép phá; ngược lại <c>false</c>.</returns>
        bool CanBreakBlock(BlockId blockId);
    }
}

