using Minecraft.Domain.Configuration;

namespace Minecraft.Domain.Contracts
{
    /// <summary>
    /// Hợp đồng dịch vụ đọc cấu hình trò chơi từ tầng hạ tầng (Config Loader Contract).
    /// <para>
    /// <b>Nguyên lý OOP áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Dependency Inversion Principle (DIP - Tiêu chí 16):</b> Các dịch vụ ứng dụng phụ thuộc vào hợp đồng trừu tượng <see cref="IConfigLoader"/> thay vì gắn cứng vào lớp đọc file cụ thể.</item>
    ///   <item><b>Khả năng chịu lỗi (Fault Tolerance - Tiêu chí 19):</b> Luôn đảm bảo trả về cấu hình hợp lệ, không gây crash chương trình khi file bị thiếu hoặc hỏng.</item>
    /// </list>
    /// </para>
    /// </summary>
    public interface IConfigLoader
    {
        /// <summary>
        /// Thông báo cảnh báo gần nhất nếu quá trình đọc cấu hình gặp sự cố và phải dùng giá trị mặc định.
        /// </summary>
        string LastWarning { get; }

        /// <summary>
        /// Đọc và phân tích file cấu hình từ đường dẫn chỉ định.
        /// </summary>
        /// <param name="filePath">Đường dẫn tuyệt đối hoặc tương đối tới file JSON cấu hình.</param>
        /// <returns>Đối tượng cấu hình trò chơi hợp lệ.</returns>
        GameConfigDto LoadConfig(string filePath);
    }
}
