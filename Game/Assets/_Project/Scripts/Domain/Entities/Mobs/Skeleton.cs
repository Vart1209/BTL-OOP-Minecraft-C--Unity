using System.Numerics;

namespace Minecraft.Domain.Entities.Mobs
{
    /// <summary>
    /// Bộ xương (Skeleton) — Quái vật thù địch tấn công tầm xa (Bài mở rộng luyện thi vấn đáp).
    /// <para>
    /// <b>Nguyên lý OOP áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Kế thừa &amp; Mở rộng (Open-Closed Principle &amp; LSP - Tiêu chí 12, 14, 15):</b> Minh chứng khả năng thêm giống loài quái vật mới mà không phải sửa đổi kiến trúc hiện có.</item>
    ///   <item><b>Đa hình (Polymorphism):</b> Khác với Zombie lao vào đánh cận chiến, Skeleton tự động giữ khoảng cách cự ly bắn (6 block) để tấn công người chơi.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class Skeleton : Mob
    {
        public const int DefaultHealth = 20;
        public const float DefaultSpeed = 2.0f;
        public const int DefaultAttackDamage = 2;
        public const float DefaultDetectionRadius = 16.0f;
        public const float PreferredAttackRange = 6.0f;

        /// <summary>
        /// Khởi tạo Skeleton tại vị trí chỉ định.
        /// </summary>
        public Skeleton(Vector3 initialPosition)
            : base(
                initialPosition,
                maxHealth: DefaultHealth,
                type: MobType.Skeleton,
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

                    // Nếu quá xa cự ly bắn (> 6 block), di chuyển lại gần
                    if (distance > PreferredAttackRange)
                    {
                        Vector3 direction = Vector3.Normalize(playerPosition - Position);
                        MoveTo(Position + direction * (MoveSpeed * deltaTime));
                    }
                    // Nếu đã vào cự ly bắn (<= 6 block), dừng lại giữ khoảng cách bắn tên

                    return;
                }
            }

            SetState(MobState.Idle);
        }
    }
}
