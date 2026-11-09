using System;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.World
{
    /// <summary>
    /// Đơn vị phân vùng không gian 3 chiều cơ bản của thế giới Voxel với kích thước cố định 16x64x16.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Tối ưu bộ nhớ (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Đóng gói (Encapsulation - Tiêu chí 13):</b> Mảng <c>_blocks</c> là <c>private readonly BlockId[]</c> 1 chiều (16x64x16 = 16.384 phần tử kiểu byte = 16 KB) để tối ưu bộ nhớ cache CPU và loại bỏ phân mảnh rác GC. Mọi truy cập phải qua <see cref="GetBlock"/> và <see cref="SetBlock"/> có kiểm tra biên nghiêm ngặt.</item>
    ///   <item><b>Quản lý trạng thái (State Tracking):</b> Cờ <see cref="IsDirty"/> đánh dấu Chunk có khối bị thay đổi hay không, giúp hệ thống dựng lưới chỉ tính toán lại Mesh cho những Chunk thực sự bẩn thay vì dựng lại toàn bộ thế giới.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class Chunk
    {
        /// <summary>Chiều rộng mỗi cạnh ngang của Chunk (trục X và Z).</summary>
        public const int Width = 16;

        /// <summary>Chiều cao tối đa của Chunk (trục Y).</summary>
        public const int Height = 64;

        private readonly ChunkCoord _coordinate;
        private readonly BlockId[] _blocks;
        private bool _isDirty;

        /// <summary>Tọa độ 2D của Chunk trên lưới thế giới.</summary>
        public ChunkCoord Coordinate => _coordinate;

        /// <summary>Trạng thái bẩn cần dựng lại Mesh hiển thị.</summary>
        public bool IsDirty => _isDirty;

        /// <summary>
        /// Khởi tạo một Chunk mới tại tọa độ xác định. Toàn bộ các khối ban đầu là <see cref="BlockId.Air"/>.
        /// </summary>
        /// <param name="coordinate">Tọa độ 2D của Chunk.</param>
        public Chunk(ChunkCoord coordinate)
        {
            _coordinate = coordinate;
            _blocks = new BlockId[Width * Height * Width];
            _isDirty = false;
        }

        /// <summary>
        /// Đánh dấu Chunk đã được dựng Mesh xong, không còn bẩn.
        /// </summary>
        public void MarkClean()
        {
            _isDirty = false;
        }

        /// <summary>
        /// Đánh dấu Chunk bị bẩn thủ công (ví dụ khi Chunk hàng xóm ở mép thay đổi).
        /// </summary>
        public void MarkDirty()
        {
            _isDirty = true;
        }

        /// <summary>
        /// Lấy mã định danh loại khối tại tọa độ cục bộ (x, y, z) bên trong Chunk.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Ném ra nếu tọa độ vượt ra ngoài phạm vi [0..15, 0..63, 0..15].</exception>
        public BlockId GetBlock(int x, int y, int z)
        {
            ValidateBounds(x, y, z);
            return _blocks[GetIndex(x, y, z)];
        }

        /// <summary>
        /// Gán loại khối tại tọa độ cục bộ (x, y, z) bên trong Chunk.
        /// </summary>
        /// <returns><c>true</c> nếu khối thực sự thay đổi và Chunk chuyển sang bẩn; <c>false</c> nếu loại khối trùng khớp khối cũ.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Ném ra nếu tọa độ vượt ra ngoài phạm vi [0..15, 0..63, 0..15].</exception>
        public bool SetBlock(int x, int y, int z, BlockId blockId)
        {
            ValidateBounds(x, y, z);
            int index = GetIndex(x, y, z);
            if (_blocks[index] == blockId)
            {
                return false;
            }

            _blocks[index] = blockId;
            _isDirty = true;
            return true;
        }

        private static int GetIndex(int x, int y, int z)
        {
            return (y * Width + z) * Width + x;
        }

        private static void ValidateBounds(int x, int y, int z)
        {
            if (x < 0 || x >= Width)
            {
                throw new ArgumentOutOfRangeException(nameof(x), $"Tọa độ X cục bộ ({x}) phải nằm trong khoảng [0, {Width - 1}].");
            }

            if (y < 0 || y >= Height)
            {
                throw new ArgumentOutOfRangeException(nameof(y), $"Tọa độ Y cục bộ ({y}) phải nằm trong khoảng [0, {Height - 1}].");
            }

            if (z < 0 || z >= Width)
            {
                throw new ArgumentOutOfRangeException(nameof(z), $"Tọa độ Z cục bộ ({z}) phải nằm trong khoảng [0, {Width - 1}].");
            }
        }
    }
}
