using System.Numerics;
using Minecraft.Domain.Entities.Mobs;

namespace Minecraft.Domain.Contracts
{
    /// <summary>
    /// Hợp đồng nhà máy sản xuất sinh vật (Mob Factory Contract).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Design Pattern áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Factory Pattern (Tiêu chí 9, 10, 11 barem [S2]):</b> Che giấu chi tiết khởi tạo đối tượng cụ thể (<see cref="Zombie"/>, <see cref="Cow"/>, <see cref="Skeleton"/>), tạo thực thể theo mã định danh <see cref="MobType"/>.</item>
    /// </list>
    /// </para>
    /// </summary>
    public interface IMobFactory
    {
        /// <summary>
        /// Tạo mới một đối tượng sinh vật theo chủng loại và tọa độ xuất phát.
        /// </summary>
        /// <param name="type">Loại sinh vật cần tạo.</param>
        /// <param name="position">Tọa độ không gian ban đầu.</param>
        /// <returns>Đối tượng sinh vật thuộc lớp con tương ứng của <see cref="Mob"/>.</returns>
        Mob CreateMob(MobType type, Vector3 position);
    }
}
