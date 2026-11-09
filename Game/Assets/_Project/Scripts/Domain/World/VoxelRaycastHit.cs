using Minecraft.Domain.Blocks;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.World
{
    /// <summary>
    /// Cấu trúc dữ liệu bất biến (Immutable Value Object) chứa kết quả sau khi thực hiện dò tia Voxel (Raycast).
    /// <para>
    /// <b>Nguyên lý OOP áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Đóng gói &amp; Bất biến (Encapsulation &amp; Immutability - Tiêu chí 13):</b> Khai báo <c>readonly struct</c> với các trường <c>private readonly</c>, chỉ đọc qua Property, không thể bị thay đổi giá trị sau khi tạo.</item>
    ///   <item><b>Null Object Pattern:</b> Cung cấp thuộc tính tĩnh <see cref="Miss"/> đại diện cho trường hợp tia không bắn trúng bất kỳ khối nào, loại bỏ lỗi NullReferenceException.</item>
    /// </list>
    /// </para>
    /// </summary>
    public readonly struct VoxelRaycastHit
    {
        private readonly bool _isHit;
        private readonly BlockPos _blockPosition;
        private readonly BlockPos _adjacentPosition;
        private readonly BlockFace _hitFace;
        private readonly float _distance;

        /// <summary>Có bắn trúng một khối rắn hay không.</summary>
        public bool IsHit => _isHit;

        /// <summary>Tọa độ của khối rắn bị trúng tia nhìn (dùng cho hành vi phá khối).</summary>
        public BlockPos BlockPosition => _blockPosition;

        /// <summary>Tọa độ của ô trống kề trước mặt khối bị bắn trúng (dùng cho hành vi đặt khối).</summary>
        public BlockPos AdjacentPosition => _adjacentPosition;

        /// <summary>Mặt của khối lập phương mà tia nhìn tiếp xúc đầu tiên.</summary>
        public BlockFace HitFace => _hitFace;

        /// <summary>Khoảng cách từ gốc tia đến điểm va chạm.</summary>
        public float Distance => _distance;

        /// <summary>Kết quả tia trượt (không trúng khối nào).</summary>
        public static VoxelRaycastHit Miss => new VoxelRaycastHit(false, BlockPos.Zero, BlockPos.Zero, BlockFace.Top, 0f);

        /// <summary>
        /// Khởi tạo kết quả dò tia.
        /// </summary>
        public VoxelRaycastHit(bool isHit, BlockPos blockPosition, BlockPos adjacentPosition, BlockFace hitFace, float distance)
        {
            _isHit = isHit;
            _blockPosition = blockPosition;
            _adjacentPosition = adjacentPosition;
            _hitFace = hitFace;
            _distance = distance;
        }
    }
}

