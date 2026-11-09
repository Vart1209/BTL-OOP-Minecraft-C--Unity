using System;
using System.Collections.Generic;
using UnityEngine;
using Minecraft.Domain.Blocks;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.World;

namespace Minecraft.Presentation.World
{
    /// <summary>
    /// Lớp quản lý hiển thị thế giới Voxel trong Unity, kết nối giữa sự kiện Domain và các GameObject hiển thị Chunk.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc phân tầng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Observer Pattern (Tiêu chí 9, 10, 11):</b> Lắng nghe sự kiện <see cref="IWorldReader.BlockChanged"/> để tự động dựng lại Mesh cho Chunk bị thay đổi (và cả Chunk hàng xóm nếu khối nằm ở biên mép).</item>
    ///   <item><b>Quản lý vòng đời (Presentation Lifecycle):</b> Tự động sinh và hủy các GameObject <see cref="ChunkView"/> theo thông báo từ dịch vụ tải Chunk.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class WorldView : MonoBehaviour
    {
        private Domain.World.World _world;
        private BlockRegistry _blockRegistry;
        private ChunkMeshBuilder _meshBuilder;
        private Material _terrainMaterial;

        private readonly Dictionary<ChunkCoord, ChunkView> _activeViews = new Dictionary<ChunkCoord, ChunkView>();

        /// <summary>
        /// Khởi tạo phụ thuộc của WorldView từ GameBootstrap.
        /// </summary>
        public void Initialize(Domain.World.World world, BlockRegistry blockRegistry, Material terrainMaterial)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
            _blockRegistry = blockRegistry ?? throw new ArgumentNullException(nameof(blockRegistry));
            _terrainMaterial = terrainMaterial;
            _meshBuilder = new ChunkMeshBuilder(_world, _blockRegistry, atlasGridSize: 16);

            _world.BlockChanged += OnBlockChanged;
        }

        private void OnDestroy()
        {
            if (_world != null)
            {
                _world.BlockChanged -= OnBlockChanged;
            }
        }

        /// <summary>
        /// Tạo hoặc hiển thị ChunkView cho một tọa độ Chunk xác định.
        /// </summary>
        public void LoadChunkView(ChunkCoord coord)
        {
            if (_activeViews.ContainsKey(coord))
            {
                return;
            }

            if (!_world.TryGetChunk(coord, out Chunk chunk))
            {
                return;
            }

            GameObject chunkGo = new GameObject($"Chunk_{coord.X}_{coord.Z}");
            chunkGo.transform.SetParent(transform);
            chunkGo.transform.position = new Vector3(coord.X * Chunk.Width, 0f, coord.Z * Chunk.Width);

            MeshRenderer renderer = chunkGo.AddComponent<MeshRenderer>();
            if (_terrainMaterial != null)
            {
                renderer.sharedMaterial = _terrainMaterial;
            }

            ChunkView view = chunkGo.AddComponent<ChunkView>();
            _activeViews.Add(coord, view);

            RebuildChunkMesh(chunk, view);
        }

        /// <summary>
        /// Hủy bỏ ChunkView khi Chunk bị giải phóng khỏi tầm nhìn.
        /// </summary>
        public void UnloadChunkView(ChunkCoord coord)
        {
            if (_activeViews.TryGetValue(coord, out ChunkView view))
            {
                Destroy(view.gameObject);
                _activeViews.Remove(coord);
            }
        }

        private void OnBlockChanged(BlockChangedEventArgs args)
        {
            ChunkCoord centerCoord = args.Position.ToChunkCoord(Chunk.Width);
            if (_world.TryGetChunk(centerCoord, out Chunk centerChunk) && _activeViews.TryGetValue(centerCoord, out ChunkView centerView))
            {
                RebuildChunkMesh(centerChunk, centerView);
            }

            // Nếu khối thay đổi nằm ở mép biên, dựng lại Mesh cho Chunk kề để cập nhật mặt giáp ranh
            BlockPos local = args.Position.ToLocalPos(Chunk.Width);
            CheckAndRebuildNeighbor(local.X == 0, new ChunkCoord(centerCoord.X - 1, centerCoord.Z));
            CheckAndRebuildNeighbor(local.X == Chunk.Width - 1, new ChunkCoord(centerCoord.X + 1, centerCoord.Z));
            CheckAndRebuildNeighbor(local.Z == 0, new ChunkCoord(centerCoord.X, centerCoord.Z - 1));
            CheckAndRebuildNeighbor(local.Z == Chunk.Width - 1, new ChunkCoord(centerCoord.X, centerCoord.Z + 1));
        }

        private void CheckAndRebuildNeighbor(bool condition, ChunkCoord neighborCoord)
        {
            if (condition && _world.TryGetChunk(neighborCoord, out Chunk neighborChunk) && _activeViews.TryGetValue(neighborCoord, out ChunkView neighborView))
            {
                RebuildChunkMesh(neighborChunk, neighborView);
            }
        }

        private void RebuildChunkMesh(Chunk chunk, ChunkView view)
        {
            ChunkMeshData meshData = _meshBuilder.BuildMesh(chunk);
            view.RenderMesh(meshData);
            chunk.MarkClean();
        }
    }
}
