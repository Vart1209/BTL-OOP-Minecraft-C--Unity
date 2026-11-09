using System;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.Entities
{
    /// <summary>
    /// Hệ thống quản lý điểm sinh lực (Máu) của thực thể trong trò chơi.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Quy tắc nghiệp vụ (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Đóng gói (Encapsulation - Tiêu chí 13):</b> <c>_current</c> và <c>_max</c> là <c>private</c>, kiểm soát giá trị luôn nằm trong khoảng [0, Max].</item>
    ///   <item><b>Observer Pattern (Tiêu chí 9, 10, 11):</b> Phát sự kiện <see cref="HealthChanged"/> và <see cref="Died"/> để UI hiển thị và hệ thống hồi sinh lắng nghe độc lập.</item>
    ///   <item><b>Quy tắc R12:</b> Chế độ Creative (<c>gameMode.CanTakeDamage == false</c>) miễn nhiễm mọi loại sát thương.</item>
    ///   <item><b>Quy tắc R9:</b> Khi sinh lực về 0 (<c>Current == 0</c>), thực thể chết và kích hoạt sự kiện tử vong.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class Health
    {
        private int _current;
        private readonly int _max;

        /// <summary>Lượng máu hiện tại (0..Max).</summary>
        public int Current => _current;

        /// <summary>Lượng máu tối đa của thực thể.</summary>
        public int Max => _max;

        /// <summary>Thực thể còn sống hay không.</summary>
        public bool IsAlive => _current > 0;

        /// <summary>Sự kiện phát ra khi lượng máu thay đổi (Current, Max).</summary>
        public event Action<int, int> HealthChanged;

        /// <summary>Sự kiện phát ra khi thực thể cạn kiệt sinh lực (Current == 0).</summary>
        public event Action Died;

        /// <summary>
        /// Khởi tạo hệ thống máu với lượng máu tối đa chỉ định.
        /// </summary>
        public Health(int maxHealth = 20)
        {
            if (maxHealth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxHealth), "Lượng máu tối đa phải > 0.");
            }

            _max = maxHealth;
            _current = maxHealth;
        }

        /// <summary>
        /// Gây sát thương lên thực thể, có xét tới phân quyền tác nhân theo chế độ chơi (Quy tắc R12).
        /// </summary>
        /// <param name="amount">Lượng sát thương cần trừ (> 0).</param>
        /// <param name="gameMode">Chế độ chơi (Creative bỏ qua sát thương).</param>
        public void TakeDamage(int amount, IGameMode gameMode)
        {
            if (amount <= 0 || _current <= 0)
            {
                return;
            }

            // Quy tắc R12: Creative không nhận sát thương
            if (gameMode != null && !gameMode.CanTakeDamage)
            {
                return;
            }

            _current = Math.Max(0, _current - amount);
            HealthChanged?.Invoke(_current, _max);

            // Quy tắc R9: Máu về 0 tử vong
            if (_current == 0)
            {
                Died?.Invoke();
            }
        }

        /// <summary>
        /// Hồi phục sinh lực (không vượt quá Max).
        /// </summary>
        public void Heal(int amount)
        {
            if (amount <= 0 || _current <= 0)
            {
                return;
            }

            int old = _current;
            _current = Math.Min(_max, _current + amount);

            if (_current != old)
            {
                HealthChanged?.Invoke(_current, _max);
            }
        }

        /// <summary>
        /// Khôi phục toàn bộ sinh lực về mức tối đa (dùng khi hồi sinh).
        /// </summary>
        public void ResetToMax()
        {
            _current = _max;
            HealthChanged?.Invoke(_current, _max);
        }
    }
}
