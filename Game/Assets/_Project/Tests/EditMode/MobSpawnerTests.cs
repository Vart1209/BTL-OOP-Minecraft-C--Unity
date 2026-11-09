using System.Numerics;
using NUnit.Framework;
using Minecraft.Application.Survival;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.Entities.Mobs;
using Minecraft.Domain.Environment;

namespace Minecraft.Domain.Tests
{
    [TestFixture]
    public class MobSpawnerTests
    {
        private IMobFactory _factory;
        private DayNightCycle _cycle;

        [SetUp]
        public void SetUp()
        {
            _factory = new MobFactory();
            _cycle = new DayNightCycle(dayLengthSeconds: 100f, initialNormalizedTime: 0.1f); // Ban ngày
        }

        [Test]
        public void MobFactory_CreatesCorrectInstancesBasedOnMobType()
        {
            var zombie = _factory.CreateMob(MobType.Zombie, new Vector3(1f, 0f, 1f));
            var cow = _factory.CreateMob(MobType.Cow, new Vector3(2f, 0f, 2f));
            var skeleton = _factory.CreateMob(MobType.Skeleton, new Vector3(3f, 0f, 3f));

            Assert.IsInstanceOf<Zombie>(zombie);
            Assert.IsInstanceOf<Cow>(cow);
            Assert.IsInstanceOf<Skeleton>(skeleton);
            Assert.AreEqual(new Vector3(1f, 0f, 1f), zombie.Position);
        }

        [Test]
        public void MobSpawnerService_RuleR11_CannotSpawnZombieDuringDay()
        {
            var spawner = new MobSpawnerService(_factory, _cycle, maxHostileMobs: 5, minSpawnDistance: 24f);

            var playerPos = new Vector3(0f, 0f, 0f);
            var spawnPos = new Vector3(30f, 0f, 0f); // Khoảng cách 30 >= 24

            // Hiện tại là ban ngày -> Không được phép spawn Zombie
            Assert.IsFalse(_cycle.IsNight);
            bool spawned = spawner.TrySpawnZombie(playerPos, spawnPos, out Mob zombie);

            Assert.IsFalse(spawned);
            Assert.IsNull(zombie);
            Assert.AreEqual(0, spawner.ActiveMobs.Count);
        }

        [Test]
        public void MobSpawnerService_RuleR11_CannotSpawnZombieCloserThan24Blocks()
        {
            var spawner = new MobSpawnerService(_factory, _cycle, maxHostileMobs: 5, minSpawnDistance: 24f);

            // Chuyển sang ban đêm
            _cycle.SetTime(0.70f);
            Assert.IsTrue(_cycle.IsNight);

            var playerPos = new Vector3(0f, 0f, 0f);
            var closeSpawnPos = new Vector3(10f, 0f, 0f); // Khoảng cách 10 < 24 block

            bool spawned = spawner.TrySpawnZombie(playerPos, closeSpawnPos, out Mob zombie);

            Assert.IsFalse(spawned);
            Assert.IsNull(zombie);
        }

        [Test]
        public void MobSpawnerService_RuleR11_SpawnsZombieAtNightWhenDistanceValid()
        {
            var spawner = new MobSpawnerService(_factory, _cycle, maxHostileMobs: 5, minSpawnDistance: 24f);

            // Chuyển sang ban đêm
            _cycle.SetTime(0.70f);
            Assert.IsTrue(_cycle.IsNight);

            var playerPos = new Vector3(0f, 0f, 0f);
            var validSpawnPos = new Vector3(25f, 0f, 0f); // Khoảng cách 25 >= 24 block

            bool spawned = spawner.TrySpawnZombie(playerPos, validSpawnPos, out Mob zombie);

            Assert.IsTrue(spawned);
            Assert.IsNotNull(zombie);
            Assert.AreEqual(MobType.Zombie, zombie.Type);
            Assert.AreEqual(1, spawner.ActiveMobs.Count);
        }

        [Test]
        public void MobSpawnerService_EnforcesMobCapacityLimit()
        {
            var spawner = new MobSpawnerService(_factory, _cycle, maxHostileMobs: 2, minSpawnDistance: 24f);
            _cycle.SetTime(0.70f); // Đêm

            var playerPos = new Vector3(0f, 0f, 0f);

            // Spawn 2 con đạt giới hạn
            Assert.IsTrue(spawner.TrySpawnZombie(playerPos, new Vector3(25f, 0f, 0f), out _));
            Assert.IsTrue(spawner.TrySpawnZombie(playerPos, new Vector3(30f, 0f, 0f), out _));

            // Con thứ 3 bị từ chối do vượt giới hạn
            bool thirdSpawned = spawner.TrySpawnZombie(playerPos, new Vector3(35f, 0f, 0f), out Mob third);
            Assert.IsFalse(thirdSpawned);
            Assert.IsNull(third);
            Assert.AreEqual(2, spawner.ActiveMobs.Count);
        }
    }
}
