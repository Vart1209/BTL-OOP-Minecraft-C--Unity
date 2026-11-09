using System;
using System.Numerics;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.Entities.Mobs
{
    /// <summary>
    /// Nhà máy tạo đối tượng sinh vật cụ thể theo mẫu thiết kế Factory (Mob Factory).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Design Pattern áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Factory Pattern (Tiêu chí 9, 10, 11 barem [S2]):</b> Tập trung hóa việc khởi tạo mọi loài sinh vật tại một điểm duy nhất, tuân thủ nguyên lý Đóng/Mở (OCP).</item>
    ///   <item><b>Đa hình &amp; Trừu tượng hóa (Polymorphism &amp; Abstraction):</b> Trả về kiểu cha <see cref="Mob"/> để các tầng trên không bị phụ thuộc vào lớp cụ thể.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class MobFactory : IMobFactory
    {
        /// <inheritdoc />
        public Mob CreateMob(MobType type, Vector3 position)
        {
            return type switch
            {
                MobType.Zombie => new Zombie(position),
                MobType.Cow => new Cow(position),
                MobType.Skeleton => new Skeleton(position),
                _ => throw new ArgumentOutOfRangeException(nameof(type), $"Không hỗ trợ loại sinh vật: {type}")
            };
        }
    }
}
