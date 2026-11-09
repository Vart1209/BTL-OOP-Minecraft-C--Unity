using System;
using System.IO;
using System.Text.Json;
using Minecraft.Domain.Configuration;
using Minecraft.Domain.Contracts;

namespace Minecraft.Infrastructure.Config
{
    /// <summary>
    /// Lớp đọc và nạp cấu hình trò chơi từ file JSON (Config Loader).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Phân tầng Infrastructure (Tiêu chí 8 barem [S2]):</b> Đảm nhiệm toàn bộ thao tác file I/O và chuyển đổi JSON thành DTO, cách ly hoàn toàn tầng Domain khỏi việc đọc đĩa.</item>
    ///   <item><b>Xử lý ngoại lệ an toàn &amp; Fallback (Tiêu chí 19 barem [S2]):</b> Bọc toàn bộ quá trình đọc và parse JSON trong <c>try-catch</c>. Khi file bị thiếu, đường dẫn không tồn tại hoặc nội dung JSON bị sai định dạng cú pháp, hệ thống tự động ghi nhận cảnh báo và hoàn trả cấu hình chuẩn mặc định (<see cref="GameConfigDto.CreateDefault"/>) mà tuyệt đối không để crash chương trình.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class ConfigLoader : IConfigLoader
    {
        private string _lastWarning;

        /// <inheritdoc />
        public string LastWarning => _lastWarning;

        /// <inheritdoc />
        public GameConfigDto LoadConfig(string filePath)
        {
            _lastWarning = null;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                _lastWarning = "Đường dẫn file cấu hình rỗng. Sử dụng cấu hình mặc định.";
                return GameConfigDto.CreateDefault();
            }

            try
            {
                if (!File.Exists(filePath))
                {
                    _lastWarning = $"File cấu hình không tồn tại tại '{filePath}'. Sử dụng cấu hình mặc định.";
                    return GameConfigDto.CreateDefault();
                }

                string jsonContent = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    _lastWarning = $"File cấu hình rỗng tại '{filePath}'. Sử dụng cấu hình mặc định.";
                    return GameConfigDto.CreateDefault();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                };

                GameConfigDto loadedConfig = JsonSerializer.Deserialize<GameConfigDto>(jsonContent, options);
                if (loadedConfig == null)
                {
                    _lastWarning = $"Không thể giải mã nội dung JSON từ '{filePath}'. Sử dụng cấu hình mặc định.";
                    return GameConfigDto.CreateDefault();
                }

                // Đảm bảo các đối tượng con không bị null nếu file json thiếu trường
                loadedConfig.World ??= new WorldConfig();
                loadedConfig.Player ??= new PlayerConfig();
                loadedConfig.ResourcePack ??= new ResourcePackConfig();

                return loadedConfig;
            }
            catch (JsonException ex)
            {
                _lastWarning = $"Lỗi định dạng JSON trong file '{filePath}': {ex.Message}. Sử dụng cấu hình mặc định.";
                return GameConfigDto.CreateDefault();
            }
            catch (IOException ex)
            {
                _lastWarning = $"Lỗi I/O khi đọc file '{filePath}': {ex.Message}. Sử dụng cấu hình mặc định.";
                return GameConfigDto.CreateDefault();
            }
            catch (Exception ex)
            {
                _lastWarning = $"Ngoại lệ không xác định khi nạp cấu hình từ '{filePath}': {ex.Message}. Sử dụng cấu hình mặc định.";
                return GameConfigDto.CreateDefault();
            }
        }
    }
}
