using System;
using System.Collections.Generic;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.World;

namespace Minecraft.Application.World
{
    /// <summary>
    /// Dịch vụ ứng dụng điều phối Use Case UC02 (Sinh và khám phá Chunk theo vị trí người chơi).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc phân tầng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Application Service (Tiêu chí 4, 8):</b> Điều phối luồng sinh và giải phóng tài nguyên Chunk mà không chứa logic dựng hình trực tiếp của Unity.</item>
    ///   <item><b>Đóng gói trạng thái (State Encapsulation - Tiêu chí 13):</b> Quản lý tập hợp các Chunk đang nạp <c>_loadedChunks</c> dưới dạng <c>private readonly HashSet&lt;ChunkCoord&gt;</c>.</item>
    ///   <item><b>Tối ưu bộ nhớ:</b> Tự động gỡ các Chunk nằm ngoài bán kính tầm nhìn để tiết kiệm RAM.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class ChunkLoadService
    {
        private readonly Domain.World.World _world;
        private readonly int _renderDistanceInChunks;
        private readonly HashSet<ChunkCoord> _loadedChunks = new HashSet<ChunkCoord>();

        /// <summary>Danh sách các tọa độ Chunk hiện đang được tải trong bộ nhớ.</summary>
        public IReadOnlyCollection<ChunkCoord> LoadedChunks => _loadedChunks;

        /// <summary>Bán kính dựng hình tính bằng số Chunk.</summary>
        public int RenderDistanceInChunks => _renderDistanceInChunks;

        /// <summary>
        /// Khởi tạo dịch vụ tải Chunk.
        /// </summary>
        /// <param name="world">Thế giới Voxel mục tiêu.</param>
        /// <param name="renderDistanceInChunks">Bán kính tải Chunk (mặc định = 4 theo config.json).</param>
        public ChunkLoadService(Domain.World.World world, int renderDistanceInChunks = 4)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
            if (renderDistanceInChunks < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(renderDistanceInChunks), "Bán kính tải Chunk phải >= 1.");
            }

            _renderDistanceInChunks = renderDistanceInChunks;
        }

        /// <summary>
        /// Cập nhật các Chunk hoạt động quanh tọa độ Chunk trung tâm của người chơi.
        /// </summary>
        /// <param name="centerChunk">Tọa độ Chunk hiện tại của người chơi.</param>
        /// <returns>Đối tượng <see cref="ChunkLoadResult"/> chứa danh sách Chunk mới nạp và Chunk đã gỡ.</returns>
        public ChunkLoadResult UpdateActiveChunks(ChunkCoord centerChunk)
        {
            List<ChunkCoord> newlyLoaded = new List<ChunkCoord>();
            List<ChunkCoord> unloaded = new List<ChunkCoord>();

            HashSet<ChunkCoord> desiredChunks = new HashSet<ChunkCoord>();

            for (int dx = -_renderDistanceInChunks; dx <= _renderDistanceInChunks; dx++)
            {
                for (int dz = -_renderDistanceInChunks; dz <= _renderDistanceInChunks; dz++)
                {
                    ChunkCoord coord = new ChunkCoord(centerChunk.X + dx, centerChunk.Z + dz);
                    desiredChunks.Add(coord);

                    if (!_loadedChunks.Contains(coord))
                    {
                        _world.GenerateChunk(coord);
                        _loadedChunks.Add(coord);
                        newlyLoaded.Add(coord);
                    }
                }
            }

            // Gỡ bỏ các Chunk nằm ngoài bán kính cho phép (renderDistance + 1 để có vùng đệm trễ)
            int unloadThreshold = _renderDistanceInChunks + 1;
            List<ChunkCoord> toRemove = new List<ChunkCoord>();

            foreach (ChunkCoord loaded in _loadedChunks)
            {
                if (Math.Abs(loaded.X - centerChunk.X) > unloadThreshold ||
                    Math.Abs(loaded.Z - centerChunk.Z) > unloadThreshold)
                {
                    _world.UnloadChunk(loaded);
                    toRemove.Add(loaded);
                    unloaded.Add(loaded);
                }
            }

            foreach (ChunkCoord removed in toRemove)
            {
                _loadedChunks.Remove(removed);
            }

            return new ChunkLoadResult(newlyLoaded, unloaded);
        }
    }
}

