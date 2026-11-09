# Nhật ký Phiên bản (Changelog)

Tất cả các thay đổi của dự án được AI và các thành viên ghi nhận tự động tại đây theo quy tắc trong [`AGENTS.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/AGENTS.md).
Định dạng đánh số phiên bản: **Semantic Versioning (`vMAJOR.MINOR.PATCH`)**.

---

## [v0.3.0] - 2026-11-09 20:20 (+07:00) — Hoàn thành Tuần T1: Triển khai Hệ thống World/Chunk/Mesh (Phân hệ A) và Items/Inventory/Player (Phân hệ B)
- **Loại cập nhật:** Minor
- **Phân hệ:** World (A) & Survival (B)
- **Thêm mới (Added):**
  - **Phân hệ World (Thành viên A):**
    - `Game/Assets/_Project/Scripts/Domain/Blocks/BlockFace.cs` — Enum định danh 6 mặt khối (`Top`, `Bottom`, `North`, `South`, `East`, `West`).
    - `Game/Assets/_Project/Scripts/Domain/Blocks/BlockType.cs` — Thực thể kiểu khối, đóng gói toàn bộ thuộc tính, kiểm tra hợp lệ độ cứng và ánh xạ chỉ số texture theo từng mặt.
    - `Game/Assets/_Project/Scripts/Domain/Blocks/BlockRegistry.cs` — Bảng tra cứu danh mục khối, hàm `CreateDefault()` cài đặt Quy tắc R5 (phá Stone rơi Cobblestone, Grass rơi Dirt, Leaves rơi None).
    - `Game/Assets/_Project/Scripts/Domain/World/Chunk.cs` — Cấu trúc dữ liệu khối 16x64x16 với mảng 1D tối ưu hóa cache, quản lý cờ trạng thái bẩn `IsDirty` và kiểm tra biên tọa độ nghiêm ngặt.
    - `Game/Assets/_Project/Scripts/Domain/World/FlatTerrainGenerator.cs` — Thực thi `ITerrainGenerator` (Strategy Pattern) cho địa hình phẳng phục vụ kiểm thử và sandbox.
    - `Game/Assets/_Project/Scripts/Domain/World/PerlinTerrainGenerator.cs` — Thực thi `ITerrainGenerator` bằng thuật toán nhiễu Perlin thuần C# tất định kết hợp đa thức nội suy mượt bậc 5 (không phụ thuộc UnityEngine).
    - `Game/Assets/_Project/Scripts/Domain/World/World.cs` — Thực thi `IWorldReader` và `IWorldWriter` (quan hệ Hợp thành `World ◆ Chunk`), quản lý sinh/tải Chunk, kiểm soát biên độ cao (Quy tắc R3) và phát sự kiện `BlockChanged` (Observer Pattern).
    - `Game/Assets/_Project/Scripts/Domain/World/ChunkMeshData.cs` — Gói hình học lưới 3D thuần C# (`Vertices`, `Triangles`, `Uvs`, `Normals`) sử dụng `System.Numerics`.
    - `Game/Assets/_Project/Scripts/Domain/World/ChunkMeshBuilder.cs` — Thuật toán dựng lưới với Face Culling loại bỏ mặt khuất kề nhau, giảm mạnh số lượng tam giác render.
    - `Game/Assets/_Project/Tests/EditMode/WorldDomainTests.cs` — Bộ 9 Unit Test NUnit kiểm thử độc lập phân hệ World.
  - **Phân hệ Survival & Presentation (Thành viên B):**
    - `Game/Assets/_Project/Scripts/Domain/Items/ToolType.cs` — Enum phân loại công cụ (`None`, `Pickaxe`, `Axe`, `Shovel`, `Sword`).
    - `Game/Assets/_Project/Scripts/Domain/Items/ItemUseContext.cs` — Parameter Object đóng gói ngữ cảnh kích hoạt vật phẩm.
    - `Game/Assets/_Project/Scripts/Domain/Items/Item.cs` — Lớp trừu tượng định nghĩa vật phẩm, kiểm soát số lượng xếp chồng 1..64 (Quy tắc R6) và phương thức thuần ảo đa hình `Use()`.
    - `Game/Assets/_Project/Scripts/Domain/Items/BlockItem.cs` — Lớp con kế thừa `Item`, ghi đè `Use()` kiểm tra ô trống và đặt khối vào thế giới (Quy tắc R2).
    - `Game/Assets/_Project/Scripts/Domain/Items/ToolItem.cs` — Lớp con kế thừa `Item` với `MaxStackSize = 1` (Quy tắc R6), tính thời gian đào theo loại công cụ và độ cứng khối (Quy tắc R4).
    - `Game/Assets/_Project/Scripts/Domain/Items/FoodItem.cs` — Lớp con kế thừa `Item` với thuộc tính hồi điểm đói và máu (Quy tắc R8).
    - `Game/Assets/_Project/Scripts/Domain/Items/ItemRegistry.cs` — Sổ đăng ký danh mục toàn bộ vật phẩm (BlockItem, ToolItem, FoodItem).
    - `Game/Assets/_Project/Scripts/Domain/Items/ItemStack.cs` — Quan hệ Kết tập `ItemStack ◇ Item`, đóng gói phương thức `Add()` và `Remove()` chống tràn stack.
    - `Game/Assets/_Project/Scripts/Domain/Items/Inventory.cs` — Quản lý 36 ô túi đồ (9 Hotbar + 27 ô chính), tự động cộng dồn trước khi chiếm ô mới, chọn ô Hotbar, sự kiện `InventoryChanged` và phương thức `RemoveItem` atomic (Quy tắc R10).
    - `Game/Assets/_Project/Tests/EditMode/InventoryAndItemsTests.cs` — Bộ 6 Unit Test NUnit kiểm thử đa hình vật phẩm và logic túi đồ.
    - `Game/Assets/_Project/Scripts/Presentation/World/ChunkView.cs` — MonoBehaviour chuyển đổi `ChunkMeshData` sang `UnityEngine.Mesh` gán vào MeshFilter & MeshCollider.
    - `Game/Assets/_Project/Scripts/Presentation/Player/FirstPersonPlayerController.cs` — MonoBehaviour điều khiển FPS camera, di chuyển/nhảy/bay theo `IGameMode.CanFly` (phân quyền tác nhân) và đổi ô Hotbar qua phím 1..9 / con lăn chuột.
    - `docs/superpowers/plans/2026-11-07-t1-world-and-inventory.md` — Kế hoạch triển khai chi tiết Tuần T1.
- **Chỉnh sửa (Changed):**
  - Không
- **Xóa bỏ (Removed):**
  - Không
- **Giải thích OOP / AI (dùng khi vấn đáp):**
  - **Kế thừa, Đa hình & Liskov Substitution (Tiêu chí 14):** `BlockItem`, `ToolItem`, `FoodItem` kế thừa lớp cha `Item`, ghi đè phương thức `Use()` để thể hiện các hành vi nghiệp vụ đặc thù nhưng đều có thể thay thế hoàn hảo cho `Item` trong `ItemStack` và `Inventory`.
  - **Quan hệ Hợp thành & Kết tập (Tiêu chí 6):** `World ◆ Chunk` và `Inventory ◆ ItemStack` (Hợp thành - quản lý vòng đời trực tiếp); `ItemStack ◇ Item` (Kết tập - ngăn xếp chỉ giữ tham chiếu tới định nghĩa vật phẩm).
  - **Strategy Pattern (Tiêu chí 9, 10, 11):** Thuật toán sinh địa hình phân tách thành `FlatTerrainGenerator` và `PerlinTerrainGenerator` cùng thực thi `ITerrainGenerator`, dễ dàng thay đổi chiến lược sinh thế giới lúc runtime.
  - **Observer Pattern (Tiêu chí 9, 10, 11):** Sử dụng C# event `World.BlockChanged` và `Inventory.InventoryChanged` giúp tầng Domain phát tín hiệu mà không hề biết tới UI hay GameObject tầng Presentation.
  - **Single Responsibility & Face Culling (Tiêu chí 15):** `Chunk` chỉ giữ dữ liệu khối, `ChunkMeshBuilder` đảm nhận dựng hình và khử mặt khuất tiếp xúc giữa các khối kề nhau.
  - **Atomic Transaction (Quy tắc R10):** Phương thức `Inventory.RemoveItem` kiểm tra đủ tổng số lượng trước khi tiến hành trừ, đảm bảo nguyên tắc chế tạo không bị mất vật phẩm nửa chừng nếu thiếu tài nguyên.
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
