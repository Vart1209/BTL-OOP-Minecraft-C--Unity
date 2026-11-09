using System.Collections.Generic;

namespace Minecraft.Domain.Configuration
{
    /// <summary>
    /// Cấu hình thế giới voxel (World Configuration).
    /// </summary>
    public sealed class WorldConfig
    {
        public int ChunkWidth { get; set; } = 16;
        public int ChunkHeight { get; set; } = 64;
        public int RenderDistanceInChunks { get; set; } = 4;
        public int DefaultSeed { get; set; } = 20261107;
    }

    /// <summary>
    /// Cấu hình thuộc tính người chơi (Player Configuration).
    /// </summary>
    public sealed class PlayerConfig
    {
        public int MaxHealth { get; set; } = 20;
        public int MaxHunger { get; set; } = 20;
        public float ReachDistance { get; set; } = 5.0f;
        public float WalkSpeed { get; set; } = 4.5f;
        public float JumpForce { get; set; } = 6.0f;
    }

    /// <summary>
    /// Cấu hình gói tài nguyên giao diện (Resource Pack Configuration).
    /// </summary>
    public sealed class ResourcePackConfig
    {
        public string ActivePack { get; set; } = "OptimumRealism_64x";
        public List<string> AvailablePacks { get; set; } = new List<string> { "default", "OptimumRealism_64x" };
        public int TileSize { get; set; } = 64;
    }

    /// <summary>
    /// Đối tượng DTO tổng thể chứa toàn bộ cấu hình trò chơi đọc từ StreamingAssets/config.json.
    /// <para>
    /// <b>Nguyên lý OOP áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Đóng gói &amp; Giá trị mặc định (Encapsulation - Tiêu chí 13):</b> Cung cấp phương thức <see cref="CreateDefault"/> đảm bảo luôn có cấu hình chuẩn xác khi file ngoài bị lỗi.</item>
    ///   <item><b>Khớp định dạng JSON:</b> Khớp chính xác cấu trúc file <c>StreamingAssets/config.json</c> của dự án.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class GameConfigDto
    {
        public WorldConfig World { get; set; } = new WorldConfig();
        public PlayerConfig Player { get; set; } = new PlayerConfig();
        public ResourcePackConfig ResourcePack { get; set; } = new ResourcePackConfig();

        /// <summary>
        /// Tạo cấu hình mặc định an toàn cho trò chơi (Fallback Default).
        /// </summary>
        public static GameConfigDto CreateDefault()
        {
            return new GameConfigDto
            {
                World = new WorldConfig(),
                Player = new PlayerConfig(),
                ResourcePack = new ResourcePackConfig()
            };
        }
    }
}
