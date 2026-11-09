using System;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.Entities;
using Minecraft.Domain.GameModes;

namespace Minecraft.Application.Survival
{
    /// <summary>
    /// Dịch vụ ứng dụng điều phối Use Case Chuyển đổi GameMode (UC11).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Design Pattern áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>State Pattern (Tiêu chí 9, 10, 11 barem [S2]):</b> Chuyển đổi trạng thái thực thể người chơi giữa các đối tượng <see cref="SurvivalMode"/>, <see cref="CreativeMode"/> và <see cref="AdventureMode"/>.</item>
    ///   <item><b>Observer Pattern:</b> Phát sự kiện <see cref="GameModeChanged"/> để Presentation cập nhật hiển thị thanh máu/đói và cờ quyền bay.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class GameModeService
    {
        private readonly SurvivalMode _survivalMode = new SurvivalMode();
        private readonly CreativeMode _creativeMode = new CreativeMode();
        private readonly AdventureMode _adventureMode = new AdventureMode();

        /// <summary>Sự kiện phát ra khi chế độ chơi của người chơi thay đổi.</summary>
        public event Action<IGameMode> GameModeChanged;

        /// <summary>
        /// Chuyển đổi người chơi sang chế độ Sinh tồn (Survival).
        /// </summary>
        public void SwitchToSurvival(Player player)
        {
            ApplyMode(player, _survivalMode);
        }

        /// <summary>
        /// Chuyển đổi người chơi sang chế độ Sáng tạo (Creative).
        /// </summary>
        public void SwitchToCreative(Player player)
        {
            ApplyMode(player, _creativeMode);
        }

        /// <summary>
        /// Chuyển đổi người chơi sang chế độ Phiêu lưu (Adventure - Bài mở rộng).
        /// </summary>
        public void SwitchToAdventure(Player player)
        {
            ApplyMode(player, _adventureMode);
        }

        /// <summary>
        /// Chuyển đổi chế độ chơi theo chuỗi tên chỉ định ("Survival", "Creative", "Adventure").
        /// </summary>
        public void SwitchGameMode(Player player, string modeName)
        {
            if (string.Equals(modeName, "Creative", StringComparison.OrdinalIgnoreCase))
            {
                SwitchToCreative(player);
            }
            else if (string.Equals(modeName, "Adventure", StringComparison.OrdinalIgnoreCase))
            {
                SwitchToAdventure(player);
            }
            else
            {
                SwitchToSurvival(player);
            }
        }

        private void ApplyMode(Player player, IGameMode newMode)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));
            player.SetGameMode(newMode);
            GameModeChanged?.Invoke(newMode);
        }
    }
}
