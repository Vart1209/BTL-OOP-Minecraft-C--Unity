using System;

namespace Minecraft.Domain.Contracts
{
    /// <summary>
    /// Hợp đồng giao diện chỉ đọc (Read-only Interface) cung cấp thông tin trạng thái khối trong thế giới Voxel.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Interface Segregation Principle (ISP - Tiêu chí 15):</b> Tách riêng quyền đọc thế giới (<see cref="IWorldReader"/>) khỏi quyền ghi thế giới (<see cref="IWorldWriter"/>). Các lớp chỉ cần truy vấn như <c>VoxelRaycaster</c>, <c>ChunkMeshBuilder</c> hoặc AI của <c>Mob</c> chỉ nhận <see cref="IWorldReader"/>, đảm bảo không thể vô tình làm thay đổi dữ liệu khối.</item>
    ///   <item><b>Dependency Inversion Principle (DIP - Tiêu chí 16):</b> Phân hệ Survival (Thành viên B) phụ thuộc vào interface này thay vì phụ thuộc trực tiếp vào lớp <c>World</c> của Thành viên A, giúp B dễ dàng viết Unit Test bằng một thế giới phẳng giả lập.</item>
    ///   <item><b>Observer Pattern (Tiêu chí 9, 10, 11):</b> Sự kiện <see cref="BlockChanged"/> thông báo cho tầng Presentation (như <c>ChunkView</c>) tự động cập nhật lại lưới hiển thị mà Domain không cần biết tới Unity.</item>
    /// </list>
    /// </para>
    /// </summary>
    public interface IWorldReader
    {
        /// <summary>
        /// Chiều cao tối đa của thế giới (mặc định 64 theo cấu hình Chunk 16×16×64).
        /// Mọi tọa độ hợp lệ đều có <c>0 &lt;= Y &lt; MaxHeight</c> (Quy tắc nghiệp vụ R3).
        /// </summary>
        int MaxHeight { get; }

        /// <summary>
        /// Sự kiện phát ra mỗi khi một khối tại vị trí bất kỳ bị thay đổi loại khối (Observer Pattern).
        /// </summary>
        event Action<BlockChangedEventArgs> BlockChanged;

        /// <summary>
        /// Kiểm tra một tọa độ khối có nằm trong giới hạn chiều cao hợp lệ của thế giới hay không (Quy tắc R3).
        /// </summary>
        /// <param name="position">Tọa độ khối cần kiểm tra.</param>
        /// <returns><c>true</c> nếu <c>0 &lt;= position.Y &lt; MaxHeight</c>; ngược lại trả về <c>false</c>.</returns>
        bool IsInBounds(BlockPos position);

        /// <summary>
        /// Lấy mã định danh loại khối tại tọa độ toàn cục được chỉ định.
        /// Nếu tọa độ nằm ngoài biên chiều cao hoặc thuộc Chunk chưa sinh, phương thức trả về <see cref="BlockId.Air"/>.
        /// </summary>
        /// <param name="position">Tọa độ toàn cục của khối.</param>
        /// <returns>Mã loại khối (<see cref="BlockId"/>) tại vị trí đó.</returns>
        BlockId GetBlock(BlockPos position);
    }
}

