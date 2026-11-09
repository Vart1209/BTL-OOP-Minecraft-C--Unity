namespace Minecraft.Domain.Contracts
{
    /// <summary>
    /// Gói dữ liệu sự kiện (Event Payload) được phát ra mỗi khi một khối trong thế giới bị thay đổi (đặt mới hoặc bị phá hủy).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Design Pattern áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Observer Pattern (Tiêu chí 9, 10, 11):</b> Lớp <c>World</c> ở tầng Domain chỉ phát sự kiện chứa <see cref="BlockChangedEventArgs"/> mà không cần biết tới <c>ChunkView</c> hay <c>AudioSource</c> ở tầng Presentation. Nhờ đó giảm sự phụ thuộc chặt (Loose Coupling).</item>
    ///   <item><b>Đóng gói (Encapsulation - Tiêu chí 13):</b> Các trường dữ liệu đều là <c>private readonly</c>, bên nhận sự kiện chỉ có thể đọc thông tin chứ không thể sửa ngược trạng thái sự kiện.</item>
    /// </list>
    /// </para>
    /// </summary>
    public readonly struct BlockChangedEventArgs
    {
        private readonly BlockPos _position;
        private readonly BlockId _oldBlock;
        private readonly BlockId _newBlock;

        /// <summary>
        /// Tọa độ toàn cục (World Position) của khối vừa thay đổi.
        /// </summary>
        public BlockPos Position => _position;

        /// <summary>
        /// Mã loại khối trước khi thay đổi.
        /// </summary>
        public BlockId OldBlock => _oldBlock;

        /// <summary>
        /// Mã loại khối mới sau khi thay đổi.
        /// </summary>
        public BlockId NewBlock => _newBlock;

        /// <summary>
        /// Trả về <c>true</c> nếu loại khối mới thực sự khác với loại khối cũ.
        /// </summary>
        public bool HasChanged => _oldBlock != _newBlock;

        /// <summary>
        /// Khởi tạo thông tin sự kiện thay đổi khối.
        /// </summary>
        /// <param name="position">Tọa độ khối thay đổi.</param>
        /// <param name="oldBlock">Loại khối cũ.</param>
        /// <param name="newBlock">Loại khối mới.</param>
        public BlockChangedEventArgs(BlockPos position, BlockId oldBlock, BlockId newBlock)
        {
            _position = position;
            _oldBlock = oldBlock;
            _newBlock = newBlock;
        }
    }
}

