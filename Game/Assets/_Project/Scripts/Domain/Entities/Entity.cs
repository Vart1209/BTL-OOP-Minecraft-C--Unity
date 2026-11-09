using System;
using System.Numerics;

namespace Minecraft.Domain.Entities
{
    /// <summary>
    /// Lớp thực thể cơ sở trừu tượng (Abstract Base Class) cho mọi sinh vật sống và người chơi trong thế giới.
    /// <para>
    /// <b>Nguyên lý OOP áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Kế thừa &amp; Trừu tượng hóa (Inheritance &amp; Abstraction - Tiêu chí 14):</b> Cung cấp các thuộc tính chung (<see cref="Position"/>, <see cref="Health"/>) và phương thức thuần ảo <see cref="Tick"/> cho các lớp con kế thừa (<see cref="Player"/>, Mob).</item>
    ///   <item><b>Đóng gói (Encapsulation - Tiêu chí 13):</b> <c>Position</c> chỉ được cập nhật qua phương thức <see cref="MoveTo"/> có kiểm soát.</item>
    /// </list>
    /// </para>
    /// </summary>
    public abstract class Entity
    {
        private Vector3 _position;
        private readonly Health _health;

        /// <summary>Tọa độ thực của thực thể trong không gian 3 chiều.</summary>
        public Vector3 Position => _position;

        /// <summary>Hệ thống máu của thực thể.</summary>
        public Health Health => _health;

        /// <summary>Thực thể còn sống hay không.</summary>
        public bool IsAlive => _health.IsAlive;

        /// <summary>
        /// Khởi tạo thực thể với vị trí xuất phát và lượng máu tối đa.
        /// </summary>
        protected Entity(Vector3 initialPosition, int maxHealth)
        {
            _position = initialPosition;
            _health = new Health(maxHealth);
        }

        /// <summary>
        /// Di chuyển thực thể đến vị trí mới.
        /// </summary>
        public void MoveTo(Vector3 newPosition)
        {
            _position = newPosition;
        }

        /// <summary>
        /// Phương thức cập nhật logic nội tại của thực thể theo thời gian.
        /// </summary>
        public abstract void Tick(float deltaTime);
    }
}

