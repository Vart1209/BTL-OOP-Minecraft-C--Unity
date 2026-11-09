using System;
using System.IO;
using System.Text.Json;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.Persistence;

namespace Minecraft.Infrastructure.Persistence
{
    /// <summary>
    /// Triển khai kho lưu trữ thế giới sử dụng định dạng JSON thuần C# (JSON World Repository).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Thực thi hợp đồng Repository (Tiêu chí 16):</b> Cài đặt cụ thể <see cref="IWorldRepository"/> trong tầng hạ tầng Infrastructure.</item>
    ///   <item><b>Xử lý ngoại lệ toàn diện (Tiêu chí 19 barem [S2]):</b> Mọi thao tác I/O ghi đĩa và phân tích cú pháp JSON đều được bọc trong các khối <c>try-catch</c> phân cấp rõ ràng, bắt đúng các loại ngoại lệ I/O chuyên biệt, ghi nhận thông báo lỗi chi tiết vào <see cref="LastError"/> và trả về <c>false</c> mà không bao giờ để sập ứng dụng.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class JsonWorldRepository : IWorldRepository
    {
        private string _lastError;

        /// <inheritdoc />
        public string LastError => _lastError;

        /// <inheritdoc />
        public bool SaveWorld(string saveFilePath, WorldSaveDto worldData)
        {
            _lastError = null;

            if (string.IsNullOrWhiteSpace(saveFilePath))
            {
                _lastError = "Đường dẫn file lưu không hợp lệ (rỗng).";
                return false;
            }

            if (worldData == null)
            {
                _lastError = "Dữ liệu thế giới (WorldSaveDto) cần lưu là null.";
                return false;
            }

            try
            {
                string directory = Path.GetDirectoryName(saveFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true
                };

                string jsonString = JsonSerializer.Serialize(worldData, options);
                File.WriteAllText(saveFilePath, jsonString);
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                _lastError = $"Không có quyền ghi vào file '{saveFilePath}': {ex.Message}";
                return false;
            }
            catch (IOException ex)
            {
                _lastError = $"Lỗi I/O khi lưu file '{saveFilePath}': {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                _lastError = $"Ngoại lệ không xác định khi lưu file '{saveFilePath}': {ex.Message}";
                return false;
            }
        }

        /// <inheritdoc />
        public bool TryLoadWorld(string saveFilePath, out WorldSaveDto worldData)
        {
            _lastError = null;
            worldData = null;

            if (string.IsNullOrWhiteSpace(saveFilePath))
            {
                _lastError = "Đường dẫn file tải không hợp lệ (rỗng).";
                return false;
            }

            try
            {
                if (!File.Exists(saveFilePath))
                {
                    _lastError = $"File lưu không tồn tại tại đường dẫn '{saveFilePath}'.";
                    return false;
                }

                string jsonString = File.ReadAllText(saveFilePath);
                if (string.IsNullOrWhiteSpace(jsonString))
                {
                    _lastError = $"File lưu rỗng tại '{saveFilePath}'.";
                    return false;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                worldData = JsonSerializer.Deserialize<WorldSaveDto>(jsonString, options);
                if (worldData == null)
                {
                    _lastError = $"Không thể deserialize dữ liệu từ '{saveFilePath}'.";
                    return false;
                }

                return true;
            }
            catch (JsonException ex)
            {
                _lastError = $"File lưu bị hỏng cú pháp JSON tại '{saveFilePath}': {ex.Message}";
                return false;
            }
            catch (IOException ex)
            {
                _lastError = $"Lỗi I/O khi đọc file '{saveFilePath}': {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                _lastError = $"Ngoại lệ không xác định khi tải file '{saveFilePath}': {ex.Message}";
                return false;
            }
        }
    }
}
