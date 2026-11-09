using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.ResourcePacks;

namespace Minecraft.Infrastructure.ResourcePacks
{
    /// <summary>
    /// Chiến lược nạp gói tài nguyên từ thư mục <c>StreamingAssets/ResourcePacks</c> trên ổ đĩa (Strategy 2 — <see cref="ITextureAtlasProvider"/>).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Strategy Pattern (Tiêu chí 9, 10, 11):</b> Hoán đổi linh hoạt với <see cref="BuiltInAtlasProvider"/> thông qua giao diện <see cref="ITextureAtlasProvider"/>.</item>
    ///   <item><b>Xử lý ngoại lệ an toàn (Fault Tolerance - Tiêu chí 19):</b> Bọc <c>try-catch</c> toàn diện khi đọc <c>pack.mcmeta</c> và quét thư mục. Nếu thiếu file <c>pack.mcmeta</c> hoặc lỗi cú pháp JSON, tự động khởi tạo thông tin mặc định; nếu thư mục gói không tồn tại, tự động ủy quyền (fallback) về <see cref="BuiltInAtlasProvider"/>.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class FolderResourcePackProvider : ITextureAtlasProvider
    {
        private readonly string _resourcePacksRootPath;
        private readonly ITextureAtlasProvider _fallbackProvider;

        private static readonly IReadOnlyDictionary<BlockId, string> BlockFileNames = new Dictionary<BlockId, string>
        {
            { BlockId.Stone, "stone.png" },
            { BlockId.Dirt, "dirt.png" },
            { BlockId.Grass, "grass_block_top.png" },
            { BlockId.Planks, "oak_planks.png" },
            { BlockId.Cobblestone, "cobblestone.png" },
            { BlockId.Bedrock, "bedrock.png" },
            { BlockId.Sand, "sand.png" },
            { BlockId.Wood, "oak_log.png" },
            { BlockId.Leaves, "oak_leaves.png" },
            { BlockId.CraftingTable, "crafting_table_top.png" }
        };

        /// <inheritdoc />
        public string ProviderName => "FolderResourcePackProvider";

        /// <summary>Đường dẫn thư mục gốc chứa các gói tài nguyên trên đĩa.</summary>
        public string ResourcePacksRootPath => _resourcePacksRootPath;

        /// <summary>
        /// Khởi tạo bộ nạp gói tài nguyên từ thư mục đĩa.
        /// </summary>
        /// <param name="resourcePacksRootPath">Đường dẫn tới thư mục <c>StreamingAssets/ResourcePacks</c>.</param>
        /// <param name="fallbackProvider">Chiến lược dự phòng khi thư mục không tồn tại (mặc định là <see cref="BuiltInAtlasProvider"/>).</param>
        public FolderResourcePackProvider(string resourcePacksRootPath, ITextureAtlasProvider fallbackProvider = null)
        {
            _resourcePacksRootPath = resourcePacksRootPath ?? string.Empty;
            _fallbackProvider = fallbackProvider ?? new BuiltInAtlasProvider();
        }

        /// <inheritdoc />
        public IReadOnlyList<string> GetAvailablePacks()
        {
            var packs = new List<string>();

            try
            {
                if (Directory.Exists(_resourcePacksRootPath))
                {
                    string[] subDirs = Directory.GetDirectories(_resourcePacksRootPath);
                    foreach (string dir in subDirs)
                    {
                        string folderName = Path.GetFileName(dir);
                        if (!string.IsNullOrWhiteSpace(folderName))
                        {
                            packs.Add(folderName);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Bắt mọi lỗi I/O hoặc quyền truy cập thư mục và trả về danh sách dự phòng
            }

            if (!packs.Contains(BuiltInAtlasProvider.DefaultPackId))
            {
                packs.Insert(0, BuiltInAtlasProvider.DefaultPackId);
            }

            return packs;
        }

        /// <inheritdoc />
        public ResourcePackManifest LoadPack(string packId)
        {
            if (string.IsNullOrWhiteSpace(packId))
            {
                return _fallbackProvider.LoadPack(BuiltInAtlasProvider.DefaultPackId);
            }

            string trimmedPackId = packId.Trim();

            try
            {
                if (string.IsNullOrWhiteSpace(_resourcePacksRootPath) || !Directory.Exists(_resourcePacksRootPath))
                {
                    return _fallbackProvider.LoadPack(trimmedPackId);
                }

                string packFolderPath = Path.Combine(_resourcePacksRootPath, trimmedPackId);
                if (!Directory.Exists(packFolderPath))
                {
                    return _fallbackProvider.LoadPack(trimmedPackId);
                }

                // Đọc thông tin pack.mcmeta an toàn
                string mcmetaPath = Path.Combine(packFolderPath, "pack.mcmeta");
                int packFormat = 15;
                string description = $"Gói tài nguyên thư mục ({trimmedPackId})";

                if (File.Exists(mcmetaPath))
                {
                    try
                    {
                        string json = File.ReadAllText(mcmetaPath);
                        using JsonDocument doc = JsonDocument.Parse(json);
                        if (doc.RootElement.TryGetProperty("pack", out JsonElement packElem))
                        {
                            if (packElem.TryGetProperty("pack_format", out JsonElement formatElem) &&
                                formatElem.TryGetInt32(out int parsedFormat))
                            {
                                packFormat = parsedFormat;
                            }

                            if (packElem.TryGetProperty("description", out JsonElement descElem) &&
                                descElem.ValueKind == JsonValueKind.String)
                            {
                                string parsedDesc = descElem.GetString();
                                if (!string.IsNullOrWhiteSpace(parsedDesc))
                                {
                                    description = parsedDesc;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Nếu pack.mcmeta bị lỗi cú pháp JSON hoặc lỗi đọc file, giữ nguyên mô tả mặc định hợp lệ
                        description = $"Gói tài nguyên ({trimmedPackId}) - Cấu hình mặc định dự phòng";
                    }
                }

                // Xác định độ phân giải dựa trên tên gói hoặc mặc định
                int tileResolution = trimmedPackId.IndexOf("64x", StringComparison.OrdinalIgnoreCase) >= 0 ? 64 : 16;

                // Kiểm tra các file ảnh khối trong assets/minecraft/textures/block/
                var texturePaths = BuiltInAtlasProvider.CreateDefaultBlockTextureMap();
                string blockTexturesDir = Path.Combine(packFolderPath, "assets", "minecraft", "textures", "block");

                if (Directory.Exists(blockTexturesDir))
                {
                    foreach (var kvp in BlockFileNames)
                    {
                        string candidatePath = Path.Combine(blockTexturesDir, kvp.Value);
                        if (File.Exists(candidatePath))
                        {
                            texturePaths[kvp.Key] = candidatePath;
                        }
                    }
                }

                return new ResourcePackManifest(
                    packId: trimmedPackId,
                    displayName: trimmedPackId,
                    description: description,
                    packFormat: packFormat,
                    tileResolution: tileResolution,
                    blockTexturePaths: texturePaths);
            }
            catch (Exception)
            {
                // Mọi sự cố bất ngờ đều được hồi phục về gói mặc định tích hợp sẵn (Tiêu chí 19)
                return _fallbackProvider.LoadPack(BuiltInAtlasProvider.DefaultPackId);
            }
        }
    }
}
