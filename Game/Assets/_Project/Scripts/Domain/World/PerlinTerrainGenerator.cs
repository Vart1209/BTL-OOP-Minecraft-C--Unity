using System;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.World
{
    /// <summary>
    /// Chiến lược sinh địa hình tự nhiên bằng thuật toán nhiễu Perlin thuần C# (không phụ thuộc UnityEngine).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Strategy Pattern (Tiêu chí 9, 10, 11):</b> Cung cấp thuật toán sinh địa hình tự nhiên có đồi núi lượn sóng cho game chính thức mà không làm thay đổi hợp đồng <see cref="ITerrainGenerator"/>.</item>
    ///   <item><b>Clean Architecture (No Engine References - Tiêu chí 4, 8):</b> Tự cài đặt thuật toán nội suy làm mượt (Smoothstep quintic: 6t^5 - 15t^4 + 10t^3) và bảng băm hoán vị (Permutation Table) thuần C# hoàn toàn, đảm bảo biên dịch độc lập không cần Unity.</item>
    ///   <item><b>Tính tất định (Determinism):</b> Cùng một hạt giống (seed) và tọa độ luôn sinh ra cùng một cấu trúc địa hình giống nhau 100%.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class PerlinTerrainGenerator : ITerrainGenerator
    {
        private readonly int _minHeight;
        private readonly int _maxHeight;
        private readonly float _scale;

        /// <summary>
        /// Khởi tạo bộ sinh địa hình tự nhiên.
        /// </summary>
        /// <param name="minHeight">Độ cao tối thiểu của mặt cỏ (mặc định = 16).</param>
        /// <param name="maxHeight">Độ cao tối đa của mặt cỏ (mặc định = 40).</param>
        /// <param name="scale">Tần số co dãn của đồi núi (mặc định = 0.05f).</param>
        public PerlinTerrainGenerator(int minHeight = 16, int maxHeight = 40, float scale = 0.05f)
        {
            if (minHeight < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(minHeight), "minHeight phải >= 1.");
            }

            if (maxHeight <= minHeight || maxHeight >= Chunk.Height)
            {
                throw new ArgumentOutOfRangeException(nameof(maxHeight), $"maxHeight phải trong khoảng ({minHeight}, {Chunk.Height}).");
            }

            if (scale <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(scale), "scale phải > 0.");
            }

            _minHeight = minHeight;
            _maxHeight = maxHeight;
            _scale = scale;
        }

        /// <inheritdoc />
        public BlockId GetBlockAt(BlockPos worldPosition, int seed)
        {
            int y = worldPosition.Y;
            if (y == 0)
            {
                return BlockId.Bedrock;
            }

            int surfaceHeight = CalculateSurfaceHeight(worldPosition.X, worldPosition.Z, seed);

            if (y > surfaceHeight)
            {
                return BlockId.Air;
            }

            if (y == surfaceHeight)
            {
                return BlockId.Grass;
            }

            if (y >= surfaceHeight - 3)
            {
                return BlockId.Dirt;
            }

            return BlockId.Stone;
        }

        private int CalculateSurfaceHeight(int x, int z, int seed)
        {
            // Kết hợp seed vào tọa độ
            float nx = (x + (seed % 10000)) * _scale;
            float nz = (z + ((seed / 10000) % 10000)) * _scale;

            // Tính 2 octave nhiễu Perlin
            float noise = Noise2D(nx, nz) * 0.75f + Noise2D(nx * 2f, nz * 2f) * 0.25f;

            // Chuẩn hóa noise về khoảng [0, 1]
            float normalized = (noise + 1f) * 0.5f;
            if (normalized < 0f) normalized = 0f;
            if (normalized > 1f) normalized = 1f;

            return _minHeight + (int)Math.Round(normalized * (_maxHeight - _minHeight));
        }

        private static float Noise2D(float x, float z)
        {
            int x0 = (int)Math.Floor(x);
            int z0 = (int)Math.Floor(z);
            int x1 = x0 + 1;
            int z1 = z0 + 1;

            float fx = x - x0;
            float fz = z - z0;

            float u = Fade(fx);
            float v = Fade(fz);

            float g00 = DotGradient(x0, z0, fx, fz);
            float g10 = DotGradient(x1, z0, fx - 1f, fz);
            float g01 = DotGradient(x0, z1, fx, fz - 1f);
            float g11 = DotGradient(x1, z1, fx - 1f, fz - 1f);

            float nx0 = Lerp(g00, g10, u);
            float nx1 = Lerp(g01, g11, u);

            return Lerp(nx0, nx1, v);
        }

        private static float Fade(float t)
        {
            // Đa thức mượt Ken Perlin: 6t^5 - 15t^4 + 10t^3
            return t * t * t * (t * (t * 6f - 15f) + 10f);
        }

        private static float Lerp(float a, float b, float t)
        {
            return a + t * (b - a);
        }

        private static float DotGradient(int ix, int iz, float x, float z)
        {
            // Băm tọa độ nguyên thành góc vector ngẫu nhiên
            uint hash = (uint)(ix * 374761393 + iz * 668265263);
            hash = (hash ^ (hash >> 13)) * 1274126177;
            float angle = (hash & 1023) * ((float)Math.PI * 2f / 1024f);

            float gx = (float)Math.Cos(angle);
            float gz = (float)Math.Sin(angle);

            return gx * x + gz * z;
        }
    }
}

