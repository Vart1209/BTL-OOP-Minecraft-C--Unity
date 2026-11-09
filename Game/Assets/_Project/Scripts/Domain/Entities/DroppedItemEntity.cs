using System;
using System.Numerics;
using Minecraft.Domain.Items;

namespace Minecraft.Domain.Entities
{
    /// <summary>
    /// Thực thể đại diện cho một ngăn xếp vật phẩm đang rơi trên mặt đất trong thế giới 3D.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Kế thừa &amp; Đa hình (Inheritance &amp; Polymorphism - Tiêu chí 6, 14):</b> <c>DroppedItemEntity : Entity</c> — Mở rộng cây phân cấp kế thừa từ <see cref="Entity"/> bên cạnh <see cref="Player"/> và <c>Mob</c>. Ghi đè phương thức <see cref="Tick"/> để tự đếm ngược thời gian biến mất (despawn).</item>
    ///   <item><b>Quan hệ Hợp thành (Composition - Tiêu chí 6):</b> Sở hữu ngăn xếp vật phẩm <see cref="Payload"/> và quản lý vòng đời tồn tại trên mặt đất.</item>
    ///   <item><b>Đóng gói &amp; Kiểm tra hợp lệ (Encapsulation - Tiêu chí 13):</b> Từ chối khởi tạo nếu <c>payload</c> rỗng hoặc thời gian tồn tại không dương.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class DroppedItemEntity : Entity
    {
        /// <summary>Thời gian tồn tại mặc định của vật phẩm rơi trên mặt đất (300 giây = 5 phút theo chuẩn Minecraft).</summary>
        public const float DefaultDespawnTimeSeconds = 300f;

        private readonly ItemStack _payload;
        private readonly float _despawnTimeSeconds;
        private float _elapsedSeconds;

        /// <summary>Ngăn xếp vật phẩm kèm số lượng đang nằm trên mặt đất.</summary>
        public ItemStack Payload => _payload;

        /// <summary>Tổng thời gian tối đa trước khi vật phẩm tự biến mất (giây).</summary>
        public float DespawnTimeSeconds => _despawnTimeSeconds;

        /// <summary>Thời gian đã trôi qua kể từ lúc vật phẩm rơi ra (giây).</summary>
        public float ElapsedSeconds => _elapsedSeconds;

        /// <summary>Vật phẩm đã hết thời gian tồn tại trên mặt đất hay chưa.</summary>
        public bool IsExpired => _elapsedSeconds >= _despawnTimeSeconds || _payload.IsEmpty;

        /// <summary>
        /// Khởi tạo thực thể vật phẩm rơi tại vị trí chỉ định.
        /// </summary>
        /// <param name="position">Vị trí rơi trong không gian 3 chiều.</param>
        /// <param name="payload">Ngăn xếp vật phẩm rơi ra (không được rỗng).</param>
        /// <param name="despawnTimeSeconds">Thời gian tồn tại trước khi tự hủy (mặc định 300 giây).</param>
        public DroppedItemEntity(
            Vector3 position,
            ItemStack payload,
            float despawnTimeSeconds = DefaultDespawnTimeSeconds)
            : base(position, maxHealth: 1)
        {
            if (payload == null || payload.IsEmpty)
            {
                throw new ArgumentException("Vật phẩm rơi trên mặt đất (Payload) không được rỗng.", nameof(payload));
            }

            if (despawnTimeSeconds <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(despawnTimeSeconds), "Thời gian biến mất (DespawnTimeSeconds) phải lớn hơn 0.");
            }

            _payload = new ItemStack(payload.Definition, payload.Count);
            _despawnTimeSeconds = despawnTimeSeconds;
            _elapsedSeconds = 0f;
        }

        /// <summary>
        /// Đánh dấu thực thể vật phẩm đã được người chơi thu thập hoàn toàn.
        /// </summary>
        public void MarkCollected()
        {
            if (!_payload.IsEmpty)
            {
                _payload.Remove(_payload.Count);
            }

            if (IsAlive)
            {
                Health.TakeDamage(Health.Max, null);
            }
        }

        /// <inheritdoc />
        public override void Tick(float deltaTime)
        {
            if (!IsAlive || deltaTime <= 0f)
            {
                return;
            }

            _elapsedSeconds += deltaTime;
            if (IsExpired)
            {
                Health.TakeDamage(Health.Max, null);
            }
        }
    }
}
