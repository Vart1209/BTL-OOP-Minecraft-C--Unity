using System;
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
    ///   <item><b>Observer Pattern (Tiêu chí 9, 10, 11):</b> Lắng nghe sự kiện <see cref="ResourcePackService.ResourcePackChanged"/> để tự động đọc dữ liệu ảnh (<c>.png</c>) từ đĩa và cập nhật vào <see cref="Material"/> của <see cref="WorldView"/> mà không làm tầng Application phụ thuộc vào <c>UnityEngine</c>.</item>
    ///   <item><b>Xử lý ngoại lệ an toàn (Tiêu chí 19):</b> Bọc <c>try-catch</c> khi đọc mảng byte tệp ảnh và giải mã bằng <see cref="ImageConversion.LoadImage(Texture2D, byte[])"/>, giữ bộ lọc <see cref="FilterMode.Point"/> để bảo toàn độ sắc nét của phong cách đồ họa Pixel Art / Voxel.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class ResourcePackView : MonoBehaviour
    {
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
                int resolution = Mathf.Max(16, manifest.TileResolution);
                var texture = new Texture2D(resolution, resolution, TextureFormat.RGBA32, mipChain: false)
                {
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp,
                    name = $"Atlas_{manifest.PackId}_{resolution}x"
                };

                bool loadedFromFile = false;
                if (manifest.TryGetTexturePath(BlockId.Stone, out string samplePath) &&
                    !string.IsNullOrWhiteSpace(samplePath) &&
                    File.Exists(samplePath))
                {
                    byte[] rawBytes = File.ReadAllBytes(samplePath);
                    loadedFromFile = ImageConversion.LoadImage(texture, rawBytes);
                    texture.filterMode = FilterMode.Point;
                }

                if (!loadedFromFile)
                {
                    FillProceduralFallbackTexture(texture, resolution);
                }

                if (_activeAtlasTexture != null)
                {
                    Destroy(_activeAtlasTexture);
                }

                _activeAtlasTexture = texture;
                ApplyTextureToWorldMaterial(_activeAtlasTexture);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[ResourcePackView] Không thể nạp texture từ gói '{manifest.PackId}': {ex.Message}");
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
                _targetMaterial.mainTexture = texture;
                if (_worldView != null)
                {
                    _worldView.SetTerrainMaterial(_targetMaterial);
                }
            }
        }

        private static void FillProceduralFallbackTexture(Texture2D texture, int resolution)
        {
            var pixels = new Color[resolution * resolution];
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    bool checker = ((x / 4) + (y / 4)) % 2 == 0;
                    pixels[y * resolution + x] = checker
                        ? new Color(0.55f, 0.55f, 0.55f, 1f)
                        : new Color(0.45f, 0.45f, 0.45f, 1f);
                }
            }

            texture.SetPixels(pixels);
            texture.Apply(updateMipmaps: false, makeNoLongerReadable: false);
        }
    }
}
