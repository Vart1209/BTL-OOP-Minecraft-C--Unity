using System.Numerics;
using NUnit.Framework;
using Minecraft.Application.Survival;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.Entities;
using Minecraft.Domain.Entities.Mobs;
using Minecraft.Domain.GameModes;

namespace Minecraft.Domain.Tests
{
    [TestFixture]
    public class MobCombatAndGameModeServiceTests
    {
        private MobCombatService _combatService;
        private GameModeService _gameModeService;

        [SetUp]
        public void SetUp()
        {
            _combatService = new MobCombatService(attackRange: 2.0f);
            _gameModeService = new GameModeService();
        }

        [Test]
        public void MobCombat_ZombieAttacksPlayerInSurvival_ReducesHealth()
        {
            var player = new Player(new Vector3(0f, 0f, 0f), new SurvivalMode(), maxHealth: 20);
            var zombie = new Zombie(new Vector3(1f, 0f, 0f)); // Khoảng cách 1.0 < 2.0

            bool hit = _combatService.TryMobAttackPlayer(zombie, player);

            Assert.IsTrue(hit);
            // Sát thương Zombie = 3 -> Máu người chơi còn 17
            Assert.AreEqual(17, player.Health.Current);
        }

        [Test]
        public void MobCombat_ZombieAttacksPlayerInCreative_PlayerTakesZeroDamage_RuleR12()
        {
            var player = new Player(new Vector3(0f, 0f, 0f), new CreativeMode(), maxHealth: 20);
            var zombie = new Zombie(new Vector3(1f, 0f, 0f));

            bool hit = _combatService.TryMobAttackPlayer(zombie, player);

            Assert.IsTrue(hit);
            // Quy tắc R12: Creative miễn nhiễm sát thương -> Máu vẫn nguyên 20
            Assert.AreEqual(20, player.Health.Current);
        }

        [Test]
        public void MobCombat_ZombieCannotAttackPlayerWhenTooFar()
        {
            var player = new Player(new Vector3(0f, 0f, 0f), new SurvivalMode(), maxHealth: 20);
            var zombie = new Zombie(new Vector3(5f, 0f, 0f)); // Khoảng cách 5.0 > 2.0

            bool hit = _combatService.TryMobAttackPlayer(zombie, player);

            Assert.IsFalse(hit);
            Assert.AreEqual(20, player.Health.Current);
        }

        [Test]
        public void MobCombat_PlayerAttacksMob_DamagesAndKillsMob()
        {
            var player = new Player(new Vector3(0f, 0f, 0f), new SurvivalMode());
            var cow = new Cow(new Vector3(1.5f, 0f, 0f)); // Máu ban đầu 10

            // Người chơi dùng vũ khí dame 5
            bool hit1 = _combatService.TryPlayerAttackMob(player, cow, weaponDamage: 5);
            Assert.IsTrue(hit1);
            Assert.AreEqual(5, cow.Health.Current);
            Assert.IsTrue(cow.IsAlive);

            // Đánh tiếp đòn thứ 2 dame 5 -> Máu về 0 -> Bò chết
            bool hit2 = _combatService.TryPlayerAttackMob(player, cow, weaponDamage: 5);
            Assert.IsTrue(hit2);
            Assert.AreEqual(0, cow.Health.Current);
            Assert.IsFalse(cow.IsAlive);
        }

        [Test]
        public void GameModeService_SwitchesGameModesSeamlessly()
        {
            var player = new Player(new Vector3(0f, 0f, 0f), new SurvivalMode());
            Assert.AreEqual("Survival", player.CurrentGameMode.ModeName);

            // Chuyển sang Creative
            _gameModeService.SwitchToCreative(player);
            Assert.AreEqual("Creative", player.CurrentGameMode.ModeName);
            Assert.IsFalse(player.CurrentGameMode.CanTakeDamage);

            // Chuyển sang Adventure (Bài mở rộng)
            _gameModeService.SwitchToAdventure(player);
            Assert.AreEqual("Adventure", player.CurrentGameMode.ModeName);
            Assert.IsFalse(player.CurrentGameMode.CanBreakBlock(BlockId.Stone));

            // Chuyển lại Survival
            _gameModeService.SwitchToSurvival(player);
            Assert.AreEqual("Survival", player.CurrentGameMode.ModeName);
        }
    }
}
