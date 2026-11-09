using System;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.World
{
    /// <summary>
    /// Chiến lược sinh địa hình phẳng (Flat Terrain Generator).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Design Pattern áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Strategy Pattern (Tiêu chí 9, 10, 11):</b> Thực thi <see cref="ITerrainGenerator"/> để cung cấp thuật toán sinh địa hình đơn giản phục vụ chế độ Sandbox và viết Unit Test tự động mà không cần tính toán nhiễu ngẫu nhiên.</item>
    ///   <item><b>Đóng gói &amp; Hợp lệ dữ liệu (Tiêu chí 13):</b> Kiểm tra <c>surfaceHeight</c> nằm trong khoảng từ 1 tới 63, giữ trường dữ liệu <c>private readonly</c>.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class FlatTerrainGenerator : ITerrainGenerator
    {
        private readonly int _surfaceHeight;

        /// <summary>Độ cao mặt cỏ bề mặt của địa hình phẳng.</summary>
        public int SurfaceHeight => _surfaceHeight;

        /// <summary>
        /// Khởi tạo chiến lược sinh địa hình phẳng với chiều cao bề mặt xác định.
        /// </summary>
        /// <param name="surfaceHeight">Độ cao mặt đất (mặc định = 16, phải nằm trong [1, 63]).</param>
        public FlatTerrainGenerator(int surfaceHeight = 16)
        {
            if (surfaceHeight < 1 || surfaceHeight >= Chunk.Height)
            {
                throw new ArgumentOutOfRangeException(nameof(surfaceHeight), $"Chiều cao mặt đất phẳng phải nằm trong khoảng [1, {Chunk.Height - 1}].");
            }

            _surfaceHeight = surfaceHeight;
        }

        /// <inheritdoc />
        public BlockId GetBlockAt(BlockPos worldPosition, int seed)
        {
            int y = worldPosition.Y;
            if (y == 0)
            {
                return BlockId.Bedrock;
            }

            if (y < _surfaceHeight)
            {
                return BlockId.Dirt;
            }

            if (y == _surfaceHeight)
            {
                return BlockId.Grass;
            }

            return BlockId.Air;
        }
    }
}
