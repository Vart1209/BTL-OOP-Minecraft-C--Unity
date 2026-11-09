using System;

namespace Minecraft.Domain.Contracts
{
    /// <summary>
    /// Kiểu giá trị bất biến (Immutable Value Object) biểu diễn tọa độ 2D (X, Z) của một Chunk trên mặt phẳng ngang.
    /// <para>
    /// <b>Nguyên lý OOP áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Đóng gói (Encapsulation):</b> Các trường dữ liệu <c>_x</c>, <c>_z</c> được khai báo <c>private readonly</c>, chỉ cho phép đọc qua Property <see cref="X"/> và <see cref="Z"/> để đảm bảo tính bất biến (Immutability) khi dùng làm khóa trong <c>Dictionary&lt;ChunkCoord, Chunk&gt;</c>.</item>
    ///   <item><b>Thực thi hợp đồng (Interface Realization):</b> Thực thi <see cref="IEquatable{ChunkCoord}"/> để tránh hiện tượng Boxing/Unboxing khi so sánh struct trong bộ sưu tập kiểu chung (Generic Collections).</item>
    /// </list>
    /// </para>
    /// </summary>
    public readonly struct ChunkCoord : IEquatable<ChunkCoord>
    {
        private readonly int _x;
        private readonly int _z;

        /// <summary>
        /// Tọa độ chỉ số Chunk theo trục Đông - Tây (X).
        /// </summary>
        public int X => _x;

        /// <summary>
        /// Tọa độ chỉ số Chunk theo trục Bắc - Nam (Z).
        /// </summary>
        public int Z => _z;

        /// <summary>
        /// Khởi tạo tọa độ của một Chunk mới.
        /// </summary>
        /// <param name="x">Chỉ số Chunk theo trục X.</param>
        /// <param name="z">Chỉ số Chunk theo trục Z.</param>
        public ChunkCoord(int x, int z)
        {
            _x = x;
            _z = z;
        }

        /// <summary>
        /// Kiểm tra hai tọa độ Chunk có trùng nhau hay không.
        /// </summary>
        public bool Equals(ChunkCoord other)
        {
            return _x == other._x && _z == other._z;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is ChunkCoord other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(_x, _z);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Chunk({_x}, {_z})";
        }

        public static bool operator ==(ChunkCoord left, ChunkCoord right) => left.Equals(right);

        public static bool operator !=(ChunkCoord left, ChunkCoord right) => !left.Equals(right);
    }
}

