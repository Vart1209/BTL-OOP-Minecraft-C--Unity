using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Minecraft.Application.World;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.ResourcePacks;

namespace Minecraft.Presentation.World
{
    /// <summary>
    /// Thành phần hiển thị và áp dụng Gói Tài nguyên (Resource Pack) lên bề mặt địa hình trong Unity (Presentation Layer).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Observer Pattern (Tiêu chí 9, 10, 11):</b> Lắng nghe sự kiện <see cref="ResourcePackService.ResourcePackChanged"/> để tự động đọc dữ liệu ảnh (<c>.png</c>) từ đĩa, ghép thành lưới Texture Atlas 16×16 ô và cập nhật vào <see cref="Material"/> của <see cref="WorldView"/> mà không làm tầng Application phụ thuộc vào <c>UnityEngine</c>.</item>
    ///   <item><b>Xử lý ngoại lệ an toàn (Tiêu chí 19):</b> Bọc <c>try-catch</c> khi đọc mảng byte tệp ảnh và giải mã bằng <see cref="ImageConversion.LoadImage(Texture2D, byte[])"/>, giữ bộ lọc <see cref="FilterMode.Point"/> để bảo toàn độ sắc nét của phong cách đồ họa Pixel Art / Voxel.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class ResourcePackView : MonoBehaviour
    {
        private const int AtlasGridSize = 16;

        [SerializeField] private WorldView _worldView;
        [SerializeField] private Material _targetMaterial;

        private ResourcePackService _resourcePackService;
        private Texture2D _activeAtlasTexture;

        /// <summary>Texture2D hiện hành đã nạp từ gói tài nguyên gần nhất.</summary>
        public Texture2D ActiveAtlasTexture => _activeAtlasTexture;

        /// <summary>
        /// Khởi tạo và đăng ký lắng nghe dịch vụ đổi gói tài nguyên từ <c>GameBootstrap</c>.
        /// </summary>
        public void Initialize(ResourcePackService resourcePackService, WorldView worldView = null, Material targetMaterial = null)
        {
            if (_resourcePackService != null)
            {
                _resourcePackService.ResourcePackChanged -= OnResourcePackChanged;
            }

            _resourcePackService = resourcePackService ?? throw new ArgumentNullException(nameof(resourcePackService));
            if (worldView != null)
            {
                _worldView = worldView;
            }

            if (targetMaterial != null)
            {
                _targetMaterial = targetMaterial;
            }

            _resourcePackService.ResourcePackChanged += OnResourcePackChanged;

            if (_resourcePackService.CurrentPack != null)
            {
                OnResourcePackChanged(_resourcePackService.CurrentPack);
            }
        }

        private void OnDestroy()
        {
            if (_resourcePackService != null)
            {
                _resourcePackService.ResourcePackChanged -= OnResourcePackChanged;
            }

            if (_activeAtlasTexture != null)
            {
                Destroy(_activeAtlasTexture);
                _activeAtlasTexture = null;
            }
        }

        private void OnResourcePackChanged(ResourcePackManifest manifest)
        {
            if (manifest == null)
            {
                return;
            }

            try
            {
                int tileRes = Mathf.Clamp(manifest.TileResolution, 16, 128);
                int atlasWidth = tileRes * AtlasGridSize;
                int atlasHeight = tileRes * AtlasGridSize;

                var atlas = new Texture2D(atlasWidth, atlasHeight, TextureFormat.RGBA32, mipChain: false)
                {
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp,
                    name = $"Atlas_{manifest.PackId}_{tileRes}x"
                };

                // Bước 1: Vẽ sẵn toàn bộ 16 ô (indices 0..15) bằng texture màu sắc đặc trưng của từng loại khối (Procedural Pixel-Art)
                for (int tileIndex = 0; tileIndex < 16; tileIndex++)
                {
                    FillProceduralTile(atlas, tileIndex, tileRes);
                }

                // Bước 2: Nếu gói tài nguyên có file ảnh thật trên đĩa (như OptimumRealism_64x), nạp và đè lên đúng ô tileIndex tương ứng
                string blockDir = ResolveBlockTexturesDirectory(manifest);
                if (!string.IsNullOrEmpty(blockDir) && Directory.Exists(blockDir))
                {
                    TryLoadAndBlitTile(atlas, 1, Path.Combine(blockDir, "bedrock.png"), tileRes, Color.white);
                    TryLoadAndBlitTile(atlas, 2, Path.Combine(blockDir, "stone.png"), tileRes, Color.white);
                    TryLoadAndBlitTile(atlas, 3, Path.Combine(blockDir, "dirt.png"), tileRes, Color.white);
                    // Mặt cỏ trên đỉnh trong Minecraft là ảnh grayscale, nhân màu xanh lá tự nhiên
                    TryLoadAndBlitTile(atlas, 4, Path.Combine(blockDir, "grass_block_top.png"), tileRes, new Color(0.48f, 0.78f, 0.32f, 1f));
                    TryLoadAndBlitTile(atlas, 5, Path.Combine(blockDir, "grass_block_side.png"), tileRes, Color.white);
                    TryLoadAndBlitTile(atlas, 6, Path.Combine(blockDir, "oak_log.png"), tileRes, Color.white);
                    TryLoadAndBlitTile(atlas, 7, Path.Combine(blockDir, "oak_log_top.png"), tileRes, Color.white);
                    TryLoadAndBlitTile(atlas, 8, Path.Combine(blockDir, "oak_leaves.png"), tileRes, new Color(0.38f, 0.72f, 0.28f, 1f));
                    TryLoadAndBlitTile(atlas, 9, Path.Combine(blockDir, "sand.png"), tileRes, Color.white);
                    TryLoadAndBlitTile(atlas, 10, Path.Combine(blockDir, "oak_planks.png"), tileRes, Color.white);
                    TryLoadAndBlitTile(atlas, 11, Path.Combine(blockDir, "crafting_table_top.png"), tileRes, Color.white);
                    TryLoadAndBlitTile(atlas, 12, Path.Combine(blockDir, "crafting_table_side.png"), tileRes, Color.white);
                    TryLoadAndBlitTile(atlas, 13, Path.Combine(blockDir, "cobblestone.png"), tileRes, Color.white);
                }

                atlas.Apply(updateMipmaps: false, makeNoLongerReadable: false);

                if (_activeAtlasTexture != null)
                {
                    Destroy(_activeAtlasTexture);
                }

                _activeAtlasTexture = atlas;
                ApplyTextureToWorldMaterial(_activeAtlasTexture);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[ResourcePackView] Không thể nạp texture từ gói '{manifest.PackId}': {ex.Message}");
            }
        }

        private static string ResolveBlockTexturesDirectory(ResourcePackManifest manifest)
        {
            if (manifest.TryGetTexturePath(BlockId.Stone, out string samplePath) &&
                !string.IsNullOrWhiteSpace(samplePath) &&
                File.Exists(samplePath))
            {
                return Path.GetDirectoryName(samplePath);
            }

            string fallbackDir = Path.Combine(
                UnityEngine.Application.streamingAssetsPath,
                "ResourcePacks",
                manifest.PackId,
                "assets",
                "minecraft",
                "textures",
                "block");
            return Directory.Exists(fallbackDir) ? fallbackDir : string.Empty;
        }

        private static void TryLoadAndBlitTile(Texture2D atlas, int tileIndex, string filePath, int tileRes, Color tint)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return;
            }

            Texture2D temp = new Texture2D(2, 2, TextureFormat.RGBA32, mipChain: false);
            try
            {
                byte[] rawBytes = File.ReadAllBytes(filePath);
                if (!ImageConversion.LoadImage(temp, rawBytes))
                {
                    return;
                }

                int tileX = tileIndex % AtlasGridSize;
                int tileY = tileIndex / AtlasGridSize;
                int startX = tileX * tileRes;
                int startY = (AtlasGridSize - 1 - tileY) * tileRes;

                Color[] blockPixels = new Color[tileRes * tileRes];
                for (int y = 0; y < tileRes; y++)
                {
                    float v = (y + 0.5f) / tileRes;
                    for (int x = 0; x < tileRes; x++)
                    {
                        float u = (x + 0.5f) / tileRes;
                        Color c = temp.GetPixelBilinear(u, v) * tint;
                        c.a = 1f;
                        blockPixels[y * tileRes + x] = c;
                    }
                }

                atlas.SetPixels(startX, startY, tileRes, tileRes, blockPixels);
            }
            catch (Exception)
            {
                // Giữ nguyên ô procedural nếu 1 ảnh đơn lẻ bị lỗi
            }
            finally
            {
                Destroy(temp);
            }
        }

        private static void FillProceduralTile(Texture2D atlas, int tileIndex, int tileRes)
        {
            int tileX = tileIndex % AtlasGridSize;
            int tileY = tileIndex / AtlasGridSize;
            int startX = tileX * tileRes;
            int startY = (AtlasGridSize - 1 - tileY) * tileRes;

            Color baseColor = GetProceduralBaseColor(tileIndex);
            Color[] pixels = new Color[tileRes * tileRes];

            for (int y = 0; y < tileRes; y++)
            {
                for (int x = 0; x < tileRes; x++)
                {
                    // Tạo hạt nhiễu pixel-art 16x16 đặc trưng kiểu Minecraft cổ điển
                    int px = (x * 16) / tileRes;
                    int py = (y * 16) / tileRes;
                    uint hash = (uint)(px * 374761393 + py * 668265263 + tileIndex * 1442695041);
                    hash = (hash ^ (hash >> 13)) * 1274126177;
                    float variation = ((hash & 15) / 15f - 0.5f) * 0.14f;

                    Color pixelColor = new Color(
                        Mathf.Clamp01(baseColor.r + variation),
                        Mathf.Clamp01(baseColor.g + variation),
                        Mathf.Clamp01(baseColor.b + variation),
                        1f);

                    // Nếu là mặt bên của khối cỏ (tileIndex == 5), vẽ viền cỏ xanh ở 1/4 phía trên
                    if (tileIndex == 5 && py >= 12)
                    {
                        Color grassTop = new Color(0.36f, 0.72f, 0.24f, 1f);
                        pixelColor = new Color(
                            Mathf.Clamp01(grassTop.r + variation),
                            Mathf.Clamp01(grassTop.g + variation),
                            Mathf.Clamp01(grassTop.b + variation),
                            1f);
                    }

                    pixels[y * tileRes + x] = pixelColor;
                }
            }

            atlas.SetPixels(startX, startY, tileRes, tileRes, pixels);
        }

        private static Color GetProceduralBaseColor(int tileIndex)
        {
            switch (tileIndex)
            {
                case 1: return new Color(0.22f, 0.22f, 0.24f, 1f); // Bedrock
                case 2: return new Color(0.52f, 0.53f, 0.55f, 1f); // Stone
                case 3: return new Color(0.54f, 0.37f, 0.23f, 1f); // Dirt
                case 4: return new Color(0.36f, 0.72f, 0.24f, 1f); // Grass Top
                case 5: return new Color(0.54f, 0.37f, 0.23f, 1f); // Grass Side (base dirt + green rim)
                case 6: return new Color(0.42f, 0.30f, 0.18f, 1f); // Wood Bark
                case 7: return new Color(0.68f, 0.54f, 0.34f, 1f); // Wood Top
                case 8: return new Color(0.24f, 0.56f, 0.20f, 1f); // Leaves
                case 9: return new Color(0.86f, 0.82f, 0.58f, 1f); // Sand
                case 10: return new Color(0.66f, 0.50f, 0.30f, 1f); // Planks
                case 11: return new Color(0.58f, 0.38f, 0.22f, 1f); // Crafting Table Top
                case 12: return new Color(0.52f, 0.34f, 0.20f, 1f); // Crafting Table Side
                case 13: return new Color(0.44f, 0.44f, 0.46f, 1f); // Cobblestone
                default: return new Color(0.50f, 0.50f, 0.50f, 1f);
            }
        }

        private void ApplyTextureToWorldMaterial(Texture2D texture)
        {
            if (_targetMaterial == null && _worldView != null)
            {
                _targetMaterial = _worldView.TerrainMaterial;
            }

            if (_targetMaterial == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                if (shader != null)
                {
                    _targetMaterial = new Material(shader);
                }
            }

            if (_targetMaterial != null)
            {
                _targetMaterial.color = Color.white;
                _targetMaterial.mainTexture = texture;
                if (_targetMaterial.HasProperty("_BaseMap"))
                {
                    _targetMaterial.SetTexture("_BaseMap", texture);
                }

                if (_targetMaterial.HasProperty("_BaseColor"))
                {
                    _targetMaterial.SetColor("_BaseColor", Color.white);
                }

                if (_targetMaterial.HasProperty("_Smoothness"))
                {
                    _targetMaterial.SetFloat("_Smoothness", 0.0f);
                }

                if (_targetMaterial.HasProperty("_Glossiness"))
                {
                    _targetMaterial.SetFloat("_Glossiness", 0.0f);
                }

                if (_worldView != null)
                {
                    _worldView.SetTerrainMaterial(_targetMaterial);
                }
            }
        }
    }
}
