using System;
using System.Numerics;
using Minecraft.Domain.Entities;
using Minecraft.Domain.Entities.Mobs;

namespace Minecraft.Application.Survival
{
    /// <summary>
    /// Dịch vụ ứng dụng xử lý tương tác chiến đấu giữa người chơi và quái vật (Use Case UC08).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Quy tắc nghiệp vụ áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Single Responsibility Principle (SRP - Tiêu chí 16):</b> Chuyên trách tính toán khoảng cách va chạm, gây sát thương và kết liễu thực thể.</item>
    ///   <item><b>Phân quyền tác nhân (Quy tắc R12, Tiêu chí 4):</b> Khi người chơi ở chế độ Creative, nhận sát thương từ Mob sẽ bị chặn hoàn toàn (<c>CanTakeDamage == false</c>).</item>
    ///   <item><b>Quy tắc R9:</b> Nếu đòn đánh của Mob làm máu người chơi về 0, sự kiện tử vong được kích hoạt để đưa người chơi về điểm hồi sinh.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class MobCombatService
    {
        private readonly float _attackRange;

        /// <summary>Cự ly tối đa để đòn đánh cận chiến có hiệu lực.</summary>
        public float AttackRange => _attackRange;

        /// <summary>
        /// Khởi tạo dịch vụ chiến đấu với tầm đánh chỉ định (mặc định 2.0 block).
        /// </summary>
        public MobCombatService(float attackRange = 2.0f)
        {
            if (attackRange <= 0f) throw new ArgumentOutOfRangeException(nameof(attackRange));
            _attackRange = attackRange;
        }

        /// <summary>
        /// Quái vật thực hiện đòn tấn công cận chiến vào người chơi.
        /// </summary>
        public bool TryMobAttackPlayer(Mob mob, Player player)
        {
            if (mob == null || player == null)
            {
                return false;
            }

            if (!mob.IsAlive || !player.IsAlive)
            {
                return false;
            }

            float distance = Vector3.Distance(mob.Position, player.Position);
            if (distance > _attackRange)
            {
                return false;
            }

            // Gây sát thương xét theo chế độ chơi của người chơi (Quy tắc R12)
            player.Health.TakeDamage(mob.AttackDamage, player.CurrentGameMode);
            return true;
        }

        /// <summary>
        /// Người chơi thực hiện đòn tấn công vào một sinh vật (Mob).
        /// </summary>
        public bool TryPlayerAttackMob(Player player, Mob targetMob, int weaponDamage)
        {
            if (player == null || targetMob == null)
            {
                return false;
            }

            if (!player.IsAlive || !targetMob.IsAlive)
            {
                return false;
            }

            float distance = Vector3.Distance(player.Position, targetMob.Position);
            if (distance > 3.5f) // Tầm đánh tay/vũ khí của người chơi
            {
                return false;
            }

            targetMob.Health.TakeDamage(weaponDamage);
            return true;
        }
    }
}
