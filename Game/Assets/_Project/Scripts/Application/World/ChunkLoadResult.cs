using System.Collections.Generic;
using Minecraft.Domain.Contracts;

namespace Minecraft.Application.World
{
    /// <summary>
    /// Gói dữ liệu kết quả sau mỗi chu kỳ cập nhật tải/gỡ Chunk quanh người chơi.
    /// <para>
    /// <b>Nguyên lý OOP áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Đóng gói &amp; Bất biến (Encapsulation &amp; Immutability - Tiêu chí 13):</b> Khai báo <c>readonly struct</c> với các danh sách chỉ đọc <see cref="IReadOnlyList{T}"/>, cung cấp thông tin cho tầng Presentation mà không để lộ cơ chế lưu trữ nội bộ.</item>
    /// </list>
    /// </para>
    /// </summary>
    public readonly struct ChunkLoadResult
    {
        private readonly IReadOnlyList<ChunkCoord> _newlyLoaded;
        private readonly IReadOnlyList<ChunkCoord> _unloaded;

        /// <summary>Danh sách tọa độ các Chunk vừa được sinh mới/tải vào bộ nhớ.</summary>
        public IReadOnlyList<ChunkCoord> NewlyLoaded => _newlyLoaded;

        /// <summary>Danh sách tọa độ các Chunk vừa được gỡ khỏi bộ nhớ khi đi xa.</summary>
        public IReadOnlyList<ChunkCoord> Unloaded => _unloaded;

        /// <summary>
        /// Khởi tạo gói dữ liệu kết quả cập nhật Chunk.
        /// </summary>
        public ChunkLoadResult(IReadOnlyList<ChunkCoord> newlyLoaded, IReadOnlyList<ChunkCoord> unloaded)
        {
            _newlyLoaded = newlyLoaded ?? System.Array.Empty<ChunkCoord>();
            _unloaded = unloaded ?? System.Array.Empty<ChunkCoord>();
        }
    }
}

