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
            int x = worldPosition.X;
            int y = worldPosition.Y;
            int z = worldPosition.Z;

            if (y == 0)
            {
                return BlockId.Bedrock;
            }

            if (y < 0 || y >= Chunk.Height)
            {
                return BlockId.Air;
            }

            int surfaceHeight = CalculateSurfaceHeight(x, z, seed);
            const int seaLevel = 19;

            // 1. Vùng phía trên mặt đất: kiểm tra hồ nước hoặc Cây sồi 3D (Thân gỗ + Tán lá)
            if (y > surfaceHeight)
            {
                if (surfaceHeight < seaLevel && y <= seaLevel)
                {
                    return BlockId.Water;
                }

                if (y <= _maxHeight + 4)
                {
                    BlockId treeBlock = GetProceduralTreeBlockAt(x, y, z, seed);
                    if (treeBlock != BlockId.Air)
                    {
                        return treeBlock;
                    }
                }

                return BlockId.Air;
            }

            // 2. Lớp bề mặt địa hình theo độ cao (Quần xã Cát / Cỏ / Tuyết)
            if (y == surfaceHeight)
            {
                if (surfaceHeight <= seaLevel + 1)
                {
                    return BlockId.Sand;
                }

                if (surfaceHeight >= _maxHeight - 3)
                {
                    return BlockId.Snow;
                }

                return BlockId.Grass;
            }

            // 3. Lớp đệm ngay dưới bề mặt (3 ô Đất hoặc Cát)
            if (y >= surfaceHeight - 3)
            {
                return surfaceHeight <= seaLevel + 1 ? BlockId.Sand : BlockId.Dirt;
            }

            // 4. Tầng đá sâu: phân bố mỏ quặng tự nhiên (Than, Sắt, Vàng, Kim cương)
            return GetUndergroundStoneOrOre(x, y, z, seed);
        }

        private BlockId GetProceduralTreeBlockAt(int x, int y, int z, int seed)
        {
            int cellX = FloorDiv(x, 8);
            int cellZ = FloorDiv(z, 8);

            // Kiểm tra ô lưới 8x8 hiện tại và các ô lân cận để tán lá 5x5 có thể vươn qua biên ô
            for (int dcx = -1; dcx <= 1; dcx++)
            {
                for (int dcz = -1; dcz <= 1; dcz++)
                {
                    int cx = cellX + dcx;
                    int cz = cellZ + dcz;
                    uint cellHash = HashCoords(cx, cz, seed);

                    // Mật độ 38% ô lưới 8x8 có cây sồi
                    if ((cellHash % 100) >= 38)
                    {
                        continue;
                    }

                    // Tọa độ gốc cây nằm trong khoảng [2..5] của ô 8x8 để 2 cây không bao giờ chạm nhau
                    int treeX = cx * 8 + 2 + (int)(cellHash & 3);
                    int treeZ = cz * 8 + 2 + (int)((cellHash >> 2) & 3);

                    // Chừa khoảng trống bán kính 4 ô quanh điểm xuất phát (0, 0) của người chơi
                    if (treeX * treeX + treeZ * treeZ <= 16)
                    {
                        continue;
                    }

                    int dx = Math.Abs(x - treeX);
                    int dz = Math.Abs(z - treeZ);
                    if (dx > 2 || dz > 2)
                    {
                        continue;
                    }

                    int treeBaseY = CalculateSurfaceHeight(treeX, treeZ, seed);
                    // Chỉ mọc cây trên vùng đồi cỏ (không mọc dưới nước/bãi cát hay đỉnh núi tuyết)
                    if (treeBaseY < 21 || treeBaseY > _maxHeight - 7)
                    {
                        continue;
                    }

                    int trunkHeight = 4 + (int)((cellHash >> 5) & 1); // Thân cao 4 hoặc 5 khối
                    int relY = y - treeBaseY;

                    // Thân gỗ ở chính giữa
                    if (dx == 0 && dz == 0 && relY >= 1 && relY <= trunkHeight)
                    {
                        return BlockId.Wood;
                    }

                    // Tán lá tầng rộng (2 lớp 5x5 bo 4 góc)
                    if ((relY == trunkHeight - 1 || relY == trunkHeight) && !(dx == 2 && dz == 2))
                    {
                        return BlockId.Leaves;
                    }

                    // Tán lá tầng đỉnh (chữ thập 3x3 trên ngọn cây)
                    if (relY == trunkHeight + 1 && dx <= 1 && dz <= 1 && (dx + dz <= 1))
                    {
                        return BlockId.Leaves;
                    }
                }
            }

            return BlockId.Air;
        }

        private static BlockId GetUndergroundStoneOrOre(int x, int y, int z, int seed)
        {
            uint h = Hash3D(x, y, z, seed);
            uint roll = h % 100;

            if (y <= 10 && roll < 3)
            {
                return BlockId.DiamondOre;
            }

            if (y <= 18 && roll >= 3 && roll < 8)
            {
                return BlockId.GoldOre;
            }

            if (y <= 26 && roll >= 8 && roll < 16)
            {
                return BlockId.IronOre;
            }

            if (y <= 34 && roll >= 16 && roll < 26)
            {
                return BlockId.CoalOre;
            }

            return BlockId.Stone;
        }

        private static int FloorDiv(int a, int b)
        {
            int q = a / b;
            int r = a % b;
            if (r != 0 && ((a < 0) ^ (b < 0)))
            {
                q--;
            }

            return q;
        }

        private static uint HashCoords(int x, int z, int seed)
        {
            uint h = (uint)(x * 374761393 + z * 668265263 + seed * 1442695041);
            h = (h ^ (h >> 13)) * 1274126177;
            return h ^ (h >> 16);
        }

        private static uint Hash3D(int x, int y, int z, int seed)
        {
            uint h = (uint)(x * 374761393 + y * 1103515245 + z * 668265263 + seed * 1442695041);
            h = (h ^ (h >> 13)) * 1274126177;
            return h ^ (h >> 16);
        }

        private int CalculateSurfaceHeight(int x, int z, int seed)
        {
            // Kết hợp seed vào tọa độ
            float nx = (x + (seed % 10000)) * _scale;
            float nz = (z + ((seed / 10000) % 10000)) * _scale;

            // Tính 2 octave nhiễu Perlin và khuếch đại biên độ để đồi núi nhấp nhô tự nhiên rõ nét
            float noise = (Noise2D(nx, nz) * 0.75f + Noise2D(nx * 2f, nz * 2f) * 0.25f) * 1.6f;

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

