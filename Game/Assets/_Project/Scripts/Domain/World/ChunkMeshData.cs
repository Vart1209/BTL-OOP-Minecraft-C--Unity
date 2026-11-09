using System;
using System.Collections.Generic;
using System.Numerics;

namespace Minecraft.Domain.World
{
    /// <summary>
    /// Gói dữ liệu hình học lưới Mesh 3D thuần C# (Vertices, Triangles, UVs, Normals).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Clean Architecture (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Độc lập công nghệ (Tiêu chí 4, 8):</b> Sử dụng <see cref="System.Numerics.Vector3"/> và <see cref="System.Numerics.Vector2"/> thay vì các kiểu của Unity, giúp tầng Domain độc lập 100% với UnityEngine.</item>
    ///   <item><b>Đóng gói (Encapsulation - Tiêu chí 13):</b> Dữ liệu lưới chỉ cho phép đọc thông qua <see cref="IReadOnlyList{T}"/>, tránh việc các tầng khác sửa đổi trực tiếp vào cấu trúc đỉnh.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class ChunkMeshData
    {
        private readonly List<Vector3> _vertices;
        private readonly List<int> _triangles;
        private readonly List<Vector2> _uvs;
        private readonly List<Vector3> _normals;

        /// <summary>Danh sách tọa độ các đỉnh của Mesh.</summary>
        public IReadOnlyList<Vector3> Vertices => _vertices;

        /// <summary>Danh sách chỉ số tam giác (mỗi 3 chỉ số tạo thành 1 tam giác).</summary>
        public IReadOnlyList<int> Triangles => _triangles;

        /// <summary>Danh sách tọa độ UV ánh xạ vào Texture Atlas.</summary>
        public IReadOnlyList<Vector2> Uvs => _uvs;

        /// <summary>Danh sách vector pháp tuyến (Normals) hỗ trợ tính toán đổ bóng.</summary>
        public IReadOnlyList<Vector3> Normals => _normals;

        /// <summary>
        /// Khởi tạo gói dữ liệu Mesh với danh sách bộ đệm được cung cấp.
        /// </summary>
        public ChunkMeshData(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, List<Vector3> normals)
        {
            _vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));
            _triangles = triangles ?? throw new ArgumentNullException(nameof(triangles));
            _uvs = uvs ?? throw new ArgumentNullException(nameof(uvs));
            _normals = normals ?? throw new ArgumentNullException(nameof(normals));
        }
    }
}
