using System;

namespace Minecraft.Domain.Environment
{
    /// <summary>
    /// Hệ thống mô phỏng chu kỳ Ngày/Đêm tuần hoàn trong thế giới (Day/Night Cycle).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Đóng gói (Encapsulation - Tiêu chí 13):</b> Quản lý thời gian trôi <c>_currentTimeInSeconds</c> và độ dài chu kỳ hoàn toàn độc lập với phần cứng hay engine.</item>
    ///   <item><b>Observer Pattern (Tiêu chí 9, 10, 11):</b> Cung cấp các sự kiện <see cref="PhaseChanged"/> và <see cref="TimeUpdated"/> để hệ thống Spawner (Domain/Application) và đèn DirectionalLight/Skybox (Presentation) tự động phản ứng mà không gây phụ thuộc ngược.</item>
    ///   <item><b>Quy tắc R11:</b> Cung cấp cờ <see cref="IsNight"/> để kiểm soát điều kiện sinh sản của Zombie.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class DayNightCycle
    {
        /// <summary>Độ dài mặc định của một ngày trong game (240 giây = 4 phút ngoài đời).</summary>
        public const float DefaultDayLengthSeconds = 240.0f;

        private readonly float _dayLengthSeconds;
        private float _currentTimeInSeconds;
        private DayNightPhase _currentPhase;

        /// <summary>Độ dài của một chu kỳ trọn vẹn (giây).</summary>
        public float DayLengthSeconds => _dayLengthSeconds;

        /// <summary>Thời gian hiện tại trong ngày (0..DayLengthSeconds).</summary>
        public float CurrentTimeInSeconds => _currentTimeInSeconds;

        /// <summary>Thời gian chuẩn hóa trong khoảng [0.0, 1.0).</summary>
        public float NormalizedTime => _currentTimeInSeconds / _dayLengthSeconds;

        /// <summary>Giai đoạn ngày/đêm hiện tại.</summary>
        public DayNightPhase CurrentPhase => _currentPhase;

        /// <summary>Xác định hiện tại có đang trong giai đoạn ban đêm hay không (Quy tắc R11).</summary>
        public bool IsNight => _currentPhase == DayNightPhase.Night;

        /// <summary>Sự kiện phát ra khi giai đoạn ngày/đêm chuyển đổi (Observer Pattern).</summary>
        public event Action<DayNightPhase> PhaseChanged;

        /// <summary>Sự kiện phát ra mỗi khung thời gian kèm thời gian chuẩn hóa [0.0, 1.0) để Presentation quay mặt trời.</summary>
        public event Action<float> TimeUpdated;

        /// <summary>
        /// Khởi tạo chu kỳ Ngày/Đêm.
        /// </summary>
        /// <param name="dayLengthSeconds">Tổng thời gian 1 ngày (giây), phải > 0.</param>
        /// <param name="initialNormalizedTime">Thời điểm xuất phát ban đầu [0.0, 1.0).</param>
        public DayNightCycle(float dayLengthSeconds = DefaultDayLengthSeconds, float initialNormalizedTime = 0.0f)
        {
            if (dayLengthSeconds <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(dayLengthSeconds), "Độ dài một ngày phải > 0.");
            }

            _dayLengthSeconds = dayLengthSeconds;
            SetTime(initialNormalizedTime);
        }

        /// <summary>
        /// Cập nhật chu kỳ thời gian theo lượng deltaTime trôi qua.
        /// </summary>
        public void Tick(float deltaTime)
        {
            if (deltaTime <= 0f)
            {
                return;
            }

            _currentTimeInSeconds = (_currentTimeInSeconds + deltaTime) % _dayLengthSeconds;
            if (_currentTimeInSeconds < 0f)
            {
                _currentTimeInSeconds += _dayLengthSeconds;
            }

            UpdatePhase();
            TimeUpdated?.Invoke(NormalizedTime);
        }

        /// <summary>
        /// Tua nhanh hoặc nhảy trực tiếp tới một thời điểm chuẩn hóa trong ngày [0.0, 1.0).
        /// </summary>
        public void SetTime(float normalizedTime)
        {
            float clamped = normalizedTime % 1.0f;
            if (clamped < 0f)
            {
                clamped += 1.0f;
            }

            _currentTimeInSeconds = clamped * _dayLengthSeconds;
            UpdatePhase();
            TimeUpdated?.Invoke(NormalizedTime);
        }

        private void UpdatePhase()
        {
            float norm = NormalizedTime;
            DayNightPhase newPhase;

            if (norm < 0.45f)
            {
                newPhase = DayNightPhase.Day;
            }
            else if (norm < 0.55f)
            {
                newPhase = DayNightPhase.Sunset;
            }
            else if (norm < 0.90f)
            {
                newPhase = DayNightPhase.Night;
            }
            else
            {
                newPhase = DayNightPhase.Sunrise;
            }

            if (newPhase != _currentPhase)
            {
                _currentPhase = newPhase;
                PhaseChanged?.Invoke(_currentPhase);
            }
        }
    }
}
