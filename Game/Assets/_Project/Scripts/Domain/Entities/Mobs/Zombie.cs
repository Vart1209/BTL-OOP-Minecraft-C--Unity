using System.Numerics;

namespace Minecraft.Domain.Entities.Mobs
{
    /// <summary>
    /// Thây ma (Zombie) — Sinh vật thù địch xuất hiện vào ban đêm (Quy tắc R11).
    /// <para>
    /// <b>Nguyên lý OOP áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Kế thừa (Inheritance - Tiêu chí 6, 14):</b> <c>Zombie : Mob : Entity</c> — Kế thừa cấu trúc sinh vật và ghi đè phương thức <see cref="UpdateAI"/>.</item>
    ///   <item><b>Đa hình (Polymorphism - Tiêu chí 14):</b> Khi người chơi trong bán kính phát hiện 16 ô, chuyển trạng thái sang <see cref="MobState.Chase"/> và áp sát tấn công cận chiến.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class Zombie : Mob
    {
        public const int DefaultHealth = 20;
        public const float DefaultSpeed = 2.5f;
        public const int DefaultAttackDamage = 3;
        public const float DefaultDetectionRadius = 16.0f;

        /// <summary>
        /// Khởi tạo Zombie tại vị trí chỉ định.
        /// </summary>
        public Zombie(Vector3 initialPosition)
            : base(
                initialPosition,
                maxHealth: DefaultHealth,
                type: MobType.Zombie,
                moveSpeed: DefaultSpeed,
                attackDamage: DefaultAttackDamage,
                detectionRadius: DefaultDetectionRadius,
                isHostile: true)
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

            if (isPlayerTargetable)
            {
                float distance = Vector3.Distance(Position, playerPosition);
                if (distance <= DetectionRadius)
                {
                    SetState(MobState.Chase);

                    // Di chuyển áp sát người chơi nếu chưa chạm cự ly cận chiến
                    if (distance > 1.0f && distance > 0.001f)
                    {
                        Vector3 direction = Vector3.Normalize(playerPosition - Position);
                        MoveTo(Position + direction * (MoveSpeed * deltaTime));
                    }

                    return;
                }
            }

            SetState(MobState.Idle);
        }
    }
}
