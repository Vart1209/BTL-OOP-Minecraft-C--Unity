using System.Collections.Generic;
using Minecraft.Domain.ResourcePacks;

namespace Minecraft.Domain.Contracts
{
    /// <summary>
    /// Hợp đồng giao diện cung cấp và nạp Gói Tài nguyên (Resource Pack) theo mẫu thiết kế Strategy Pattern.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Design Pattern (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Strategy Pattern (Tiêu chí 9, 10, 11):</b> Tách rời thuật toán và nguồn nạp chất liệu khỏi dịch vụ điều phối. Hệ thống có thể dùng gói tích hợp sẵn trong bộ nhớ (<c>BuiltInAtlasProvider</c>) hoặc quét trực tiếp thư mục ngoài đĩa (<c>FolderResourcePackProvider</c>) mà không cần sửa đổi tầng Application hay Domain.</item>
    ///   <item><b>Đảo ngược phụ thuộc (DIP - Tiêu chí 16):</b> Tầng Application chỉ gọi qua giao diện <see cref="ITextureAtlasProvider"/>, hoàn toàn không phụ thuộc vào <c>System.IO</c> ở tầng Infrastructure.</item>
    /// </list>
    /// </para>
    /// </summary>
    public interface ITextureAtlasProvider
    {
        /// <summary>Tên định danh của chiến lược cung cấp gói tài nguyên.</summary>
        string ProviderName { get; }

        /// <summary>
        /// Lấy danh sách mã định danh của tất cả các gói tài nguyên hiện có.
        /// </summary>
        IReadOnlyList<string> GetAvailablePacks();

        /// <summary>
        /// Nạp thông tin chi tiết và bảng ánh xạ chất liệu của một gói tài nguyên theo mã định danh.
        /// </summary>
        /// <param name="packId">Mã định danh gói cần nạp (ví dụ: <c>default</c>, <c>OptimumRealism_64x</c>).</param>
        /// <returns>Thực thể <see cref="ResourcePackManifest"/> chứa thông tin gói đã nạp.</returns>
        ResourcePackManifest LoadPack(string packId);
    }
}
