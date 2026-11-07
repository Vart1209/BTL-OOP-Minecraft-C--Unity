# Project Rules – Minecraft Voxel OOP (Unity 6 C#)

## 1. Quy tắc Tự động ghi Nhật ký Phiên bản (`CHANGELOG.md`)

Nhóm sử dụng AI làm trợ lý chính. Để đảm bảo minh bạch và nắm rõ mọi thay đổi khi vấn đáp, **MỌI lần AI tạo file mới, sửa code, xóa file, hoặc thay đổi cấu hình dự án**, AI **BẮT BUỘC** phải cập nhật file [`CHANGELOG.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/CHANGELOG.md) ở thư mục gốc trước khi kết thúc câu trả lời.

### 1.1 Quy tắc tự động đánh số phiên bản (Semantic Versioning `vMAJOR.MINOR.PATCH`)
Đọc phiên bản mới nhất ở đầu file `CHANGELOG.md` và tự động tăng số theo mức độ thay đổi:
- **PATCH (`vX.Y.Z` → `vX.Y.(Z+1)`) — Thay đổi nhỏ:**
  - Sửa bug nhỏ, refactor nội bộ 1 lớp, thêm unit test, chỉnh comment/tài liệu, tinh chỉnh thông số config/UI.
- **MINOR (`vX.Y.Z` → `vX.(Y+1).0`) — Thay đổi vừa / Tính năng mới:**
  - Thêm mới class/interface/service trong Domain hoặc Application, hoàn thành 1 Use Case mới, thêm Design Pattern mới, thêm hệ thống mới (vd: Inventory, Crafting, Mob, Save/Load, Resource Pack).
- **MAJOR (`vX.Y.Z` → `v(X+1).0.0`) — Thay đổi lớn / Cột mốc chính:**
  - Khởi tạo kiến trúc hoặc thay đổi kiến trúc phân tầng, đổi hợp đồng interface giữa 2 phân hệ, hoặc hoàn thành cột mốc lớn (`v1.0.0` cho MVP hoàn chỉnh).

### 1.2 Định dạng bắt buộc cho mỗi bản ghi (chèn lên ĐẦU danh sách trong `CHANGELOG.md`)
Lấy đúng ngày giờ hệ thống thực tế (`YYYY-MM-DD HH:MM +07:00`) khi ghi:

```markdown
## [vX.Y.Z] - YYYY-MM-DD HH:MM (+07:00) — <Tóm tắt ngắn gọn thay đổi>
- **Loại cập nhật:** Major | Minor | Patch
- **Phân hệ:** World (A) | Survival (B) | Shared/Setup
- **Thêm mới (Added):**
  - `đường/dẫn/file` — mô tả ngắn những gì vừa thêm
- **Chỉnh sửa (Changed):**
  - `đường/dẫn/file` — mô tả những gì thay đổi và lý do
- **Xóa bỏ (Removed):**
  - `đường/dẫn/file` — (hoặc ghi "Không")
- **Giải thích OOP / AI (dùng khi vấn đáp):**
  - Giải thích ngắn gọn nguyên lý OOP / Design Pattern áp dụng trong lần cập nhật này để thành viên đọc hiểu ngay.
```

---

## 2. Quy tắc Kiến trúc & OOP (Bám sát Barem [S2])

1. **Kiến trúc 4 Assembly (`.asmdef`):**
   - `Minecraft.Domain`: **C# thuần 100%**, bật `noEngineReferences: true`. Tuyệt đối không `using UnityEngine;`.
   - `Minecraft.Application`: chỉ tham chiếu `Minecraft.Domain`. Chứa các Service điều phối Use Case.
   - `Minecraft.Infrastructure`: tham chiếu `Domain` + `Application`. Chứa JSON Save/Load, ConfigLoader, ResourcePack, bọc `try-catch` đầy đủ.
   - `Minecraft.Presentation`: tham chiếu cả 3 tầng trên. Chứa `MonoBehaviour`, `ScriptableObject`, UI, View.
2. **Nguyên lý OOP bắt buộc:**
   - Mọi field trong Domain phải là `private` (hoặc `protected` nếu cần cho kế thừa hợp lý), truy cập qua property và kiểm tra hợp lệ dữ liệu đầu vào tại các phương thức.
   - Không dùng Singleton cho logic nghiệp vụ. Khởi tạo và tiêm phụ thuộc (Dependency Injection) qua constructor tại `GameBootstrap`.
   - Giao tiếp giữa phân hệ World (A) và Survival (B) đi qua interface (`IWorldReader`, `IWorldWriter`).
3. **Coding Convention:**
   - `PascalCase` cho Class, Struct, Interface (`I...`), Method, Property, Public Event.
   - `_camelCase` cho private field, `camelCase` cho biến cục bộ và tham số.
   - Mỗi class/interface chính tách 1 file `.cs` riêng khớp tên.
