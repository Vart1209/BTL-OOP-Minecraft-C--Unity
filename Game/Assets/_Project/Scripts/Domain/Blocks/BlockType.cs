using System;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.Blocks
{
    /// <summary>
    /// Thực thể định nghĩa thuộc tính và quy tắc của một loại khối trong thế giới Voxel.
    /// <para>
    /// <b>Nguyên lý OOP áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Đóng gói (Encapsulation - Tiêu chí 13):</b> Toàn bộ các trường dữ liệu (<c>_id</c>, <c>_name</c>, <c>_hardness</c>,...) đều là <c>private readonly</c>, chỉ truy xuất qua các thuộc tính chỉ đọc.</item>
    ///   <item><b>Kiểm tra tính hợp lệ dữ liệu (Validation - Tiêu chí 13):</b> Kiểm tra <c>hardness &gt;= 0</c> hoặc bằng <c>-1.0f</c> (quy ước Bedrock không thể phá trong chế độ Survival - Quy tắc R1). Tên khối không được null hoặc rỗng.</item>
    ///   <item><b>Quy tắc nghiệp vụ rơi đồ (Quy tắc R5):</b> Thuộc tính <see cref="DropItemId"/> xác định vật phẩm rơi ra khi khối bị khai thác.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class BlockType
    {
        private readonly BlockId _id;
        private readonly string _name;
        private readonly bool _isSolid;
        private readonly bool _isTransparent;
        private readonly float _hardness;
        private readonly ItemId _dropItemId;
        private readonly int _topTextureIndex;
        private readonly int _bottomTextureIndex;
        private readonly int _sideTextureIndex;
        private readonly ToolTier _requiredToolTier;

        /// <summary>Mã định danh của khối.</summary>
        public BlockId Id => _id;

        /// <summary>Tên hiển thị của khối.</summary>
        public string Name => _name;

        /// <summary>Khối có cản đường vật lý (rắn) hay không.</summary>
        public bool IsSolid => _isSolid;

        /// <summary>Khối có trong suốt hay không (khối trong suốt không cản ánh sáng và không culling mặt kề).</summary>
        public bool IsTransparent => _isTransparent;

        /// <summary>Cấp độ công cụ tối thiểu cần thiết để thu hoạch được vật phẩm khi phá khối.</summary>
        public ToolTier RequiredToolTier => _requiredToolTier;

        /// <summary>
        /// Độ cứng của khối dùng để tính thời gian đào (Quy tắc R4).
        /// Giá trị -1.0f thể hiện khối vĩnh cửu không thể phá ở chế độ Survival (Quy tắc R1).
        /// </summary>
        public float Hardness => _hardness;

        /// <summary>Mã vật phẩm rơi ra khi khối bị phá hủy (Quy tắc R5).</summary>
        public ItemId DropItemId => _dropItemId;

        /// <summary>Chỉ số ô texture mặt trên.</summary>
        public int TopTextureIndex => _topTextureIndex;

        /// <summary>Chỉ số ô texture mặt dưới.</summary>
        public int BottomTextureIndex => _bottomTextureIndex;

        /// <summary>Chỉ số ô texture các mặt xung quanh.</summary>
        public int SideTextureIndex => _sideTextureIndex;

        /// <summary>
        /// Khởi tạo một kiểu khối mới với đầy đủ thông số nghiệp vụ và ánh xạ texture.
        /// </summary>
        public BlockType(
            BlockId id,
            string name,
            bool isSolid,
            bool isTransparent,
            float hardness,
            ItemId dropItemId,
            int topTextureIndex,
            int bottomTextureIndex,
            int sideTextureIndex,
            ToolTier requiredToolTier = ToolTier.Hand)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Tên loại khối không được để trống.", nameof(name));
            }

            if (hardness < 0 && Math.Abs(hardness - (-1.0f)) > 0.0001f)
            {
                throw new ArgumentOutOfRangeException(nameof(hardness), "Độ cứng của khối phải >= 0 hoặc bằng -1 (Bedrock).");
            }

            _id = id;
            _name = name;
            _isSolid = isSolid;
            _isTransparent = isTransparent;
            _hardness = hardness;
            _dropItemId = dropItemId;
            _topTextureIndex = topTextureIndex;
            _bottomTextureIndex = bottomTextureIndex;
            _sideTextureIndex = sideTextureIndex;
            _requiredToolTier = requiredToolTier;
        }

        /// <summary>
        /// Lấy chỉ số ô texture trên Texture Atlas ứng với từng mặt của khối lập phương.
        /// </summary>
        /// <param name="face">Hướng mặt cần lấy chỉ số texture.</param>
        /// <returns>Chỉ số nguyên đại diện cho ô trong Texture Atlas.</returns>
        public int GetTextureIndex(BlockFace face)
        {
            switch (face)
            {
                case BlockFace.Top:
                    return _topTextureIndex;
                case BlockFace.Bottom:
                    return _bottomTextureIndex;
                case BlockFace.North:
                case BlockFace.South:
                case BlockFace.East:
                case BlockFace.West:
                default:
                    return _sideTextureIndex;
            }
        }
    }
}

