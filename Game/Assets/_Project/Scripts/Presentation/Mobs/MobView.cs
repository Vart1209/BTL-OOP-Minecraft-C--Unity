using UnityEngine;
using Minecraft.Domain.Entities.Mobs;

namespace Minecraft.Presentation.Mobs
{
    /// <summary>
    /// Thành phần hiển thị trực quan cho một sinh vật (Mob View) trong không gian Unity 3D.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Mô hình MVC / MVP (Presentation Layer - Tiêu chí 4):</b> <c>MobView</c> chỉ là lớp hiển thị, lắng nghe sự thay đổi của thực thể <see cref="Mob"/> tầng Domain và đồng bộ Transform.</item>
    ///   <item><b>Đa hình (Polymorphism):</b> Gắn kết với lớp cha trừu tượng <see cref="Mob"/>, có thể hiển thị bất kỳ loài nào (<see cref="Zombie"/>, <see cref="Cow"/>, <see cref="Skeleton"/>).</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class MobView : MonoBehaviour
    {
        private Mob _mobModel;

        /// <summary>Thực thể dữ liệu Domain liên kết với View này.</summary>
        public Mob MobModel => _mobModel;

        /// <summary>
        /// Khởi tạo và liên kết View với đối tượng sinh vật Domain.
        /// </summary>
        public void Initialize(Mob mobModel)
        {
            _mobModel = mobModel;
            if (_mobModel != null)
            {
                transform.position = new UnityEngine.Vector3(_mobModel.Position.X, _mobModel.Position.Y, _mobModel.Position.Z);
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

            // Cập nhật vị trí mượt mà theo Domain
            var targetPos = new UnityEngine.Vector3(_mobModel.Position.X, _mobModel.Position.Y, _mobModel.Position.Z);
            transform.position = UnityEngine.Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 10f);
        }
    }
}
