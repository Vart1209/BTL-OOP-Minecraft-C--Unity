using Minecraft.Domain.Persistence;

namespace Minecraft.Domain.Contracts
{
    /// <summary>
    /// Hợp đồng lưu trữ và khôi phục dữ liệu thế giới (World Repository Contract).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Design Pattern áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Repository Pattern (Tiêu chí 16 barem [S2]):</b> Trừu tượng hóa tầng lưu trữ, che giấu chi tiết ghi đọc file vật lý (JSON hay Binary) đằng sau interface.</item>
    ///   <item><b>Dependency Inversion Principle (DIP):</b> Tầng Application điều phối lưu/tải dựa trên hợp đồng <see cref="IWorldRepository"/>, dễ dàng thay thế từ <c>JsonWorldRepository</c> sang <c>BinaryWorldRepository</c> mà không cần sửa một dòng code trong Application.</item>
    /// </list>
    /// </para>
    /// </summary>
    public interface IWorldRepository
    {
        /// <summary>
        /// Thông báo lỗi gần nhất khi thao tác lưu hoặc tải gặp sự cố.
        /// </summary>
        string LastError { get; }

        /// <summary>
        /// Lưu dữ liệu DTO thế giới ra file vật lý.
        /// </summary>
        /// <param name="saveFilePath">Đường dẫn file đích.</param>
        /// <param name="worldData">Đối tượng DTO dữ liệu thế giới.</param>
        /// <returns><c>true</c> nếu lưu thành công; ngược lại <c>false</c>.</returns>
        bool SaveWorld(string saveFilePath, WorldSaveDto worldData);

        /// <summary>
        /// Đọc và tải dữ liệu DTO thế giới từ file vật lý.
        /// </summary>
        /// <param name="saveFilePath">Đường dẫn file nguồn.</param>
        /// <param name="worldData">Đối tượng DTO được nạp; hoặc <c>null</c> nếu thất bại.</param>
        /// <returns><c>true</c> nếu tải thành công; ngược lại <c>false</c>.</returns>
        bool TryLoadWorld(string saveFilePath, out WorldSaveDto worldData);
    }
}
