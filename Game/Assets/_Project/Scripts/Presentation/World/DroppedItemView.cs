using System;
using UnityEngine;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.Entities;
using Minecraft.Domain.Items;

namespace Minecraft.Presentation.World
{
    /// <summary>
    /// Đối tượng hiển thị 3D cho thực thể vật phẩm rơi trên mặt đất (DroppedItemEntity).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Hiệu năng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Single Responsibility Principle (SRP - Tiêu chí 8):</b> Tách biệt tuyệt đối giữa mô hình dữ liệu thực thể (<see cref="DroppedItemEntity"/> trong Domain) và đối tượng trực quan hóa 3D trong không gian Unity Scene.</item>
    ///   <item><b>Mô phỏng chuyển động &amp; Xúc giác (Kinematics &amp; Tactile Feel):</b> Vật phẩm xoay đều 90°/s quanh trục Y, nhấp nhô sóng Sin và kích hoạt gia tốc lực hút nam châm (Magnetic Acceleration) lao về phía người chơi khi ở cự ly gần (&lt; 2.2m).</item>
    ///   <item><b>Vòng đời tự dọn dẹp:</b> Tự động tiêu hủy GameObject khi thực thể hết hạn (<c>IsExpired</c>) hoặc đã được nhặt sạch (<c>Payload.IsEmpty</c>).</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class DroppedItemView : MonoBehaviour
    {
        private DroppedItemEntity _entity;
        private Transform _playerTransform;
        private float _phaseOffset;
        private float _flySpeed = 3.5f;

        /// <summary>Thực thể dữ liệu Domain liên kết với View hiển thị này.</summary>
        public DroppedItemEntity Entity => _entity;

        /// <summary>
        /// Khởi tạo đối tượng hiển thị 3D cho vật phẩm rơi.
        /// </summary>
        /// <param name="entity">Thực thể vật phẩm rơi từ Domain.</param>
        /// <param name="playerTransform">Transform của người chơi để tính cự ly lực hút nam châm.</param>
        public void Initialize(DroppedItemEntity entity, Transform playerTransform)
        {
            _entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _playerTransform = playerTransform;
            _phaseOffset = UnityEngine.Random.Range(0f, Mathf.PI * 2f);

            transform.position = new Vector3(_entity.Position.X, _entity.Position.Y + 0.15f, _entity.Position.Z);
            transform.localScale = Vector3.one * 0.28f;

            // Tạo hình khối lập phương 3D thu nhỏ đại diện cho vật phẩm
            MeshFilter mf = gameObject.AddComponent<MeshFilter>();
            GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mf.sharedMesh = primitive.GetComponent<MeshFilter>().sharedMesh;
            Destroy(primitive);

            MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            Material mat = new Material(shader)
            {
                name = $"DroppedItem_{_entity.Payload.Definition.Name}"
            };

            Color itemColor = GetColorForItem(_entity.Payload.Definition);
            mat.color = itemColor;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", itemColor);
            if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 0f);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 0f);

            mr.sharedMaterial = mat;
        }

        private void Update()
        {
            if (_entity == null || !_entity.IsAlive || _entity.IsExpired || _entity.Payload.IsEmpty)
            {
                Destroy(gameObject);
                return;
            }

            // 1. Tự xoay đều 90°/s quanh trục thẳng đứng Y
            transform.Rotate(Vector3.up, 90f * Time.deltaTime, Space.World);

            // 2. Kiểm tra khoảng cách tới người chơi để hút nam châm
            if (_playerTransform != null)
            {
                Vector3 playerChest = _playerTransform.position + Vector3.up * 0.8f;
                float distance = Vector3.Distance(transform.position, playerChest);

                if (distance <= 2.2f)
                {
                    // Lực hút nam châm: Tăng dần tốc độ khi lao về phía người chơi
                    _flySpeed = Mathf.Min(_flySpeed + 12f * Time.deltaTime, 16f);
                    transform.position = Vector3.MoveTowards(transform.position, playerChest, _flySpeed * Time.deltaTime);

                    // Đồng bộ hóa tọa độ Domain để dịch vụ DeathDropService.CollectNearbyDrops thu nhặt
                    _entity.MoveTo(new System.Numerics.Vector3(transform.position.x, transform.position.y, transform.position.z));
                    return;
                }
            }

            // 3. Nếu chưa vào tầm hút: Nhấp nhô bồng bềnh hình sin trên mặt đất
            _flySpeed = 3.5f;
            float bob = Mathf.Sin((Time.time + _phaseOffset) * 3.2f) * 0.08f;
            Vector3 groundPos = new Vector3(_entity.Position.X, _entity.Position.Y, _entity.Position.Z);
            transform.position = groundPos + new Vector3(0f, bob + 0.18f, 0f);
        }

        private static Color GetColorForItem(Item item)
        {
            if (item == null)
            {
                return Color.white;
            }

            if (item is BlockItem blockItem && blockItem.PlaceBlockId != Domain.Contracts.BlockId.Air)
            {
                return ResourcePackView.GetBlockColor(blockItem.PlaceBlockId);
            }

            switch (item.Id)
            {
                case ItemId.Stick: return new Color(0.58f, 0.42f, 0.22f, 1f);
                case ItemId.WoodenPickaxe:
                case ItemId.WoodenSword:
                case ItemId.WoodenAxe: return new Color(0.62f, 0.46f, 0.26f, 1f);
                case ItemId.StonePickaxe: return new Color(0.55f, 0.55f, 0.55f, 1f);
                case ItemId.DiamondPickaxe: return new Color(0.28f, 0.84f, 0.90f, 1f);
                case ItemId.Apple: return new Color(0.85f, 0.16f, 0.16f, 1f);
                case ItemId.RawBeef: return new Color(0.72f, 0.24f, 0.24f, 1f);
                case ItemId.CookedBeef: return new Color(0.48f, 0.26f, 0.18f, 1f);
                default: return new Color(0.80f, 0.80f, 0.80f, 1f);
            }
        }
    }
}
