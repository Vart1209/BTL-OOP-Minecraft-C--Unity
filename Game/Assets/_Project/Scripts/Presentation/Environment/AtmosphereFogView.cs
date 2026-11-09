using System;
using UnityEngine;
using Minecraft.Domain.Environment;

namespace Minecraft.Presentation.Environment
{
    /// <summary>
    /// Thành phần điều khiển hiệu ứng Sương mù (Fog) và Ánh sáng môi trường (Ambient Light) theo chu kỳ Ngày/Đêm (Tuần T4).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Observer Pattern (Tiêu chí 9, 10, 11):</b> Đăng ký lắng nghe sự kiện <see cref="DayNightCycle.PhaseChanged"/> và <see cref="DayNightCycle.TimeUpdated"/> (cùng bí danh <c>TimeChanged</c>) để nội suy mượt mà màu sắc sương mù (<see cref="RenderSettings.fogColor"/>), mật độ sương mù (<see cref="RenderSettings.fogDensity"/>) và ánh sáng môi trường (<see cref="RenderSettings.ambientLight"/>) giữa 4 buổi Ban ngày, Hoàng hôn, Ban đêm và Bình minh.</item>
    ///   <item><b>Tách biệt hiển thị &amp; nghiệp vụ (Tiêu chí 4, 8):</b> Toàn bộ gọi lệnh tới <see cref="RenderSettings"/> của Unity nằm gọn tại tầng Presentation, không xâm phạm tầng Domain.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class AtmosphereFogView : MonoBehaviour
    {
        [Header("Màu sương mù theo các buổi trong ngày")]
        [SerializeField] private Color _dayFogColor = new Color(0.72f, 0.86f, 0.98f, 1f);
        [SerializeField] private Color _sunsetFogColor = new Color(0.92f, 0.52f, 0.34f, 1f);
        [SerializeField] private Color _nightFogColor = new Color(0.05f, 0.07f, 0.14f, 1f);
        [SerializeField] private Color _sunriseFogColor = new Color(0.95f, 0.68f, 0.45f, 1f);

        [Header("Ánh sáng môi trường (Ambient Light)")]
        [SerializeField] private Color _dayAmbientColor = new Color(0.65f, 0.68f, 0.72f, 1f);
        [SerializeField] private Color _sunsetAmbientColor = new Color(0.48f, 0.32f, 0.28f, 1f);
        [SerializeField] private Color _nightAmbientColor = new Color(0.12f, 0.14f, 0.22f, 1f);
        [SerializeField] private Color _sunriseAmbientColor = new Color(0.52f, 0.42f, 0.36f, 1f);

        [Header("Mật độ sương mù (Fog Density)")]
        [SerializeField] private float _dayFogDensity = 0.0025f;
        [SerializeField] private float _nightFogDensity = 0.0065f;

        private DayNightCycle _cycle;
        private DayNightPhase _currentPhase;

        /// <summary>
        /// Khởi tạo và đăng ký lắng nghe chu kỳ Ngày/Đêm từ <c>GameBootstrap</c>.
        /// </summary>
        public void Initialize(DayNightCycle cycle)
        {
            if (_cycle != null)
            {
                _cycle.PhaseChanged -= OnPhaseChanged;
                _cycle.TimeChanged -= OnTimeChanged;
            }

            _cycle = cycle ?? throw new ArgumentNullException(nameof(cycle));

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;

            _cycle.PhaseChanged += OnPhaseChanged;
            _cycle.TimeChanged += OnTimeChanged;

            OnPhaseChanged(_cycle.CurrentPhase);
            OnTimeChanged(_cycle.NormalizedTime);
        }

        private void OnDestroy()
        {
            if (_cycle != null)
            {
                _cycle.PhaseChanged -= OnPhaseChanged;
                _cycle.TimeChanged -= OnTimeChanged;
            }
        }

        private void OnPhaseChanged(DayNightPhase newPhase)
        {
            _currentPhase = newPhase;
        }

        private void OnTimeChanged(float normalizedTime)
        {
            Color targetFog;
            Color targetAmbient;
            float targetDensity;

            if (normalizedTime < 0.45f)
            {
                // Ban ngày (0.00 -> 0.45)
                float t = Mathf.Clamp01(normalizedTime / 0.45f);
                targetFog = Color.Lerp(_dayFogColor, _sunsetFogColor, Mathf.SmoothStep(0f, 1f, Mathf.Max(0f, (t - 0.8f) * 5f)));
                targetAmbient = _dayAmbientColor;
                targetDensity = _dayFogDensity;
            }
            else if (normalizedTime < 0.55f)
            {
                // Hoàng hôn (0.45 -> 0.55)
                float t = Mathf.Clamp01((normalizedTime - 0.45f) / 0.10f);
                targetFog = Color.Lerp(_sunsetFogColor, _nightFogColor, t);
                targetAmbient = Color.Lerp(_sunsetAmbientColor, _nightAmbientColor, t);
                targetDensity = Mathf.Lerp(_dayFogDensity, _nightFogDensity, t);
            }
            else if (normalizedTime < 0.90f)
            {
                // Ban đêm (0.55 -> 0.90)
                targetFog = _nightFogColor;
                targetAmbient = _nightAmbientColor;
                targetDensity = _nightFogDensity;
            }
            else
            {
                // Bình minh (0.90 -> 1.00)
                float t = Mathf.Clamp01((normalizedTime - 0.90f) / 0.10f);
                targetFog = Color.Lerp(_sunriseFogColor, _dayFogColor, t);
                targetAmbient = Color.Lerp(_sunriseAmbientColor, _dayAmbientColor, t);
                targetDensity = Mathf.Lerp(_nightFogDensity, _dayFogDensity, t);
            }

            RenderSettings.fogColor = targetFog;
            RenderSettings.fogDensity = targetDensity;
            RenderSettings.ambientLight = targetAmbient;
        }
    }
}
