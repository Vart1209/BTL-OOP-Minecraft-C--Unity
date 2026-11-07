# Nhật ký Phiên bản (Changelog)

Tất cả các thay đổi của dự án được AI và các thành viên ghi nhận tự động tại đây theo quy tắc trong [`AGENTS.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/AGENTS.md).
Định dạng đánh số phiên bản: **Semantic Versioning (`vMAJOR.MINOR.PATCH`)**.

---

## [v0.1.0] - 2026-11-07 10:08 (+07:00) — Khởi tạo kế hoạch Brainstorm, Rule AI và khung kiến trúc 3 tầng Unity
- **Loại cập nhật:** Minor (Khởi tạo dự án `v0.1.0`)
- **Phân hệ:** Shared/Setup (Cả 2 thành viên A & B)
- **Thêm mới (Added):**
  - `docs/brainstorm.md` — Tài liệu tổng hợp toàn bộ tiêu chí barem [S2], phạm vi MVP/Nên có/Mở rộng, danh sách 21 lớp thực thể, 4 Design Pattern, 12 quy tắc nghiệp vụ, lộ trình 5,5 tuần và ánh xạ 20 tiêu chí.
  - `AGENTS.md` — Quy tắc bắt buộc cho AI: tự động cập nhật `CHANGELOG.md` kèm ngày giờ, đánh số phiên bản Major/Minor/Patch, và giữ nghiêm ngặt ranh giới 4 tầng `.asmdef` + chuẩn OOP.
  - `CHANGELOG.md` — File nhật ký theo dõi từng phiên bản cập nhật của dự án.
  - `README.md` — Hướng dẫn cấu trúc dự án, cách cài Unity 6.3 LTS (`6000.3.28f1`) và cấu hình Git cho 2 thành viên.
  - `.gitignore`, `.editorconfig` — Bộ lọc file rác Unity/macOS và chuẩn định dạng code C# thống nhất giữa 2 máy.
  - `Game/Assets/_Project/Scripts/Domain/Minecraft.Domain.asmdef` — Assembly tầng Domain (bật `noEngineReferences: true` để trình biên dịch chặn mọi phụ thuộc vào `UnityEngine`).
  - `Game/Assets/_Project/Scripts/Application/Minecraft.Application.asmdef` — Assembly tầng Application (chỉ phụ thuộc `Minecraft.Domain`).
  - `Game/Assets/_Project/Scripts/Infrastructure/Minecraft.Infrastructure.asmdef` — Assembly tầng Infrastructure (lưu/tải file, đọc config, resource pack).
  - `Game/Assets/_Project/Scripts/Presentation/Minecraft.Presentation.asmdef` — Assembly tầng Presentation (MonoBehaviour, ScriptableObject, UI).
  - `Game/Assets/_Project/Tests/EditMode/Minecraft.Domain.Tests.asmdef` — Assembly kiểm thử NUnit EditMode.
  - `Game/Assets/StreamingAssets/config.json` — File cấu hình thông số thế giới, người chơi và resource pack (đáp ứng tiêu chí tách file cấu hình, không gán cứng đường dẫn).
  - Cây thư mục con trong `Game/Assets/_Project/` (`World`, `Blocks`, `Entities`, `Items`, `Crafting`, `GameModes`, `Data`, `Scenes`, `Prefabs`, `Art`).
- **Chỉnh sửa (Changed):**
  - Không
- **Xóa bỏ (Removed):**
  - Không
- **Giải thích OOP / AI (dùng khi vấn đáp):**
  - **Kiến trúc phân tầng (Tiêu chí 8):** Tách 4 file `.asmdef` giúp kiểm soát chiều gọi lệnh ngay từ trình biên dịch: `Presentation -> Application -> Domain`, trong đó `Domain` là C# thuần không chạm tới `UnityEngine`, giúp viết ≥20 Unit Test dễ dàng và chứng minh rõ ranh giới hệ thống (Tiêu chí 4, 18).
  - **Quản lý cấu hình & đường dẫn tương đối (Tiêu chí 17, 19):** Đặt `config.json` trong `StreamingAssets` để khi build ra macOS hoặc Windows đều đọc được qua `Application.streamingAssetsPath`.
