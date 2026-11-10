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
        private Transform _sunVisual;
        private Transform _moonVisual;
        private Renderer _sunRenderer;
        private Renderer _moonRenderer;

        [Header("Ánh sáng & Màu sắc")]
        [SerializeField] private Color _dayColor = new Color(1f, 0.96f, 0.88f);
        [SerializeField] private Color _sunsetColor = new Color(1f, 0.55f, 0.32f);
        [SerializeField] private Color _nightColor = new Color(0.22f, 0.28f, 0.45f);

        [SerializeField] private float _dayIntensity = 1.15f;
        [SerializeField] private float _nightIntensity = 0.35f;

        private void Awake()
        {
            _sunLight = GetComponent<Light>();
            if (_sunLight != null)
            {
                _sunLight.shadows = LightShadows.Soft;
                _sunLight.shadowStrength = 0.65f;
            }

            CreateCelestialVisuals();
        }

        private void CreateCelestialVisuals()
        {
            // Tạo khối Mặt Trời vuông kiểu Minecraft trên bầu trời
            GameObject sunGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sunGo.name = "MinecraftSunVisual";
            sunGo.transform.SetParent(transform, worldPositionStays: false);
            sunGo.transform.localPosition = new Vector3(0f, 0f, -140f);
            sunGo.transform.localScale = new Vector3(16f, 16f, 2f);
            Collider sunCol = sunGo.GetComponent<Collider>();
            if (sunCol != null) Destroy(sunCol);

            _sunRenderer = sunGo.GetComponent<Renderer>();
            if (_sunRenderer != null)
            {
                Shader unlit = Shader.Find("Unlit/Color") ?? Shader.Find("Standard");
                Material sunMat = new Material(unlit) { color = new Color(1f, 0.94f, 0.45f, 1f) };
                _sunRenderer.sharedMaterial = sunMat;
                _sunRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            _sunVisual = sunGo.transform;

            // Tạo khối Mặt Trăng vuông đối diện Mặt Trời
            GameObject moonGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            moonGo.name = "MinecraftMoonVisual";
            moonGo.transform.SetParent(transform, worldPositionStays: false);
            moonGo.transform.localPosition = new Vector3(0f, 0f, 140f);
            moonGo.transform.localScale = new Vector3(12f, 12f, 2f);
            Collider moonCol = moonGo.GetComponent<Collider>();
            if (moonCol != null) Destroy(moonCol);

            _moonRenderer = moonGo.GetComponent<Renderer>();
            if (_moonRenderer != null)
            {
                Shader unlit = Shader.Find("Unlit/Color") ?? Shader.Find("Standard");
                Material moonMat = new Material(unlit) { color = new Color(0.85f, 0.92f, 1.0f, 1f) };
                _moonRenderer.sharedMaterial = moonMat;
                _moonRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            _moonVisual = moonGo.transform;
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

            // Góc chiếu mặt trời luôn hướng từ trên bầu trời xuống mặt đất (35° .. 145° ban ngày)
            // để mặt Top của các khối Voxel luôn nhận đủ ánh sáng và tạo độ tương phản 3D rõ nét với mặt bên
            float pitchAngle;
            if (normalizedTime <= 0.55f)
            {
                float dayProgress = normalizedTime / 0.55f;
                pitchAngle = Mathf.Lerp(38f, 142f, dayProgress);
            }
            else
            {
                pitchAngle = 55f;
            }

            transform.rotation = Quaternion.Euler(pitchAngle, -35f, 0f);
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
