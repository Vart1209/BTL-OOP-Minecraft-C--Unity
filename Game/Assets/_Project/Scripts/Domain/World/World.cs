using System;
using System.Collections.Generic;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.World
{
    /// <summary>
    /// Thực thể quản lý toàn bộ thế giới Voxel, bao gồm tập hợp các Chunk và quy tắc nghiệp vụ thế giới.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Quan hệ Hợp thành (Composition - Tiêu chí 6):</b> <c>World ◆ Chunk</c> — World quản lý vòng đời và lưu trữ các Chunk trong bảng băm <c>Dictionary&lt;ChunkCoord, Chunk&gt;</c>.</item>
    ///   <item><b>Thực thi nhiều Interface (Interface Realization - Tiêu chí 6):</b> World thực thi cả <see cref="IWorldReader"/> và <see cref="IWorldWriter"/>.</item>
    ///   <item><b>Observer Pattern (Tiêu chí 9, 10, 11):</b> Phát sự kiện <see cref="BlockChanged"/> mỗi khi một khối thay đổi thành công để các hệ thống hiển thị (Presentation) và âm thanh cập nhật.</item>
    ///   <item><b>Quy tắc nghiệp vụ R3 (Tiêu chí 3):</b> Kiểm soát biên chiều cao thế giới [0, MaxHeight), từ chối mọi thao tác ngoài biên.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class World : IWorldReader, IWorldWriter
    {
        private readonly Dictionary<ChunkCoord, Chunk> _chunks = new Dictionary<ChunkCoord, Chunk>();
        private readonly ITerrainGenerator _terrainGenerator;
        private readonly int _seed;
        private readonly int _maxHeight;

        /// <inheritdoc />
        public int MaxHeight => _maxHeight;

        /// <summary>Hạt giống ngẫu nhiên của thế giới.</summary>
        public int Seed => _seed;

        /// <inheritdoc />
        public event Action<BlockChangedEventArgs> BlockChanged;

        /// <summary>
        /// Khởi tạo thế giới với chiến lược sinh địa hình và hạt giống xác định.
        /// </summary>
        /// <param name="terrainGenerator">Chiến lược thuật toán sinh địa hình.</param>
        /// <param name="seed">Hạt giống sinh số ngẫu nhiên.</param>
        /// <param name="maxHeight">Chiều cao tối đa của thế giới (mặc định = 64).</param>
        public World(ITerrainGenerator terrainGenerator, int seed = 20261109, int maxHeight = Chunk.Height)
        {
            _terrainGenerator = terrainGenerator ?? throw new ArgumentNullException(nameof(terrainGenerator));
            _seed = seed;
            _maxHeight = maxHeight > 0 ? maxHeight : throw new ArgumentOutOfRangeException(nameof(maxHeight), "Chiều cao thế giới phải > 0.");
        }

        /// <inheritdoc />
        public bool IsInBounds(BlockPos position)
        {
            return position.Y >= 0 && position.Y < _maxHeight;
        }

        /// <summary>
        /// Sinh dữ liệu cho một Chunk mới nếu chưa tồn tại.
        /// </summary>
        /// <param name="coord">Tọa độ 2D của Chunk.</param>
        /// <returns>Đối tượng <see cref="Chunk"/> chứa dữ liệu khối vừa sinh.</returns>
        public Chunk GenerateChunk(ChunkCoord coord)
        {
            if (_chunks.TryGetValue(coord, out Chunk existing))
            {
                return existing;
            }

            Chunk chunk = new Chunk(coord);
            int startX = coord.X * Chunk.Width;
            int startZ = coord.Z * Chunk.Width;

            for (int x = 0; x < Chunk.Width; x++)
            {
                for (int z = 0; z < Chunk.Width; z++)
                {
                    int worldX = startX + x;
                    int worldZ = startZ + z;

                    for (int y = 0; y < _maxHeight; y++)
                    {
                        BlockPos pos = new BlockPos(worldX, y, worldZ);
                        BlockId block = _terrainGenerator.GetBlockAt(pos, _seed);
                        if (block != BlockId.Air)
                        {
                            chunk.SetBlock(x, y, z, block);
                        }
                    }
                }
            }

            chunk.MarkClean();
            _chunks.Add(coord, chunk);
            return chunk;
        }

        /// <summary>
        /// Thử lấy Chunk tại tọa độ xác định nếu Chunk đã được tải vào bộ nhớ.
        /// </summary>
        public bool TryGetChunk(ChunkCoord coord, out Chunk chunk)
        {
            return _chunks.TryGetValue(coord, out chunk);
        }

        /// <summary>
        /// Hủy tải Chunk khỏi bộ nhớ RAM khi người chơi đi ra xa.
        /// </summary>
        public bool UnloadChunk(ChunkCoord coord)
        {
            return _chunks.Remove(coord);
        }

        /// <inheritdoc />
        public BlockId GetBlock(BlockPos position)
        {
            if (!IsInBounds(position))
            {
                return BlockId.Air;
            }

            ChunkCoord coord = position.ToChunkCoord(Chunk.Width);
            if (_chunks.TryGetValue(coord, out Chunk chunk))
            {
                BlockPos local = position.ToLocalPos(Chunk.Width);
                return chunk.GetBlock(local.X, local.Y, local.Z);
            }

            return BlockId.Air;
        }

        /// <inheritdoc />
        public bool SetBlock(BlockPos position, BlockId blockId)
        {
            if (!IsInBounds(position))
            {
                return false;
            }

            ChunkCoord coord = position.ToChunkCoord(Chunk.Width);
            if (!_chunks.TryGetValue(coord, out Chunk chunk))
            {
                chunk = GenerateChunk(coord);
            }

            BlockPos local = position.ToLocalPos(Chunk.Width);
            BlockId oldBlock = chunk.GetBlock(local.X, local.Y, local.Z);
            if (oldBlock == blockId)
            {
                return false;
            }

            chunk.SetBlock(local.X, local.Y, local.Z, blockId);

            // Nếu khối nằm ở biên Chunk, đánh dấu Chunk kề bị bẩn để cập nhật lại mặt giáp ranh
            MarkNeighborChunkDirtyIfNeeded(coord, local);

            BlockChanged?.Invoke(new BlockChangedEventArgs(position, oldBlock, blockId));
            return true;
        }

        private void MarkNeighborChunkDirtyIfNeeded(ChunkCoord coord, BlockPos local)
        {
            if (local.X == 0 && _chunks.TryGetValue(new ChunkCoord(coord.X - 1, coord.Z), out Chunk westChunk))
            {
                westChunk.MarkDirty();
            }
            else if (local.X == Chunk.Width - 1 && _chunks.TryGetValue(new ChunkCoord(coord.X + 1, coord.Z), out Chunk eastChunk))
            {
                eastChunk.MarkDirty();
            }

            if (local.Z == 0 && _chunks.TryGetValue(new ChunkCoord(coord.X, coord.Z - 1), out Chunk southChunk))
            {
                southChunk.MarkDirty();
            }
            else if (local.Z == Chunk.Width - 1 && _chunks.TryGetValue(new ChunkCoord(coord.X, coord.Z + 1), out Chunk northChunk))
            {
                northChunk.MarkDirty();
            }
        }
    }
}
