using System;
using System.Collections.Generic;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.ResourcePacks;

namespace Minecraft.Application.World
{
    /// <summary>
    /// Dịch vụ điều phối Use Case đổi Gói Tài nguyên lúc chạy (UC06b — Đổi Resource Pack trong menu/runtime).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Design Pattern (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Strategy Pattern (Tiêu chí 9, 10, 11):</b> Ủy quyền việc tìm kiếm và nạp gói tài nguyên cho <see cref="ITextureAtlasProvider"/>. Cho phép thay đổi chiến lược nạp lúc runtime qua <see cref="SetProvider"/>.</item>
    ///   <item><b>Observer Pattern (Tiêu chí 9, 10, 11):</b> Phát sự kiện <see cref="ResourcePackChanged"/> mỗi khi chuyển đổi gói tài nguyên thành công để tầng Presentation (<c>ResourcePackView</c>) tự động nạp lại ảnh và cập nhật Material mà không làm bẩn tầng Application.</item>
    ///   <item><b>Đóng gói (Encapsulation - Tiêu chí 13):</b> Giữ <see cref="CurrentPack"/> chỉ đọc bên ngoài và kiểm tra đầu vào hợp lệ.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class ResourcePackService
    {
        private ITextureAtlasProvider _provider;
        private ResourcePackManifest _currentPack;

        /// <summary>Gói tài nguyên đang được kích hoạt hiện tại.</summary>
        public ResourcePackManifest CurrentPack => _currentPack;

        /// <summary>Chiến lược cung cấp gói tài nguyên hiện hành.</summary>
        public ITextureAtlasProvider Provider => _provider;

        /// <summary>Sự kiện phát ra khi gói tài nguyên được thay đổi thành công (Observer Pattern).</summary>
        public event Action<ResourcePackManifest> ResourcePackChanged;

        /// <summary>
        /// Khởi tạo dịch vụ quản lý gói tài nguyên với chiến lược cung cấp ban đầu.
        /// </summary>
        /// <param name="provider">Chiến lược nạp gói tài nguyên (không được null).</param>
        public ResourcePackService(ITextureAtlasProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _currentPack = _provider.LoadPack("default");
        }

        /// <summary>
        /// Thay đổi chiến lược nạp gói tài nguyên lúc runtime (Strategy Pattern).
        /// </summary>
        /// <param name="newProvider">Chiến lược cung cấp mới (không được null).</param>
        public void SetProvider(ITextureAtlasProvider newProvider)
        {
            _provider = newProvider ?? throw new ArgumentNullException(nameof(newProvider));
        }

        /// <summary>
        /// Lấy danh sách các gói tài nguyên hiện có từ nhà cung cấp hiện tại.
        /// </summary>
        public IReadOnlyList<string> GetAvailablePacks()
        {
            return _provider.GetAvailablePacks();
        }

        /// <summary>
        /// Chuyển đổi sang gói tài nguyên có mã định danh chỉ định và thông báo cho các Observer.
        /// </summary>
        /// <param name="packId">Mã gói tài nguyên cần chuyển sang.</param>
        /// <returns><c>true</c> nếu nạp và chuyển gói thành công; ngược lại <c>false</c>.</returns>
        public bool SwitchPack(string packId)
        {
            if (string.IsNullOrWhiteSpace(packId))
            {
                return false;
            }

            ResourcePackManifest loadedManifest = _provider.LoadPack(packId);
            if (loadedManifest == null)
            {
                return false;
            }

            _currentPack = loadedManifest;
            ResourcePackChanged?.Invoke(_currentPack);
            return true;
        }
    }
}
