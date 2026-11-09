using NUnit.Framework;
using Minecraft.Domain.Environment;

namespace Minecraft.Domain.Tests
{
    [TestFixture]
    public class DayNightCycleTests
    {
        [Test]
        public void DayNightCycle_InitializesAtDayPhase()
        {
            var cycle = new DayNightCycle(dayLengthSeconds: 100f, initialNormalizedTime: 0.1f);

            Assert.AreEqual(DayNightPhase.Day, cycle.CurrentPhase);
            Assert.IsFalse(cycle.IsNight);
            Assert.AreEqual(0.1f, cycle.NormalizedTime, 0.001f);
        }

        [Test]
        public void DayNightCycle_AdvancesThroughPhasesAndFiresObserverEvents()
        {
            var cycle = new DayNightCycle(dayLengthSeconds: 100f, initialNormalizedTime: 0.0f);

            DayNightPhase observedPhase = DayNightPhase.Day;
            int phaseChangeCount = 0;
            cycle.PhaseChanged += phase =>
            {
                observedPhase = phase;
                phaseChangeCount++;
            };

            // Tiến tới hoàng hôn (0.50)
            cycle.Tick(50f);
            Assert.AreEqual(DayNightPhase.Sunset, cycle.CurrentPhase);
            Assert.AreEqual(DayNightPhase.Sunset, observedPhase);
            Assert.IsFalse(cycle.IsNight);

            // Tiến tới ban đêm (0.70)
            cycle.Tick(20f);
            Assert.AreEqual(DayNightPhase.Night, cycle.CurrentPhase);
            Assert.AreEqual(DayNightPhase.Night, observedPhase);
            Assert.IsTrue(cycle.IsNight);

            // Tiến tới bình minh (0.95)
            cycle.Tick(25f);
            Assert.AreEqual(DayNightPhase.Sunrise, cycle.CurrentPhase);
            Assert.IsFalse(cycle.IsNight);

            // Hoàn tất 1 ngày quay về Ngày (1.05 -> 0.05)
            cycle.Tick(10f);
            Assert.AreEqual(DayNightPhase.Day, cycle.CurrentPhase);
            Assert.AreEqual(DayNightPhase.Day, observedPhase);
            Assert.GreaterOrEqual(phaseChangeCount, 4);
        }

        [Test]
        public void DayNightCycle_SetTime_AllowsDirectTimeSkipping()
        {
            var cycle = new DayNightCycle(dayLengthSeconds: 100f, initialNormalizedTime: 0.0f);

            cycle.SetTime(0.75f); // Đêm muộn
            Assert.AreEqual(DayNightPhase.Night, cycle.CurrentPhase);
            Assert.IsTrue(cycle.IsNight);

            cycle.SetTime(0.25f); // Giữa trưa
            Assert.AreEqual(DayNightPhase.Day, cycle.CurrentPhase);
            Assert.IsFalse(cycle.IsNight);
        }
    }
}
