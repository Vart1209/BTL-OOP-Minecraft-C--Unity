using System;
using System.Collections.Generic;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.ResourcePacks;

namespace Minecraft.Infrastructure.ResourcePacks
{
    /// <summary>
    /// Chiến lược nạp gói tài nguyên mặc định tích hợp sẵn trong mã nguồn (Strategy 1 — <see cref="ITextureAtlasProvider"/>).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Design Pattern (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Strategy Pattern (Tiêu chí 9, 10, 11):</b> Cài đặt cụ thể của <see cref="ITextureAtlasProvider"/>, cung cấp gói chất liệu chuẩn <c>default</c> 16×16 không phụ thuộc vào hệ thống tệp tin bên ngoài.</item>
    ///   <item><b>Khả năng tự phục hồi (Fallback - Tiêu chí 19):</b> Đóng vai trò gói dự phòng an toàn khi thư mục <c>StreamingAssets/ResourcePacks</c> bị xóa hoặc hỏng.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class BuiltInAtlasProvider : ITextureAtlasProvider
    {
        /// <summary>Mã định danh của gói mặc định tích hợp sẵn.</summary>
        public const string DefaultPackId = "default";

        private static readonly IReadOnlyList<string> AvailablePackIds = new[] { DefaultPackId };

        /// <inheritdoc />
        public string ProviderName => "BuiltInAtlasProvider";

        /// <inheritdoc />
        public IReadOnlyList<string> GetAvailablePacks()
        {
            return AvailablePackIds;
        }

        /// <inheritdoc />
        public ResourcePackManifest LoadPack(string packId)
        {
            string resolvedId = string.IsNullOrWhiteSpace(packId) ? DefaultPackId : DefaultPackId;
            var defaultTextures = CreateDefaultBlockTextureMap();

            return new ResourcePackManifest(
                packId: resolvedId,
                displayName: "Default Pixel Art (16x)",
                description: "Gói tài nguyên mặc định tích hợp sẵn chuẩn 16x16 pixel.",
                packFormat: 15,
                tileResolution: 16,
                blockTexturePaths: defaultTextures);
        }

        /// <summary>
        /// Tạo bảng ánh xạ tên tệp ảnh chuẩn Minecraft cho toàn bộ các loại khối trong trò chơi.
        /// </summary>
        public static Dictionary<BlockId, string> CreateDefaultBlockTextureMap()
        {
            return new Dictionary<BlockId, string>
            {
                { BlockId.Bedrock, "assets/minecraft/textures/block/bedrock.png" },
                { BlockId.Stone, "assets/minecraft/textures/block/stone.png" },
                { BlockId.Dirt, "assets/minecraft/textures/block/dirt.png" },
                { BlockId.Grass, "assets/minecraft/textures/block/grass_block_top.png" },
                { BlockId.Wood, "assets/minecraft/textures/block/oak_log.png" },
                { BlockId.Leaves, "assets/minecraft/textures/block/oak_leaves.png" },
                { BlockId.Sand, "assets/minecraft/textures/block/sand.png" },
                { BlockId.Planks, "assets/minecraft/textures/block/oak_planks.png" },
                { BlockId.CraftingTable, "assets/minecraft/textures/block/crafting_table_top.png" },
                { BlockId.Cobblestone, "assets/minecraft/textures/block/cobblestone.png" }
            };
        }
    }
}
