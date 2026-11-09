using System.Numerics;
using NUnit.Framework;
using Minecraft.Domain.Entities.Mobs;

namespace Minecraft.Domain.Tests
{
    [TestFixture]
    public class MobHierarchyTests
    {
        [Test]
        public void Zombie_AttributesAndInheritance_MatchesHostileMobContract()
        {
            var zombie = new Zombie(new Vector3(10f, 0f, 10f));

            Assert.AreEqual(MobType.Zombie, zombie.Type);
            Assert.IsTrue(zombie.IsHostile);
            Assert.AreEqual(20, zombie.Health.Current);
            Assert.AreEqual(3, zombie.AttackDamage);
            Assert.AreEqual(16f, zombie.DetectionRadius);
            Assert.AreEqual(MobState.Idle, zombie.CurrentState);
        }

        [Test]
        public void Zombie_AI_ChasesPlayerWhenWithinDetectionRadius()
        {
            var zombie = new Zombie(new Vector3(10f, 0f, 0f));
            var playerPos = new Vector3(15f, 0f, 0f); // Khoảng cách 5 block < 16 block

            // Tick AI với deltaTime 1 giây
            zombie.UpdateAI(1.0f, playerPos, isPlayerTargetable: true);

            Assert.AreEqual(MobState.Chase, zombie.CurrentState);
            // Zombie phải di chuyển về phía player (vị trí X tăng lên)
            Assert.Greater(zombie.Position.X, 10f);
        }

        [Test]
        public void Zombie_AI_IgnoresPlayerWhenOutOfDetectionRadius()
        {
            var zombie = new Zombie(new Vector3(0f, 0f, 0f));
            var farPlayerPos = new Vector3(50f, 0f, 0f); // Khoảng cách 50 block > 16 block

            zombie.UpdateAI(1.0f, farPlayerPos, isPlayerTargetable: true);

            Assert.AreNotEqual(MobState.Chase, zombie.CurrentState);
            Assert.AreEqual(0f, zombie.Position.X);
        }

        [Test]
        public void Cow_AttributesAndInheritance_MatchesPassiveMobContract()
        {
            var cow = new Cow(new Vector3(5f, 0f, 5f));

            Assert.AreEqual(MobType.Cow, cow.Type);
            Assert.IsFalse(cow.IsHostile);
            Assert.AreEqual(10, cow.Health.Current);
            Assert.AreEqual(0, cow.AttackDamage);

            // Cow không bao giờ đuổi người chơi
            cow.UpdateAI(1.0f, new Vector3(6f, 0f, 5f), isPlayerTargetable: true);
            Assert.AreNotEqual(MobState.Chase, cow.CurrentState);
        }

        [Test]
        public void Skeleton_AI_MaintainsRangedDistance()
        {
            // Bài mở rộng luyện thi vấn đáp
            var skeleton = new Skeleton(new Vector3(0f, 0f, 0f));
            Assert.AreEqual(MobType.Skeleton, skeleton.Type);
            Assert.IsTrue(skeleton.IsHostile);

            // Khi người chơi ở xa (10 block), skeleton tiến lại gần
            skeleton.UpdateAI(1.0f, new Vector3(10f, 0f, 0f), isPlayerTargetable: true);
            Assert.AreEqual(MobState.Chase, skeleton.CurrentState);
            Assert.Greater(skeleton.Position.X, 0f);

            // Khi người chơi đã ở cự ly bắn (5 block <= 6 block), skeleton dừng lại giữ khoảng cách
            skeleton.MoveTo(new Vector3(5f, 0f, 0f));
            skeleton.UpdateAI(1.0f, new Vector3(10f, 0f, 0f), isPlayerTargetable: true);
            Assert.AreEqual(5f, skeleton.Position.X, 0.01f);
        }
    }
}
