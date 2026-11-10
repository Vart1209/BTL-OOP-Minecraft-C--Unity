using System.Collections.Generic;
using UnityEngine;
using Minecraft.Domain.Entities.Mobs;

namespace Minecraft.Presentation.Mobs
{
    /// <summary>
    /// Thành phần hiển thị trực quan 3D kiểu Voxel cho một sinh vật (Mob View) trong không gian Unity 3D.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Mô hình MVC / MVP (Presentation Layer - Tiêu chí 4):</b> <c>MobView</c> chỉ là lớp hiển thị, lắng nghe sự thay đổi của thực thể <see cref="Mob"/> tầng Domain và đồng bộ Transform.</item>
    ///   <item><b>Đa hình (Polymorphism):</b> Gắn kết với lớp cha trừu tượng <see cref="Mob"/>, tự động dựng mô hình khối 3D đặc trưng cho từng loài (<see cref="Zombie"/>, <see cref="Cow"/>, <see cref="Skeleton"/>).</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class MobView : MonoBehaviour
    {
        private Mob _mobModel;
        private readonly List<Renderer> _partRenderers = new List<Renderer>();
        private readonly List<Color> _originalColors = new List<Color>();
        private float _hitFlashTimer;
        private int _lastKnownHealth;

        /// <summary>Thực thể dữ liệu Domain liên kết với View này.</summary>
        public Mob MobModel => _mobModel;

        /// <summary>
        /// Khởi tạo và liên kết View với đối tượng sinh vật Domain, đồng thời dựng hình khối 3D tương ứng.
        /// </summary>
        public void Initialize(Mob mobModel)
        {
            _mobModel = mobModel;
            if (_mobModel != null)
            {
                _lastKnownHealth = _mobModel.Health.Current;
                transform.position = new Vector3(_mobModel.Position.X, _mobModel.Position.Y, _mobModel.Position.Z);
                BuildVoxelMobVisuals(_mobModel);
            }
        }

        private void BuildVoxelMobVisuals(Mob mob)
        {
            BoxCollider rootCol = gameObject.GetComponent<BoxCollider>();
            if (rootCol == null)
            {
                rootCol = gameObject.AddComponent<BoxCollider>();
            }
            rootCol.center = new Vector3(0f, 0.9f, 0f);
            rootCol.size = new Vector3(0.8f, 1.8f, 0.8f);

            if (mob is Cow)
            {
                // Mô hình Bò 4 chân (Thân nâu đốm trắng, đầu có sừng)
                CreatePart("Body", new Vector3(0f, 0.75f, 0f), new Vector3(0.85f, 0.65f, 1.25f), new Color(0.42f, 0.26f, 0.16f));
                CreatePart("Spot", new Vector3(0f, 0.78f, 0.1f), new Vector3(0.87f, 0.45f, 0.5f), new Color(0.92f, 0.90f, 0.86f));
                CreatePart("Head", new Vector3(0f, 1.0f, 0.72f), new Vector3(0.55f, 0.55f, 0.55f), new Color(0.46f, 0.30f, 0.20f));
                CreatePart("LegFL", new Vector3(-0.28f, 0.25f, 0.42f), new Vector3(0.24f, 0.5f, 0.24f), new Color(0.35f, 0.22f, 0.14f));
                CreatePart("LegFR", new Vector3(0.28f, 0.25f, 0.42f), new Vector3(0.24f, 0.5f, 0.24f), new Color(0.35f, 0.22f, 0.14f));
                CreatePart("LegBL", new Vector3(-0.28f, 0.25f, -0.42f), new Vector3(0.24f, 0.5f, 0.24f), new Color(0.35f, 0.22f, 0.14f));
                CreatePart("LegBR", new Vector3(0.28f, 0.25f, -0.42f), new Vector3(0.24f, 0.5f, 0.24f), new Color(0.35f, 0.22f, 0.14f));
            }
            else if (mob is Skeleton)
            {
                // Mô hình Skeleton xương trắng
                CreatePart("Head", new Vector3(0f, 1.55f, 0f), new Vector3(0.5f, 0.5f, 0.5f), new Color(0.88f, 0.88f, 0.84f));
                CreatePart("Body", new Vector3(0f, 1.0f, 0f), new Vector3(0.45f, 0.65f, 0.25f), new Color(0.80f, 0.80f, 0.76f));
                CreatePart("ArmL", new Vector3(-0.35f, 1.15f, 0.3f), new Vector3(0.18f, 0.2f, 0.65f), new Color(0.85f, 0.85f, 0.80f));
                CreatePart("ArmR", new Vector3(0.35f, 1.15f, 0.3f), new Vector3(0.18f, 0.2f, 0.65f), new Color(0.85f, 0.85f, 0.80f));
                CreatePart("LegL", new Vector3(-0.14f, 0.35f, 0f), new Vector3(0.18f, 0.7f, 0.18f), new Color(0.78f, 0.78f, 0.74f));
                CreatePart("LegR", new Vector3(0.14f, 0.35f, 0f), new Vector3(0.18f, 0.7f, 0.18f), new Color(0.78f, 0.78f, 0.74f));
            }
            else
            {
                // Mô hình Zombie (Đầu xanh lá, áo xanh lam, quần tím than, 2 tay giơ ra trước)
                CreatePart("Head", new Vector3(0f, 1.55f, 0f), new Vector3(0.52f, 0.52f, 0.52f), new Color(0.32f, 0.62f, 0.28f));
                CreatePart("Body", new Vector3(0f, 1.0f, 0f), new Vector3(0.52f, 0.65f, 0.30f), new Color(0.18f, 0.56f, 0.68f));
                CreatePart("ArmL", new Vector3(-0.38f, 1.18f, 0.32f), new Vector3(0.22f, 0.22f, 0.68f), new Color(0.32f, 0.62f, 0.28f));
                CreatePart("ArmR", new Vector3(0.38f, 1.18f, 0.32f), new Vector3(0.22f, 0.22f, 0.68f), new Color(0.32f, 0.62f, 0.28f));
                CreatePart("LegL", new Vector3(-0.14f, 0.35f, 0f), new Vector3(0.22f, 0.7f, 0.24f), new Color(0.24f, 0.22f, 0.54f));
                CreatePart("LegR", new Vector3(0.14f, 0.35f, 0f), new Vector3(0.22f, 0.7f, 0.24f), new Color(0.24f, 0.22f, 0.54f));
            }
        }

        private void CreatePart(string partName, Vector3 localPos, Vector3 scale, Color color)
        {
            GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part.name = partName;
            part.transform.SetParent(transform, worldPositionStays: false);
            part.transform.localPosition = localPos;
            part.transform.localScale = scale;

            Collider col = part.GetComponent<Collider>();
            if (col != null)
            {
                Destroy(col);
            }

            Renderer r = part.GetComponent<Renderer>();
            if (r != null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                Material mat = new Material(shader) { color = color };
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
                if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 0f);
                if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 0f);
                r.sharedMaterial = mat;
                _partRenderers.Add(r);
                _originalColors.Add(color);
            }
        }

        private void Update()
        {
            if (_mobModel == null)
            {
                return;
            }

            // Nếu sinh vật đã tử vong, tự động tiêu hủy GameObject trên Scene
            if (!_mobModel.IsAlive)
            {
                Destroy(gameObject);
                return;
            }

            // Nháy đỏ khi nhận sát thương
            if (_mobModel.Health.Current < _lastKnownHealth)
            {
                _lastKnownHealth = _mobModel.Health.Current;
                _hitFlashTimer = 0.22f;
                SetPartsColor(new Color(1f, 0.2f, 0.2f));
            }

            if (_hitFlashTimer > 0f)
            {
                _hitFlashTimer -= Time.deltaTime;
                if (_hitFlashTimer <= 0f)
                {
                    RestoreOriginalColors();
                }
            }

            // Cập nhật vị trí và xoay mặt theo hướng di chuyển mượt mà
            Vector3 targetPos = new Vector3(_mobModel.Position.X, _mobModel.Position.Y, _mobModel.Position.Z);
            Vector3 moveDelta = targetPos - transform.position;
            moveDelta.y = 0f;
            if (moveDelta.sqrMagnitude > 0.0005f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDelta.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 8f);
            }

            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 10f);
        }

        private void SetPartsColor(Color flashColor)
        {
            for (int i = 0; i < _partRenderers.Count; i++)
            {
                if (_partRenderers[i] != null && _partRenderers[i].sharedMaterial != null)
                {
                    _partRenderers[i].sharedMaterial.color = flashColor;
                }
            }
        }

        private void RestoreOriginalColors()
        {
            for (int i = 0; i < _partRenderers.Count; i++)
            {
                if (_partRenderers[i] != null && _partRenderers[i].sharedMaterial != null)
                {
                    _partRenderers[i].sharedMaterial.color = _originalColors[i];
                }
            }
        }
    }
}
