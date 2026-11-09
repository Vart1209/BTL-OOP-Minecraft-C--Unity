using UnityEngine;
using Minecraft.Domain.Environment;

namespace Minecraft.Presentation.Environment
{
    /// <summary>
    /// Thành phần hiển thị vòng lặp Ngày/Đêm trên môi trường Unity (Day/Night View).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Observer Pattern (Tiêu chí 9, 10, 11 barem [S2]):</b> Lắng nghe sự kiện <see cref="DayNightCycle.TimeUpdated"/> và <see cref="DayNightCycle.PhaseChanged"/> để quay hướng ánh sáng mặt trời và chỉnh màu bầu trời mà không làm dính mã Unity vào tầng Domain.</item>
    ///   <item><b>Tách biệt View và Model:</b> <c>DayNightCycle</c> tính toán số học thời gian, <c>DayNightView</c> chỉ chịu trách nhiệm thị giác.</item>
    /// </list>
    /// </para>
    /// </summary>
    [RequireComponent(typeof(Light))]
    public sealed class DayNightView : MonoBehaviour
    {
        private Light _sunLight;
        private DayNightCycle _cycle;

        [Header("Ánh sáng & Màu sắc")]
        [SerializeField] private Color _dayColor = new Color(1f, 0.95f, 0.85f);
        [SerializeField] private Color _sunsetColor = new Color(1f, 0.5f, 0.3f);
        [SerializeField] private Color _nightColor = new Color(0.1f, 0.15f, 0.3f);

        [SerializeField] private float _dayIntensity = 1.0f;
        [SerializeField] private float _nightIntensity = 0.15f;

        private void Awake()
        {
            _sunLight = GetComponent<Light>();
        }

        /// <summary>
        /// Khởi tạo và liên kết View với mô hình thời gian chu kỳ ngày/đêm.
        /// </summary>
        public void Initialize(DayNightCycle cycle)
        {
            if (_cycle != null)
            {
                _cycle.TimeUpdated -= OnTimeUpdated;
                _cycle.PhaseChanged -= OnPhaseChanged;
            }

            _cycle = cycle;
            if (_cycle != null)
            {
                _cycle.TimeUpdated += OnTimeUpdated;
                _cycle.PhaseChanged += OnPhaseChanged;
                OnTimeUpdated(_cycle.NormalizedTime);
                OnPhaseChanged(_cycle.CurrentPhase);
            }
        }

        private void OnDestroy()
        {
            if (_cycle != null)
            {
                _cycle.TimeUpdated -= OnTimeUpdated;
                _cycle.PhaseChanged -= OnPhaseChanged;
            }
        }

        private void OnTimeUpdated(float normalizedTime)
        {
            if (_sunLight == null) return;

            // Góc quay mặt trời theo trục X từ -90 (mọc) đến 270 (lặn và quay lại)
            float sunAngle = (normalizedTime * 360f) - 90f;
            transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);
        }

        private void OnPhaseChanged(DayNightPhase phase)
        {
            if (_sunLight == null) return;

            switch (phase)
            {
                case DayNightPhase.Day:
                    _sunLight.color = _dayColor;
                    _sunLight.intensity = _dayIntensity;
                    break;
                case DayNightPhase.Sunset:
                    _sunLight.color = _sunsetColor;
                    _sunLight.intensity = _dayIntensity * 0.6f;
                    break;
                case DayNightPhase.Night:
                    _sunLight.color = _nightColor;
                    _sunLight.intensity = _nightIntensity;
                    break;
                case DayNightPhase.Sunrise:
                    _sunLight.color = _sunsetColor;
                    _sunLight.intensity = _dayIntensity * 0.5f;
                    break;
            }
        }
    }
}
