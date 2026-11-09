using System;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.Entities
{
    /// <summary>
    /// Hệ thống quản lý thanh độ đói (Hunger) và nhịp sinh học của người chơi.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Quy tắc nghiệp vụ (Quy tắc R8 - dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Đóng gói (Encapsulation - Tiêu chí 13):</b> <c>_current</c> được bảo vệ trong khoảng [0, Max], tăng khi ăn thức ăn và giảm dần theo thời gian/hoạt động.</item>
    ///   <item><b>Observer Pattern (Tiêu chí 9, 10, 11):</b> Phát sự kiện <see cref="HungerChanged"/> mỗi khi chỉ số đói thay đổi để HUD cập nhật.</item>
    ///   <item><b>Quy tắc R8:</b> Mỗi chu kỳ 4.0 giây: nếu <c>Current == 0</c> thì trừ 1 máu (chết đói); nếu <c>Current &gt;= 18</c> và máu chưa đầy thì hồi 1 máu.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class Hunger
    {
        /// <summary>Chu kỳ kiểm tra hiệu ứng đói/hồi phục tính bằng giây.</summary>
        public const float HungerIntervalSeconds = 4.0f;

        private int _current;
        private readonly int _max;
        private float _tickTimer;

        /// <summary>Chỉ số điểm đói hiện tại (0..Max).</summary>
        public int Current => _current;

        /// <summary>Chỉ số điểm đói tối đa (mặc định = 20).</summary>
        public int Max => _max;

        /// <summary>Sự kiện phát ra khi điểm đói thay đổi (Current, Max).</summary>
        public event Action<int, int> HungerChanged;

        /// <summary>
        /// Khởi tạo thanh đói với giá trị tối đa chỉ định.
        /// </summary>
        public Hunger(int maxHunger = 20)
        {
            if (maxHunger <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxHunger), "Chỉ số đói tối đa phải > 0.");
            }

            _max = maxHunger;
            _current = maxHunger;
            _tickTimer = 0f;
        }

        /// <summary>
        /// Tiêu thụ thức ăn để phục hồi điểm đói.
        /// </summary>
        public void Eat(int restoreAmount)
        {
            if (restoreAmount <= 0)
            {
                return;
            }

            int old = _current;
            _current = Math.Min(_max, _current + restoreAmount);

            if (_current != old)
            {
                HungerChanged?.Invoke(_current, _max);
            }
        }

        /// <summary>
        /// Giảm điểm đói khi vận động.
        /// </summary>
        public void Reduce(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            int old = _current;
            _current = Math.Max(0, _current - amount);

            if (_current != old)
            {
                HungerChanged?.Invoke(_current, _max);
            }
        }

        /// <summary>
        /// Khôi phục toàn bộ thanh đói về mức tối đa (dùng khi hồi sinh).
        /// </summary>
        public void ResetToMax()
        {
            _current = _max;
            _tickTimer = 0f;
            HungerChanged?.Invoke(_current, _max);
        }

        /// <summary>
        /// Cập nhật nhịp sinh học đói/hồi máu theo thời gian (Quy tắc R8).
        /// </summary>
        /// <param name="deltaTime">Khoảng thời gian trôi qua giữa các khung hình (giây).</param>
        /// <param name="playerHealth">Đối tượng máu của người chơi chịu ảnh hưởng.</param>
        /// <param name="gameMode">Chế độ chơi (Creative bỏ qua cơ chế đói).</param>
        public void Tick(float deltaTime, Health playerHealth, IGameMode gameMode)
        {
            if (deltaTime <= 0f)
            {
                return;
            }

            // Creative không bị ảnh hưởng bởi thanh đói
            if (gameMode != null && !gameMode.CanTakeDamage)
            {
                return;
            }

            _tickTimer += deltaTime;
            if (_tickTimer >= HungerIntervalSeconds)
            {
                _tickTimer -= HungerIntervalSeconds;

                // Quy tắc R8: Đói = 0 thì mất 1 máu
                if (_current == 0)
                {
                    playerHealth?.TakeDamage(1, gameMode);
                }
                // Quy tắc R8: Đói >= 18 thì hồi 1 máu
                else if (_current >= 18 && playerHealth != null && playerHealth.Current < playerHealth.Max)
                {
                    playerHealth.Heal(1);
                }
            }
        }
    }
}
