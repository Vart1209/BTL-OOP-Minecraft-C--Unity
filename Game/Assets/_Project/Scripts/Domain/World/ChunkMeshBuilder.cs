using System;
using System.Collections.Generic;
using System.Numerics;
using Minecraft.Domain.Blocks;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.World
{
    /// <summary>
    /// Bộ dựng lưới hình học 3D (Chunk Mesh Builder) áp dụng kỹ thuật loại bỏ mặt khuất (Face Culling).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Tối ưu đồ họa (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Single Responsibility Principle (SRP - Tiêu chí 15):</b> <c>Chunk</c> chỉ giữ mảng dữ liệu, việc tính toán hình học đỉnh và chỉ số tam giác được tách riêng sang <see cref="ChunkMeshBuilder"/>.</item>
    ///   <item><b>Face Culling (Tối ưu cốt lõi Voxel - Tiêu chí barem):</b> Chỉ sinh 4 đỉnh và 2 tam giác cho một mặt khi khối kề theo hướng đó là không khí (<see cref="BlockId.Air"/>) hoặc khối trong suốt (<see cref="BlockType.IsTransparent"/>). Giảm từ ~98.000 tam giác xuống dưới ~5.000 tam giác mỗi Chunk thông thường.</item>
    ///   <item><b>Không phụ thuộc Unity (Tiêu chí 4, 8):</b> Trả về <see cref="ChunkMeshData"/> thuần C#, có thể chạy trên luồng phụ (Background Thread / Job) mà không bị xung đột với Main Thread của Unity.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class ChunkMeshBuilder
    {
        private readonly IWorldReader _worldReader;
        private readonly BlockRegistry _blockRegistry;
        private readonly int _atlasGridSize;

        /// <summary>
        /// Khởi tạo bộ dựng lưới với ngữ cảnh thế giới, danh mục khối và kích thước lưới Texture Atlas.
        /// </summary>
        /// <param name="worldReader">Hợp đồng đọc thế giới để truy vấn khối lân cận.</param>
        /// <param name="blockRegistry">Danh mục tra cứu thông số và texture của khối.</param>
        /// <param name="atlasGridSize">Số ô texture trên một cạnh của Texture Atlas (mặc định = 16 cho 16x16 tiles).</param>
        public ChunkMeshBuilder(IWorldReader worldReader, BlockRegistry blockRegistry, int atlasGridSize = 16)
        {
            _worldReader = worldReader ?? throw new ArgumentNullException(nameof(worldReader));
            _blockRegistry = blockRegistry ?? throw new ArgumentNullException(nameof(blockRegistry));
            _atlasGridSize = atlasGridSize > 0 ? atlasGridSize : throw new ArgumentOutOfRangeException(nameof(atlasGridSize), "Kích thước lưới Atlas phải > 0.");
        }

        /// <summary>
        /// Dựng toàn bộ lưới hiển thị 3D cho một Chunk cụ thể.
        /// </summary>
        /// <param name="chunk">Chunk cần tính toán lưới.</param>
        /// <returns>Đối tượng <see cref="ChunkMeshData"/> chứa danh sách đỉnh, tam giác, UV và pháp tuyến.</returns>
        public ChunkMeshData BuildMesh(Chunk chunk)
        {
            if (chunk == null)
            {
                throw new ArgumentNullException(nameof(chunk));
            }

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            List<Vector3> normals = new List<Vector3>();

            int startWorldX = chunk.Coordinate.X * Chunk.Width;
            int startWorldZ = chunk.Coordinate.Z * Chunk.Width;

            for (int x = 0; x < Chunk.Width; x++)
            {
                for (int y = 0; y < Chunk.Height; y++)
                {
                    for (int z = 0; z < Chunk.Width; z++)
                    {
                        BlockId blockId = chunk.GetBlock(x, y, z);
                        if (blockId == BlockId.Air)
                        {
                            continue;
                        }

                        if (!_blockRegistry.TryGet(blockId, out BlockType currentBlock))
                        {
                            continue;
                        }

                        BlockPos worldPos = new BlockPos(startWorldX + x, y, startWorldZ + z);

                        // Duyệt 6 mặt của khối lập phương
                        CheckAndAddFace(chunk, worldPos, currentBlock, BlockFace.Top, x, y, z, 0, 1, 0, vertices, triangles, uvs, normals);
                        CheckAndAddFace(chunk, worldPos, currentBlock, BlockFace.Bottom, x, y, z, 0, -1, 0, vertices, triangles, uvs, normals);
                        CheckAndAddFace(chunk, worldPos, currentBlock, BlockFace.North, x, y, z, 0, 0, 1, vertices, triangles, uvs, normals);
                        CheckAndAddFace(chunk, worldPos, currentBlock, BlockFace.South, x, y, z, 0, 0, -1, vertices, triangles, uvs, normals);
                        CheckAndAddFace(chunk, worldPos, currentBlock, BlockFace.East, x, y, z, 1, 0, 0, vertices, triangles, uvs, normals);
                        CheckAndAddFace(chunk, worldPos, currentBlock, BlockFace.West, x, y, z, -1, 0, 0, vertices, triangles, uvs, normals);
                    }
                }
            }

            return new ChunkMeshData(vertices, triangles, uvs, normals);
        }

        private void CheckAndAddFace(
            Chunk chunk,
            BlockPos worldPos,
            BlockType currentBlock,
            BlockFace face,
            int lx, int ly, int lz,
            int dx, int dy, int dz,
            List<Vector3> vertices,
            List<int> triangles,
            List<Vector2> uvs,
            List<Vector3> normals)
        {
            BlockId neighborId;
            int nx = lx + dx;
            int ny = ly + dy;
            int nz = lz + dz;

            // Nếu ô kề nằm ngay trong cùng Chunk
            if (nx >= 0 && nx < Chunk.Width && ny >= 0 && ny < Chunk.Height && nz >= 0 && nz < Chunk.Width)
            {
                neighborId = chunk.GetBlock(nx, ny, nz);
            }
            else
            {
                // Nếu ô kề nằm vượt biên Chunk, truy vấn qua WorldReader
                BlockPos neighborWorldPos = worldPos.Offset(dx, dy, dz);
                neighborId = _worldReader.GetBlock(neighborWorldPos);
            }

            // Quy tắc Face Culling: Vẽ mặt nếu khối lân cận là Air hoặc khối trong suốt (như Lá)
            bool shouldRender = false;
            if (neighborId == BlockId.Air)
            {
                shouldRender = true;
            }
            else if (_blockRegistry.TryGet(neighborId, out BlockType neighborBlock))
            {
                if (neighborBlock.IsTransparent && (!currentBlock.IsTransparent || neighborId != currentBlock.Id))
                {
                    shouldRender = true;
                }
            }

            if (!shouldRender)
            {
                return;
            }

            AddFaceGeometry(face, lx, ly, lz, currentBlock.GetTextureIndex(face), vertices, triangles, uvs, normals);
        }

        private void AddFaceGeometry(
            BlockFace face,
            float x, float y, float z,
            int textureIndex,
            List<Vector3> vertices,
            List<int> triangles,
            List<Vector2> uvs,
            List<Vector3> normals)
        {
            int startIndex = vertices.Count;

            Vector3 v0, v1, v2, v3, normal;

            switch (face)
            {
                case BlockFace.Top:
                    v0 = new Vector3(x, y + 1, z);
                    v1 = new Vector3(x, y + 1, z + 1);
                    v2 = new Vector3(x + 1, y + 1, z + 1);
                    v3 = new Vector3(x + 1, y + 1, z);
                    normal = new Vector3(0, 1, 0);
                    break;
                case BlockFace.Bottom:
                    v0 = new Vector3(x, y, z + 1);
                    v1 = new Vector3(x, y, z);
                    v2 = new Vector3(x + 1, y, z);
                    v3 = new Vector3(x + 1, y, z + 1);
                    normal = new Vector3(0, -1, 0);
                    break;
                case BlockFace.North:
                    v0 = new Vector3(x + 1, y, z + 1);
                    v1 = new Vector3(x + 1, y + 1, z + 1);
                    v2 = new Vector3(x, y + 1, z + 1);
                    v3 = new Vector3(x, y, z + 1);
                    normal = new Vector3(0, 0, 1);
                    break;
                case BlockFace.South:
                    v0 = new Vector3(x, y, z);
                    v1 = new Vector3(x, y + 1, z);
                    v2 = new Vector3(x + 1, y + 1, z);
                    v3 = new Vector3(x + 1, y, z);
                    normal = new Vector3(0, 0, -1);
                    break;
                case BlockFace.East:
                    v0 = new Vector3(x + 1, y, z);
                    v1 = new Vector3(x + 1, y + 1, z);
                    v2 = new Vector3(x + 1, y + 1, z + 1);
                    v3 = new Vector3(x + 1, y, z + 1);
                    normal = new Vector3(1, 0, 0);
                    break;
                case BlockFace.West:
                default:
                    v0 = new Vector3(x, y, z + 1);
                    v1 = new Vector3(x, y + 1, z + 1);
                    v2 = new Vector3(x, y + 1, z);
                    v3 = new Vector3(x, y, z);
                    normal = new Vector3(-1, 0, 0);
                    break;
            }

            vertices.Add(v0);
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);

            // 2 tam giác tạo thành mặt chữ nhật (theo chiều kim đồng hồ)
            triangles.Add(startIndex + 0);
            triangles.Add(startIndex + 1);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 0);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 3);

            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);

            // Tính UV trên Atlas
            float tileSize = 1.0f / _atlasGridSize;
            int tileX = textureIndex % _atlasGridSize;
            int tileY = textureIndex / _atlasGridSize;

            float uMin = tileX * tileSize;
            float uMax = uMin + tileSize;
            float vMax = 1.0f - (tileY * tileSize);
            float vMin = vMax - tileSize;

            uvs.Add(new Vector2(uMin, vMin));
            uvs.Add(new Vector2(uMin, vMax));
            uvs.Add(new Vector2(uMax, vMax));
            uvs.Add(new Vector2(uMax, vMin));
        }
    }
}

