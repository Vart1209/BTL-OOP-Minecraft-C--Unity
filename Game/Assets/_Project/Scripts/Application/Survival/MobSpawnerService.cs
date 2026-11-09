using System;
using System.Collections.Generic;
using System.Numerics;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.Entities.Mobs;
using Minecraft.Domain.Environment;

namespace Minecraft.Application.Survival
{
    /// <summary>
    /// Dịch vụ ứng dụng điều phối việc sinh sản và quản lý vòng đời của các sinh vật trong thế giới (Use Case UC12).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Quy tắc nghiệp vụ (Quy tắc R11 - dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Dependency Injection (DIP - Tiêu chí 16):</b> Nhận <see cref="IMobFactory"/> và <see cref="DayNightCycle"/> qua constructor.</item>
    ///   <item><b>Thực thi Quy tắc R11 (Tiêu chí 3):</b>
    ///     <list type="number">
    ///       <item>Zombie chỉ được sinh vào ban đêm (<see cref="DayNightCycle.IsNight"/> == true).</item>
    ///       <item>Vị trí sinh phải cách vị trí người chơi tối thiểu 24 block.</item>
    ///       <item>Có giới hạn số lượng tối đa (<see cref="MaxHostileMobs"/>) để đảm bảo hiệu năng.</item>
    ///     </list>
    ///   </item>
    ///   <item><b>Quản lý vòng đời:</b> Tự động thu hồi và dọn dẹp các sinh vật đã chết (<c>!mob.IsAlive</c>) ra khỏi danh sách kích hoạt.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class MobSpawnerService
    {
        private readonly IMobFactory _mobFactory;
        private readonly DayNightCycle _dayNightCycle;
        private readonly int _maxHostileMobs;
        private readonly float _minSpawnDistance;
        private readonly List<Mob> _activeMobs = new List<Mob>();

        /// <summary>Số lượng quái vật thù địch tối đa cho phép tồn tại cùng lúc.</summary>
        public int MaxHostileMobs => _maxHostileMobs;

        /// <summary>Khoảng cách tối thiểu từ vị trí sinh tới người chơi (Quy tắc R11: >= 24 block).</summary>
        public float MinSpawnDistance => _minSpawnDistance;

        /// <summary>Danh sách các sinh vật đang hoạt động trong thế giới.</summary>
        public IReadOnlyList<Mob> ActiveMobs => _activeMobs;

        /// <summary>
        /// Khởi tạo dịch vụ sinh quái vật với các tham số nghiệp vụ.
        /// </summary>
        public MobSpawnerService(
            IMobFactory mobFactory,
            DayNightCycle dayNightCycle,
            int maxHostileMobs = 10,
            float minSpawnDistance = 24.0f)
        {
            _mobFactory = mobFactory ?? throw new ArgumentNullException(nameof(mobFactory));
            _dayNightCycle = dayNightCycle ?? throw new ArgumentNullException(nameof(dayNightCycle));

            if (maxHostileMobs <= 0) throw new ArgumentOutOfRangeException(nameof(maxHostileMobs));
            if (minSpawnDistance <= 0f) throw new ArgumentOutOfRangeException(nameof(minSpawnDistance));

            _maxHostileMobs = maxHostileMobs;
            _minSpawnDistance = minSpawnDistance;
        }

        /// <summary>
        /// Thử nghiệm sinh một Zombie tại tọa độ ứng viên tuân theo Quy tắc R11.
        /// </summary>
        public bool TrySpawnZombie(Vector3 playerPosition, Vector3 candidateSpawnPosition, out Mob spawnedZombie)
        {
            spawnedZombie = null;
            CleanupDeadMobs();

            // Quy tắc R11.1: Zombie chỉ sinh vào ban đêm
            if (!_dayNightCycle.IsNight)
            {
                return false;
            }

            // Quy tắc R11.2: Cách người chơi tối thiểu 24 block
            float distance = Vector3.Distance(playerPosition, candidateSpawnPosition);
            if (distance < _minSpawnDistance)
            {
                return false;
            }

            // Quy tắc R11.3: Giới hạn số lượng tối đa
            if (CountHostileMobs() >= _maxHostileMobs)
            {
                return false;
            }

            spawnedZombie = _mobFactory.CreateMob(MobType.Zombie, candidateSpawnPosition);
            _activeMobs.Add(spawnedZombie);
            return true;
        }

        /// <summary>
        /// Thử nghiệm sinh một sinh vật hiền lành (Bò) tại tọa độ chỉ định.
        /// </summary>
        public bool TrySpawnPassiveMob(Vector3 candidateSpawnPosition, out Mob spawnedMob)
        {
            CleanupDeadMobs();
            spawnedMob = _mobFactory.CreateMob(MobType.Cow, candidateSpawnPosition);
            _activeMobs.Add(spawnedMob);
            return true;
        }

        /// <summary>
        /// Cập nhật trí tuệ nhân tạo (AI) cho toàn bộ sinh vật đang hoạt động.
        /// </summary>
        public void Tick(float deltaTime, Vector3 playerPosition, bool isPlayerTargetable)
        {
            CleanupDeadMobs();

            for (int i = 0; i < _activeMobs.Count; i++)
            {
                var mob = _activeMobs[i];
                if (mob.IsAlive)
                {
                    mob.UpdateAI(deltaTime, playerPosition, isPlayerTargetable);
                }
            }
        }

        /// <summary>
        /// Đếm số lượng quái vật thù địch đang còn sống.
        /// </summary>
        public int CountHostileMobs()
        {
            int count = 0;
            for (int i = 0; i < _activeMobs.Count; i++)
            {
                if (_activeMobs[i].IsHostile && _activeMobs[i].IsAlive)
                {
                    count++;
                }
            }
            return count;
        }

        private void CleanupDeadMobs()
        {
            _activeMobs.RemoveAll(mob => !mob.IsAlive);
        }
    }
}
