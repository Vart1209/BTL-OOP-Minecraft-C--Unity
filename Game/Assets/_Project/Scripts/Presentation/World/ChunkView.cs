using System;
using System.Collections.Generic;
using UnityEngine;
using Minecraft.Domain.World;

namespace Minecraft.Presentation.World
{
    /// <summary>
    /// Thành phần hiển thị lưới 3D của một Chunk trong thế giới Unity (Presentation Layer).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc phân tầng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Phân tách giao diện và nghiệp vụ (Tiêu chí 4, 8):</b> Tầng Domain tính toán lưới thuần C# (<see cref="ChunkMeshData"/>), <see cref="ChunkView"/> ở tầng Presentation nhận dữ liệu và chuyển đổi sang <see cref="UnityEngine.Mesh"/> để GPU dựng hình.</item>
    ///   <item><b>Tối ưu hiệu năng:</b> Chỉ gán <see cref="MeshCollider.sharedMesh"/> cho những Chunk vừa cập nhật, tái sử dụng Mesh instance thay vì hủy và tạo mới liên tục.</item>
    /// </list>
    /// </para>
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public sealed class ChunkView : MonoBehaviour
    {
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;
        private Mesh _mesh;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();

            _mesh = new Mesh
            {
                name = "ChunkMesh"
            };
            _meshFilter.sharedMesh = _mesh;
        }

        /// <summary>
        /// Nhận dữ liệu hình học từ Domain và cập nhật lại lưới hiển thị cũng như Collider vật lý.
        /// </summary>
        /// <param name="meshData">Dữ liệu lưới thuần C# từ <see cref="ChunkMeshBuilder"/>.</param>
        public void RenderMesh(ChunkMeshData meshData)
        {
            if (meshData == null)
            {
                throw new ArgumentNullException(nameof(meshData));
            }

            if (_mesh == null)
            {
                _mesh = new Mesh { name = "ChunkMesh" };
                _meshFilter.sharedMesh = _mesh;
            }

            _mesh.Clear();

            int vertexCount = meshData.Vertices.Count;
            if (vertexCount == 0)
            {
                _meshCollider.sharedMesh = null;
                return;
            }

            Vector3[] unityVertices = new Vector3[vertexCount];
            Vector3[] unityNormals = new Vector3[vertexCount];
            Vector2[] unityUvs = new Vector2[vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                System.Numerics.Vector3 v = meshData.Vertices[i];
                unityVertices[i] = new Vector3(v.X, v.Y, v.Z);

                System.Numerics.Vector3 n = meshData.Normals[i];
                unityNormals[i] = new Vector3(n.X, n.Y, n.Z);

                System.Numerics.Vector2 uv = meshData.Uvs[i];
                unityUvs[i] = new Vector2(uv.X, uv.Y);
            }

            int[] unityTriangles = new int[meshData.Triangles.Count];
            for (int i = 0; i < meshData.Triangles.Count; i++)
            {
                unityTriangles[i] = meshData.Triangles[i];
            }

            _mesh.vertices = unityVertices;
            _mesh.triangles = unityTriangles;
            _mesh.normals = unityNormals;
            _mesh.uv = unityUvs;

            // Gán lại Collider để cập nhật bề mặt va chạm của nhân vật
            _meshCollider.sharedMesh = null;
            _meshCollider.sharedMesh = _mesh;
        }
    }
}

