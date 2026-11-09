using System;
using System.Collections.Generic;

namespace Minecraft.Domain.Persistence
{
    /// <summary>
    /// Đối tượng DTO tổng thể chứa toàn bộ dữ liệu lưu trữ của một thế giới (Save File).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Data Transfer Object (DTO Pattern):</b> Tách biệt tuyệt đối giữa cấu trúc dữ liệu lưu trữ trên đĩa và các thực thể nghiệp vụ Domain (<see cref="World.World"/>, <see cref="Entities.Player"/>).</item>
    ///   <item><b>Tính độc lập:</b> Cho phép thay đổi định dạng lưu trữ (JSON, nhị phân, SQLite) mà không ảnh hưởng tới logic của thế giới.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class WorldSaveDto
    {
        public int WorldSeed { get; set; }
        public string SaveName { get; set; } = "World";
        public string SavedAtUtc { get; set; } = DateTime.UtcNow.ToString("o");
        public PlayerSaveDto Player { get; set; } = new PlayerSaveDto();
        public List<BlockChangeDto> ModifiedBlocks { get; set; } = new List<BlockChangeDto>();
    }
}
