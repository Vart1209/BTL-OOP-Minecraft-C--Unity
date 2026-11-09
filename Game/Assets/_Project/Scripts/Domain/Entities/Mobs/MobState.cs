namespace Minecraft.Domain.Entities.Mobs
{
    /// <summary>
    /// Các trạng thái hành vi AI của sinh vật (Mob AI State Machine).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Design Pattern áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>State Pattern (Tiêu chí 9, 10, 11):</b> Định nghĩa các trạng thái rời rạc (Đứng yên, Đi dạo, Đuổi mục tiêu) giúp quản lý logic AI sáng sủa, không lồng ghép switch lộn xộn.</item>
    /// </list>
    /// </para>
    /// </summary>
    public enum MobState
    {
        /// <summary>Đứng yên quan sát môi trường.</summary>
        Idle = 0,

        /// <summary>Đi dạo tự do không mục đích.</summary>
        Wander = 1,

        /// <summary>Phát hiện mục tiêu và truy đuổi.</summary>
        Chase = 2
    }
}
