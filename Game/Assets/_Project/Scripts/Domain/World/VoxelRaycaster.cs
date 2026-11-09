using System;
using System.Numerics;
using Minecraft.Domain.Blocks;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.World
{
    /// <summary>
    /// Thuật toán dò tia không gian Voxel 3D sử dụng giải thuật DDA (Digital Differential Analyzer - Amanatides &amp; Woo).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Độc lập công nghệ (Tiêu chí 4, 8):</b> Cài đặt thuật toán DDA thuần C# với <see cref="Vector3"/> của <c>System.Numerics</c>, không phụ thuộc vào hệ thống Physics hay Raycast của Unity.</item>
    ///   <item><b>Quy tắc R7 (Tiêu chí 3):</b> Giới hạn tầm với tối đa của người chơi (mặc định = 5.0 block), kiểm tra hợp lệ <c>maxDistance &gt; 0</c>.</item>
    ///   <item><b>Đóng gói phụ thuộc (Dependency Injection):</b> Nhận <see cref="IWorldReader"/> và <see cref="BlockRegistry"/> qua constructor.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class VoxelRaycaster
    {
        private readonly IWorldReader _worldReader;
        private readonly BlockRegistry _blockRegistry;

        /// <summary>
        /// Khởi tạo bộ dò tia Voxel.
        /// </summary>
        public VoxelRaycaster(IWorldReader worldReader, BlockRegistry blockRegistry)
        {
            _worldReader = worldReader ?? throw new ArgumentNullException(nameof(worldReader));
            _blockRegistry = blockRegistry ?? throw new ArgumentNullException(nameof(blockRegistry));
        }

        /// <summary>
        /// Bắn một tia nhìn từ vị trí gốc theo hướng chỉ định để tìm khối rắn đầu tiên va chạm.
        /// </summary>
        /// <param name="origin">Tọa độ gốc của tia nhìn.</param>
        /// <param name="direction">Vector hướng nhìn (sẽ được chuẩn hóa).</param>
        /// <param name="maxDistance">Tầm với tối đa (mặc định = 5.0 block theo Quy tắc R7).</param>
        /// <returns>Đối tượng <see cref="VoxelRaycastHit"/> chứa thông tin va chạm.</returns>
        public VoxelRaycastHit Raycast(Vector3 origin, Vector3 direction, float maxDistance = 5.0f)
        {
            if (maxDistance <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(maxDistance), "Tầm với dò tia phải > 0 (Quy tắc R7).");
            }

            if (direction == Vector3.Zero)
            {
                return VoxelRaycastHit.Miss;
            }

            direction = Vector3.Normalize(direction);

            int x = (int)Math.Floor(origin.X);
            int y = (int)Math.Floor(origin.Y);
            int z = (int)Math.Floor(origin.Z);

            int stepX = direction.X > 0 ? 1 : (direction.X < 0 ? -1 : 0);
            int stepY = direction.Y > 0 ? 1 : (direction.Y < 0 ? -1 : 0);
            int stepZ = direction.Z > 0 ? 1 : (direction.Z < 0 ? -1 : 0);

            float tDeltaX = direction.X != 0 ? Math.Abs(1.0f / direction.X) : float.PositiveInfinity;
            float tDeltaY = direction.Y != 0 ? Math.Abs(1.0f / direction.Y) : float.PositiveInfinity;
            float tDeltaZ = direction.Z != 0 ? Math.Abs(1.0f / direction.Z) : float.PositiveInfinity;

            float tMaxX = stepX > 0 ? ((x + 1) - origin.X) * tDeltaX : (stepX < 0 ? (origin.X - x) * tDeltaX : float.PositiveInfinity);
            float tMaxY = stepY > 0 ? ((y + 1) - origin.Y) * tDeltaY : (stepY < 0 ? (origin.Y - y) * tDeltaY : float.PositiveInfinity);
            float tMaxZ = stepZ > 0 ? ((z + 1) - origin.Z) * tDeltaZ : (stepZ < 0 ? (origin.Z - z) * tDeltaZ : float.PositiveInfinity);

            float distance = 0f;
            BlockPos currentPos = new BlockPos(x, y, z);
            BlockPos adjacentPos = currentPos;
            BlockFace hitFace = BlockFace.Top;

            // Kiểm tra ngay ô voxel tại điểm xuất phát
            if (IsSolidBlock(currentPos))
            {
                return new VoxelRaycastHit(true, currentPos, adjacentPos, hitFace, 0f);
            }

            while (distance <= maxDistance)
            {
                if (tMaxX < tMaxY && tMaxX < tMaxZ)
                {
                    if (tMaxX > maxDistance) break;
                    distance = tMaxX;
                    adjacentPos = new BlockPos(x, y, z);
                    hitFace = stepX > 0 ? BlockFace.West : BlockFace.East;
                    x += stepX;
                    tMaxX += tDeltaX;
                }
                else if (tMaxY < tMaxZ)
                {
                    if (tMaxY > maxDistance) break;
                    distance = tMaxY;
                    adjacentPos = new BlockPos(x, y, z);
                    hitFace = stepY > 0 ? BlockFace.Bottom : BlockFace.Top;
                    y += stepY;
                    tMaxY += tDeltaY;
                }
                else
                {
                    if (tMaxZ > maxDistance) break;
                    distance = tMaxZ;
                    adjacentPos = new BlockPos(x, y, z);
                    hitFace = stepZ > 0 ? BlockFace.South : BlockFace.North;
                    z += stepZ;
                    tMaxZ += tDeltaZ;
                }

                currentPos = new BlockPos(x, y, z);
                if (IsSolidBlock(currentPos))
                {
                    return new VoxelRaycastHit(true, currentPos, adjacentPos, hitFace, distance);
                }
            }

            return VoxelRaycastHit.Miss;
        }

        private bool IsSolidBlock(BlockPos pos)
        {
            if (!_worldReader.IsInBounds(pos))
            {
                return false;
            }

            BlockId blockId = _worldReader.GetBlock(pos);
            if (blockId == BlockId.Air)
            {
                return false;
            }

            if (_blockRegistry.TryGet(blockId, out BlockType type))
            {
                return type.IsSolid;
            }

            return false;
        }
    }
}

