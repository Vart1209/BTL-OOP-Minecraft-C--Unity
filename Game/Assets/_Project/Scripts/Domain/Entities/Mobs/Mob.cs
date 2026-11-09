using System;
using System.Numerics;

namespace Minecraft.Domain.Entities.Mobs
{
    /// <summary>
    /// Lớp cơ sở trừu tượng cho mọi sinh vật (Mob) trong thế giới trò chơi.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Kế thừa &amp; Trừu tượng hóa (Inheritance &amp; Abstraction - Tiêu chí 6, 14):</b> <c>Mob : Entity</c> — Kế thừa vị trí và máu từ <see cref="Entity"/>, đồng thời bổ sung các thuộc tính sinh vật (<see cref="Type"/>, <see cref="MoveSpeed"/>, <see cref="AttackDamage"/>, <see cref="DetectionRadius"/>).</item>
    ///   <item><b>Đa hình (Polymorphism - Tiêu chí 14):</b> Phương thức trừu tượng <see cref="UpdateAI"/> cho phép mỗi loài (<see cref="Zombie"/>, <see cref="Cow"/>, <see cref="Skeleton"/>) tự định nghĩa hành vi nhận biết và di chuyển hoàn toàn khác biệt.</item>
    ///   <item><b>Liskov Substitution Principle (LSP - Tiêu chí 15):</b> Các hệ thống quản lý (Spawner, Combat) hoàn toàn có thể tương tác với danh sách kiểu cha <see cref="Mob"/> mà không cần ép kiểu cụ thể.</item>
    /// </list>
    /// </para>
    /// </summary>
    public abstract class Mob : Entity
    {
        private readonly MobType _type;
        private readonly float _moveSpeed;
        private readonly int _attackDamage;
        private readonly float _detectionRadius;
        private readonly bool _isHostile;
        private MobState _currentState;

        /// <summary>Chủng loại sinh vật.</summary>
        public MobType Type => _type;

        /// <summary>Tốc độ di chuyển cơ bản (block/giây).</summary>
        public float MoveSpeed => _moveSpeed;

        /// <summary>Lực sát thương khi tấn công người chơi.</summary>
        public int AttackDamage => _attackDamage;

        /// <summary>Bán kính phát hiện mục tiêu người chơi (block).</summary>
        public float DetectionRadius => _detectionRadius;

        /// <summary>Xác định sinh vật có phải quái vật thù địch hay không.</summary>
        public bool IsHostile => _isHostile;

        /// <summary>Trạng thái hành vi AI hiện tại của sinh vật.</summary>
        public MobState CurrentState => _currentState;

        /// <summary>
        /// Khởi tạo thực thể sinh vật với các chỉ số sinh tồn và hành vi.
        /// </summary>
        protected Mob(
            Vector3 initialPosition,
            int maxHealth,
            MobType type,
            float moveSpeed,
            int attackDamage,
            float detectionRadius,
            bool isHostile)
            : base(initialPosition, maxHealth)
        {
            if (moveSpeed < 0f) throw new ArgumentOutOfRangeException(nameof(moveSpeed));
            if (attackDamage < 0) throw new ArgumentOutOfRangeException(nameof(attackDamage));
            if (detectionRadius < 0f) throw new ArgumentOutOfRangeException(nameof(detectionRadius));

            _type = type;
            _moveSpeed = moveSpeed;
            _attackDamage = attackDamage;
            _detectionRadius = detectionRadius;
            _isHostile = isHostile;
            _currentState = MobState.Idle;
        }

        /// <summary>
        /// Thiết lập trạng thái hành vi mới cho sinh vật.
        /// </summary>
        public void SetState(MobState newState)
        {
            _currentState = newState;
        }

        /// <summary>
        /// Cập nhật logic trí tuệ nhân tạo (AI) của sinh vật theo thời gian và môi trường.
        /// </summary>
        /// <param name="deltaTime">Thời gian trôi qua giữa các khung hình (giây).</param>
        /// <param name="playerPosition">Vị trí hiện tại của người chơi trong thế giới.</param>
        /// <param name="isPlayerTargetable">Xác định người chơi có thể bị nhắm tới hay không (ví dụ: Survival thì có, Creative thì không bị quái đuổi).</param>
        public abstract void UpdateAI(float deltaTime, Vector3 playerPosition, bool isPlayerTargetable);

        /// <inheritdoc />
        public override void Tick(float deltaTime)
        {
            // Mặc định sinh vật gọi tick cơ bản
        }
    }
}
