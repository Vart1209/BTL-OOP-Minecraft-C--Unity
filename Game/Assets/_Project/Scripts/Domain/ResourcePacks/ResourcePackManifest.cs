using System;
using System.Collections.Generic;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.ResourcePacks
{
    /// <summary>
    /// Thực thể bất biến (Immutable Entity) lưu trữ thông tin mô tả và bảng ánh xạ đường dẫn ảnh của một Gói Tài nguyên (Resource Pack).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Đóng gói &amp; Bất biến (Encapsulation &amp; Immutability - Tiêu chí 13):</b> Toàn bộ thuộc tính là <c>private readonly</c>, cung cấp qua getter chỉ đọc và sao chép từ điển sang bộ đọc an toàn để không lớp nào ngoài kia sửa đổi được dữ liệu gốc.</item>
    ///   <item><b>Kiểm tra hợp lệ đầu vào (Validation - Tiêu chí 13):</b> Từ chối <c>packId</c> rỗng hoặc độ phân giải ô <c>tileResolution &lt;= 0</c>.</item>
    ///   <item><b>Thuần C# (Domain Purity - Tiêu chí 8):</b> Không phụ thuộc <c>UnityEngine.Texture2D</c>, chỉ lưu đường dẫn tới tài nguyên để tầng Presentation nạp và hiển thị.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class ResourcePackManifest
    {
        private readonly string _packId;
        private readonly string _displayName;
        private readonly string _description;
        private readonly int _packFormat;
        private readonly int _tileResolution;
        private readonly IReadOnlyDictionary<BlockId, string> _blockTexturePaths;

        /// <summary>Mã định danh duy nhất của gói tài nguyên (thường trùng tên thư mục, ví dụ <c>default</c> hoặc <c>OptimumRealism_64x</c>).</summary>
        public string PackId => _packId;

        /// <summary>Tên hiển thị của gói tài nguyên trên giao diện người dùng.</summary>
        public string DisplayName => _displayName;

        /// <summary>Mô tả chi tiết gói tài nguyên trích xuất từ <c>pack.mcmeta</c>.</summary>
        public string Description => _description;

        /// <summary>Phiên bản định dạng gói tài nguyên theo chuẩn Minecraft (<c>pack_format</c>).</summary>
        public int PackFormat => _packFormat;

        /// <summary>Độ phân giải mỗi ô chất liệu tính theo pixel (mặc định 16 cho gói chuẩn, 64 cho gói HD).</summary>
        public int TileResolution => _tileResolution;

        /// <summary>Bảng ánh xạ chỉ đọc từ từng mã khối (<see cref="BlockId"/>) sang đường dẫn file ảnh (<c>.png</c>) tương ứng.</summary>
        public IReadOnlyDictionary<BlockId, string> BlockTexturePaths => _blockTexturePaths;

        /// <summary>
        /// Khởi tạo thông tin gói tài nguyên mới với kiểm tra dữ liệu đầu vào nghiêm ngặt.
        /// </summary>
        /// <param name="packId">Mã định danh gói tài nguyên (không được rỗng).</param>
        /// <param name="displayName">Tên hiển thị của gói.</param>
        /// <param name="description">Mô tả gói tài nguyên.</param>
        /// <param name="packFormat">Phiên bản định dạng gói.</param>
        /// <param name="tileResolution">Độ phân giải mỗi ô chất liệu (&gt; 0, mặc định 16 hoặc 64).</param>
        /// <param name="blockTexturePaths">Bảng ánh xạ đường dẫn texture của từng loại khối.</param>
        public ResourcePackManifest(
            string packId,
            string displayName,
            string description,
            int packFormat,
            int tileResolution = 16,
            IReadOnlyDictionary<BlockId, string> blockTexturePaths = null)
        {
            if (string.IsNullOrWhiteSpace(packId))
            {
                throw new ArgumentException("Mã định danh gói tài nguyên (PackId) không được để trống.", nameof(packId));
            }

            if (tileResolution <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tileResolution), "Độ phân giải ô chất liệu (TileResolution) phải lớn hơn 0.");
            }

            _packId = packId.Trim();
            _displayName = string.IsNullOrWhiteSpace(displayName) ? _packId : displayName.Trim();
            _description = string.IsNullOrWhiteSpace(description) ? "Gói tài nguyên mặc định" : description.Trim();
            _packFormat = packFormat;
            _tileResolution = tileResolution;

            var copy = new Dictionary<BlockId, string>();
            if (blockTexturePaths != null)
            {
                foreach (var kvp in blockTexturePaths)
                {
                    if (!string.IsNullOrWhiteSpace(kvp.Value))
                    {
                        copy[kvp.Key] = kvp.Value;
                    }
                }
            }

            _blockTexturePaths = copy;
        }

        /// <summary>
        /// Tra cứu đường dẫn file ảnh chất liệu của một loại khối cụ thể.
        /// </summary>
        /// <param name="blockId">Mã khối cần tra cứu.</param>
        /// <param name="texturePath">Đường dẫn file ảnh trả về nếu tìm thấy.</param>
        /// <returns><c>true</c> nếu gói tài nguyên có định nghĩa đường dẫn cho khối này; ngược lại <c>false</c>.</returns>
        public bool TryGetTexturePath(BlockId blockId, out string texturePath)
        {
            return _blockTexturePaths.TryGetValue(blockId, out texturePath);
        }
    }
}
