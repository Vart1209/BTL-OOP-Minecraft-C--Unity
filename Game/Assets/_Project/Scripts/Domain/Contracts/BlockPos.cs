using System;

namespace Minecraft.Domain.Contracts
{
    /// <summary>
    /// Kiểu giá trị bất biến (Immutable Value Object) biểu diễn tọa độ nguyên 3 chiều (X, Y, Z) của một khối (Block) trong thế giới Voxel.
    /// <para>
    /// <b>Nguyên lý OOP áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Độc lập công nghệ (Clean Architecture - Tiêu chí 4, 8):</b> Thay thế hoàn toàn <c>UnityEngine.Vector3Int</c> để tầng <c>Minecraft.Domain</c> không phụ thuộc vào Unity Engine.</item>
    ///   <item><b>Đóng gói &amp; Kiểm tra hợp lệ (Encapsulation &amp; Validation - Tiêu chí 13):</b> Giữ <c>_x</c>, <c>_y</c>, <c>_z</c> là <c>private readonly</c>; kiểm tra <c>chunkWidth &gt; 0</c> trước khi quy đổi tọa độ.</item>
    /// </list>
    /// </para>
    /// </summary>
    public readonly struct BlockPos : IEquatable<BlockPos>
    {
        private readonly int _x;
        private readonly int _y;
        private readonly int _z;

        /// <summary>
        /// Tọa độ nguyên theo trục ngang Đông - Tây (X).
        /// </summary>
        public int X => _x;

        /// <summary>
        /// Tọa độ nguyên theo trục độ cao (Y).
        /// </summary>
        public int Y => _y;

        /// <summary>
        /// Tọa độ nguyên theo trục ngang Bắc - Nam (Z).
        /// </summary>
        public int Z => _z;

        /// <summary>
        /// Tọa độ gốc (0, 0, 0).
        /// </summary>
        public static BlockPos Zero => new BlockPos(0, 0, 0);

        /// <summary>
        /// Khởi tạo một vị trí khối nguyên 3 chiều.
        /// </summary>
        public BlockPos(int x, int y, int z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        /// <summary>
        /// Tạo một tọa độ mới dịch chuyển từ tọa độ hiện tại một khoảng (dx, dy, dz).
        /// </summary>
        public BlockPos Offset(int dx, int dy, int dz)
        {
            return new BlockPos(_x + dx, _y + dy, _z + dz);
        }

        /// <summary>
        /// Quy đổi tọa độ khối toàn cục (World Position) sang tọa độ Chunk chứa khối đó.
        /// Sử dụng phép chia làm tròn xuống (<c>FloorDiv</c>) để xử lý chính xác cả vùng tọa độ âm.
        /// </summary>
        /// <param name="chunkWidth">Chiều rộng mỗi cạnh ngang của Chunk (mặc định = 16).</param>
        /// <exception cref="ArgumentOutOfRangeException">Ném ra nếu <paramref name="chunkWidth"/> nhỏ hơn hoặc bằng 0.</exception>
        public ChunkCoord ToChunkCoord(int chunkWidth = 16)
        {
            if (chunkWidth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(chunkWidth), "Kích thước chiều ngang của Chunk phải lớn hơn 0.");
            }

            return new ChunkCoord(FloorDiv(_x, chunkWidth), FloorDiv(_z, chunkWidth));
        }

        /// <summary>
        /// Quy đổi tọa độ khối toàn cục (World Position) sang tọa độ cục bộ bên trong Chunk (0 .. chunkWidth - 1).
        /// Đảm bảo không bao giờ trả về chỉ số mảng âm khi người chơi đi sang vùng tọa độ âm.
        /// </summary>
        /// <param name="chunkWidth">Chiều rộng mỗi cạnh ngang của Chunk (mặc định = 16).</param>
        /// <exception cref="ArgumentOutOfRangeException">Ném ra nếu <paramref name="chunkWidth"/> nhỏ hơn hoặc bằng 0.</exception>
        public BlockPos ToLocalPos(int chunkWidth = 16)
        {
            if (chunkWidth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(chunkWidth), "Kích thước chiều ngang của Chunk phải lớn hơn 0.");
            }

            return new BlockPos(FloorMod(_x, chunkWidth), _y, FloorMod(_z, chunkWidth));
        }

        private static int FloorDiv(int a, int b)
        {
            int quotient = a / b;
            int remainder = a % b;
            if (remainder != 0 && ((a < 0) ^ (b < 0)))
            {
                quotient--;
            }

            return quotient;
        }

        private static int FloorMod(int a, int b)
        {
            int mod = a % b;
            if (mod < 0)
            {
                mod += b;
            }

            return mod;
        }

        /// <inheritdoc />
        public bool Equals(BlockPos other)
        {
            return _x == other._x && _y == other._y && _z == other._z;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is BlockPos other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(_x, _y, _z);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"({_x}, {_y}, {_z})";
        }

        public static BlockPos operator +(BlockPos a, BlockPos b) => new BlockPos(a._x + b._x, a._y + b._y, a._z + b._z);

        public static BlockPos operator -(BlockPos a, BlockPos b) => new BlockPos(a._x - b._x, a._y - b._y, a._z - b._z);

        public static bool operator ==(BlockPos left, BlockPos right) => left.Equals(right);

        public static bool operator !=(BlockPos left, BlockPos right) => !left.Equals(right);
    }
}

