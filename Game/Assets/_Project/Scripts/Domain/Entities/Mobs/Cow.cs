using System.Numerics;

namespace Minecraft.Domain.Entities.Mobs
{
    /// <summary>
    /// Bò sữa (Cow) — Sinh vật hiền lành, đi lang thang ăn cỏ trong thế giới.
    /// <para>
    /// <b>Nguyên lý OOP áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Kế thừa (Inheritance - Tiêu chí 6, 14):</b> <c>Cow : Mob : Entity</c> — Kế thừa các chỉ số sinh học nhưng không có hành vi tấn công.</item>
    ///   <item><b>Đa hình (Polymorphism - Tiêu chí 14):</b> Hành vi AI là đi dạo hòa bình (<see cref="MobState.Wander"/>), hoàn toàn không truy đuổi hay làm hại người chơi.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class Cow : Mob
    {
        public const int DefaultHealth = 10;
        public const float DefaultSpeed = 1.5f;

        /// <summary>
        /// Khởi tạo Bò tại vị trí chỉ định.
        /// </summary>
        public Cow(Vector3 initialPosition)
            : base(
                initialPosition,
                maxHealth: DefaultHealth,
                type: MobType.Cow,
                moveSpeed: DefaultSpeed,
                attackDamage: 0,
                detectionRadius: 8.0f,
                isHostile: false)
        {
        }

        /// <inheritdoc />
        public override void UpdateAI(float deltaTime, Vector3 playerPosition, bool isPlayerTargetable)
        {
            if (!IsAlive)
            {
                SetState(MobState.Idle);
                return;
            }

            // Sinh vật hiền lành chỉ chuyển trạng thái đi dạo (Wander), không bao giờ Chase người chơi
            SetState(MobState.Wander);
        }
    }
}
