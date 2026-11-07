# Nhật ký Phiên bản (Changelog)

Tất cả các thay đổi của dự án được AI và các thành viên ghi nhận tự động tại đây theo quy tắc trong [`AGENTS.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/AGENTS.md).
Định dạng đánh số phiên bản: **Semantic Versioning (`vMAJOR.MINOR.PATCH`)**.

---

## [v0.2.0] - 2026-11-07 11:09 (+07:00) — Hoàn thành Tuần T0: Chuẩn hóa Unity 6000.6.8f1, tích hợp Resource Pack và xây dựng Bộ Hợp đồng chung tầng Domain
- **Loại cập nhật:** Minor
- **Phân hệ:** Shared/Setup (Hợp đồng chung giữa Thành viên A & Thành viên B)
- **Thêm mới (Added):**
  - `Game/ProjectSettings/ProjectVersion.txt` — Chốt phiên bản Unity Editor `6000.6.8f1 (237b6593792b)` có sẵn trên máy.
  - `Game/Assets/_Project/Scripts/Domain/Contracts/ChunkCoord.cs` — Kiểu giá trị bất biến (`readonly struct`) biểu diễn tọa độ Chunk 2D `(X, Z)`, thực thi `IEquatable<ChunkCoord>`.
  - `Game/Assets/_Project/Scripts/Domain/Contracts/BlockPos.cs` — Kiểu giá trị bất biến biểu diễn tọa độ khối 3D `(X, Y, Z)` thuần C# thay thế `Vector3Int`, hỗ trợ phép chia `FloorDiv`/`FloorMod` xử lý chính xác tọa độ âm khi quy đổi sang `ChunkCoord` và tọa độ cục bộ.
  - `Game/Assets/_Project/Scripts/Domain/Contracts/BlockId.cs` — Enum định danh các loại khối (`byte`, `Air = 0`, `Bedrock = 1`, `Stone`, `Dirt`, `Grass`, `Wood`, `Leaves`, `Sand`, `Planks`, `CraftingTable`, `Cobblestone`).
  - `Game/Assets/_Project/Scripts/Domain/Contracts/ItemId.cs` — Enum định danh vật phẩm (`ushort`), đồng bộ mã số 1-1 với `BlockId` cho dải `1..99` và tách dải công cụ (`100+`), thức ăn (`200+`).
  - `Game/Assets/_Project/Scripts/Domain/Contracts/BlockChangedEventArgs.cs` — Gói dữ liệu sự kiện bất biến dùng cho Observer Pattern khi khối trong thế giới thay đổi.
  - `Game/Assets/_Project/Scripts/Domain/Contracts/IWorldReader.cs` — Hợp đồng chỉ đọc trạng thái thế giới (`MaxHeight`, `IsInBounds`, `GetBlock`, sự kiện `BlockChanged`).
  - `Game/Assets/_Project/Scripts/Domain/Contracts/IWorldWriter.cs` — Hợp đồng ghi/cập nhật khối trong thế giới (`SetBlock`).
  - `Game/Assets/_Project/Scripts/Domain/Contracts/ITerrainGenerator.cs` — Hợp đồng Strategy Pattern cho thuật toán sinh địa hình theo seed (`GetBlockAt`).
  - `Game/Assets/_Project/Scripts/Domain/Contracts/IGameMode.cs` — Hợp đồng State Pattern & phân quyền tác nhân giữa Survival và Creative (`CanTakeDamage`, `ConsumesItemsOnPlace`, `InstantBreak`, `CanFly`, `CanBreakBlock`).
  - `Game/Assets/_Project/Tests/EditMode/DomainContractsTests.cs` — Bộ 10 Unit Test NUnit kiểm thử toàn diện hợp đồng chung tầng Domain.
  - `docs/superpowers/plans/2026-11-07-t0-setup-and-domain-contracts.md` — Kế hoạch triển khai chi tiết Tuần T0 theo quy trình TDD.
- **Chỉnh sửa (Changed):**
  - `docs/brainstorm.md`, `README.md` — Cập nhật yêu cầu phiên bản Unity sang `6000.6.8f1` (hoặc bản Unity 6 mới nhất thống nhất giữa 2 máy).
  - `Game/Assets/StreamingAssets/config.json` — Bổ sung cấu hình `OptimumRealism_64x` vào danh sách resource pack.
  - `Game/Assets/_Project/Scripts/Domain/Contracts/ChunkCoord.cs` — Chuẩn hóa XML comment hoàn toàn sang tiếng Việt thuần.
  - Tối ưu gói `Game/Assets/StreamingAssets/ResourcePacks/OptimumRealism_64x`: chỉ giữ lại `pack.mcmeta`, `pack.png`, `credits.txt` và texture khối gốc trong `textures/block/`, loại bỏ các file normal/specular map và các thư mục chưa dùng để giảm dung lượng xuống ~9.1 MB; dọn sạch index git bị stage nhầm từ thư mục ngoài.
- **Xóa bỏ (Removed):**
  - Không
- **Giải thích OOP / AI (dùng khi vấn đáp):**
  - **Đóng gói & Bất biến (Encapsulation & Immutability - Tiêu chí 13):** Mọi trường dữ liệu trong `BlockPos`, `ChunkCoord`, `BlockChangedEventArgs` đều là `private readonly`, truy cập qua Property và có kiểm tra hợp lệ đầu vào (`chunkWidth <= 0` ném `ArgumentOutOfRangeException`).
  - **Phân tách Interface & Đảo ngược phụ thuộc (ISP & DIP - Tiêu chí 15, 16):** Tách `IWorldReader` khỏi `IWorldWriter` để các lớp chỉ đọc (AI Mob, Raycaster, MeshBuilder) không thể làm hỏng dữ liệu khối; giúp Thành viên B tạo thế giới giả lập (`FakeContractWorld`) để test độc lập mà không cần chờ Thành viên A code xong `World`.
  - **Mẫu thiết kế (Strategy, Observer, State - Tiêu chí 4, 9, 10, 11):** Định nghĩa rõ hợp đồng `ITerrainGenerator` (Strategy), `IWorldReader.BlockChanged` (Observer) và `IGameMode` (State + Phân quyền tác nhân Survival/Creative).

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
