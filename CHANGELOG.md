# Nhật ký Phiên bản (Changelog)

Tất cả các thay đổi của dự án được AI và các thành viên ghi nhận tự động tại đây theo quy tắc trong [`AGENTS.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/AGENTS.md).
Định dạng đánh số phiên bản: **Semantic Versioning (`vMAJOR.MINOR.PATCH`)**.

---

## [v1.0.2] - 2026-11-09 23:18 (+07:00) — Ghép lưới Texture Atlas 16×16 từ gói OptimumRealism_64x, nạp đúng gói mặc định từ config.json và tinh chỉnh sương mù
- **Loại cập nhật:** Patch
- **Phân hệ:** World (A) & Presentation (Shared)
- **Thêm mới (Added):**
  - Không
- **Chỉnh sửa (Changed):**
  - `Game/Assets/_Project/Scripts/Application/World/ResourcePackService.cs` — Hỗ trợ tham số tùy chọn `initialPackId = "default"` trên constructor và fallback về `"default"` nếu gói chỉ định không tồn tại.
  - `Game/Assets/_Project/Scripts/Presentation/Bootstrap/GameBootstrap.cs` — Truyền `config.ResourcePack.ActivePack` (`OptimumRealism_64x`) vào `ResourcePackService` khi khởi động thay vì mặc định `default`.
  - `Game/Assets/_Project/Scripts/Presentation/World/ResourcePackView.cs` — Nâng cấp bộ ghép Texture Atlas 16×16 ô khớp với chỉ số UV của `BlockRegistry` (từ `tileIndex` 1..13: `bedrock`, `stone`, `dirt`, `grass_block_top` nhuộm xanh lá, `grass_block_side`, `oak_log`, `oak_log_top`, `oak_leaves`, `sand`, `oak_planks`, `crafting_table_top`, `crafting_table_side`, `cobblestone`), kèm bộ sinh pixel-art màu sắc tự động cho gói `default` và gán cả `_BaseMap` (URP) lẫn `mainTexture` (Standard).
  - `Game/Assets/_Project/Scripts/Presentation/Environment/AtmosphereFogView.cs` — Giảm mật độ sương mù mặc định (`0.0025f` ban ngày / `0.0065f` ban đêm) để nhìn rõ bề mặt texture của các khối ở xa.
- **Xóa bỏ (Removed):**
  - Không
- **Giải thích OOP / AI (dùng khi vấn đáp):**
  - Hoàn thiện cơ chế phối hợp giữa `ChunkMeshBuilder` (Domain tính UV theo lưới 16×16) và `ResourcePackView` (Presentation ghép các tệp `.png` từ gói `OptimumRealism_64x` vào đúng ô `tileIndex` trên `Texture2D` Atlas).

---

## [v1.0.1] - 2026-11-09 23:10 (+07:00) — Sửa lỗi CS1061 thiếu hàm `MobCombatService.ProcessHostileAttacks` khi biên dịch trong Unity Editor
- **Loại cập nhật:** Patch
- **Phân hệ:** Survival (B) & Presentation (Shared)
- **Thêm mới (Added):**
  - `Game/Assets/_Project/Scripts/Application/Survival/MobCombatService.cs` — Bổ sung phương thức `ProcessHostileAttacks(IReadOnlyList<Mob> activeMobs, Player player)` để khớp với lời gọi tại `GameBootstrap.cs(199,32)` và sơ đồ tuần tự số 4.
  - `Game/Assets/_Project/Tests/EditMode/MobCombatAndGameModeServiceTests.cs` — Bổ sung test case thứ 74 `MobCombat_ProcessHostileAttacks_DamagesPlayerOnlyFromHostileMobsInRange`.
- **Chỉnh sửa (Changed):**
  - Không
- **Xóa bỏ (Removed):**
  - Không
- **Giải thích OOP / AI (dùng khi vấn đáp):**
  - Đảm bảo tính nhất quán hợp đồng giữa tầng `Presentation` (`GameBootstrap`) và dịch vụ điều phối Use Case ở tầng `Application` (`MobCombatService`), biên dịch sạch 100% (`0 Error`) cả 4 assembly trong Unity 6000.6.8f1.

---

## [v1.0.0] - 2026-11-09 23:03 (+07:00) — Phát hành Chính thức v1.0.0: Hoàn thiện 100% Dự án, Công cụ Tự động Dựng Scene 1-Click, Bộ Sơ đồ UML (Use Case, Class, 4 Sequence Diagrams) & Cẩm nang Bảo vệ 20/20 Tiêu chí Barem [S2]
- **Loại cập nhật:** Major
- **Phân hệ:** World (A), Survival (B) & Shared/Setup
- **Thêm mới (Added):**
  - `Game/Assets/_Project/Scripts/Presentation/Bootstrap/EditorSceneSetup.cs` — Công cụ Editor (`#if UNITY_EDITOR`) cung cấp 2 menu: `Minecraft OOP -> 1. Auto-Setup Playable Game Scene` (tự động tạo `Directional Light`, `WorldRoot`, `Player` tại `(0, 42, 0)`, `HUD`, `GameBootstrap` và nối sẵn toàn bộ các `SerializedProperty` chỉ trong 1 cú nhấp chuột) và `Minecraft OOP -> 2. Generate All 4 Scenes (MainMenu, Game, Sandbox_A, Sandbox_B)`.
  - `docs/uml/use-case-diagram.md` — Sơ đồ Use Case tổng thể bằng Mermaid (`flowchart LR`) thể hiện 3 tác nhân (`Người chơi Survival`, `Người chơi Creative`, `Hệ thống Tự động`), bảng đặc tả chi tiết 12 Use Case (6 UC Phân hệ A + 6 UC Phân hệ B) ánh xạ chính xác tới từng phương thức điều khiển, và giải thích cơ chế phân quyền tác nhân qua `IGameMode` (Đáp ứng Tiêu chí 1 & 4).
  - `docs/uml/class-diagram.md` — Sơ đồ lớp UML tổng thể bằng Mermaid (`classDiagram`) chứa 35 lớp/interface/struct thực tế, bảng giải thích ý nghĩa thuộc tính của 22 lớp thực thể/giá trị trong tầng Domain, và bảng phân tích đủ 5 mối quan hệ OOP (Kế thừa, Thực thi, Hợp thành, Kết tập, Phụ thuộc) xuyên suốt 2 phân hệ A & B (Đáp ứng Tiêu chí 2, 5, 6).
  - `docs/uml/sequence-diagrams.md` — Bộ 4 Sơ đồ Tuần tự (Sequence Diagrams bằng Mermaid `sequenceDiagram`) khớp 100% thứ tự gọi hàm runtime: (1) Dò tia DDA & Phá khối rơi vật phẩm, (2) Di chuyển sinh Chunk & Dựng lưới Face Culling, (3) Chế tạo vật phẩm Atomic theo R10, (4) Sinh quái ban đêm R11, chiến đấu, chết rơi đồ & hồi sinh R9 (Đáp ứng Tiêu chí 7).
  - `docs/defense-guide.md` — Cẩm nang bảo vệ và vấn đáp toàn diện: Bảng ánh xạ 20/20 tiêu chí Barem [S2], bảng 12 quy tắc nghiệp vụ (R1–R12), phân tích 5 Design Pattern (Strategy, Observer, State, Factory, Repository), hướng dẫn thực hành trực tiếp 3 bài mở rộng code ngay tại bàn vấn đáp trong < 5 phút/bài (Tiêu chí 12), kịch bản Demo 7 bước trên máy và 10 câu hỏi vấn đáp xoáy kèm đáp án mẫu cho Thành viên A & B (Đáp ứng Tiêu chí 3, 8–20).
  - `docs/superpowers/plans/2026-12-07-t5-final-uml-report-and-release-v1.md` — Kế hoạch triển khai chi tiết Tuần T5 & phát hành `v1.0.0`.
- **Chỉnh sửa (Changed):**
  - `Game/Assets/_Project/Scripts/Presentation/Bootstrap/GameBootstrap.cs` — Bổ sung tích hợp `ChunkLoadService` tự động sinh và nạp lưới Chunk theo vị trí di chuyển của người chơi (`UpdateStreamingChunks`), liên kết đầy đủ `FirstPersonPlayerController`, `PlayerInteractionController`, `SandboxHudView` và hỗ trợ phím `F1` xoay vòng cả 3 chế độ `Survival -> Creative -> Adventure`.
  - `README.md` — Cập nhật thống kê hoàn chỉnh của phiên bản `v1.0.0` (73 Unit Tests PASS 100%, 88 files C#, 4 tầng `.asmdef`), bảng phím tắt điều khiển, hướng dẫn sử dụng menu 1-click `EditorSceneSetup` và liên kết tới toàn bộ hồ sơ UML & cẩm nang vấn đáp.
- **Xóa bỏ (Removed):**
  - Không
- **Giải thích OOP / AI (dùng khi vấn đáp):**
  - **Đồng bộ hóa 100% Thiết kế UML ↔ Mã nguồn (Tiêu chí 1–8):** Toàn bộ sơ đồ Use Case, Class Diagram (35 lớp) và 4 Sequence Diagrams được trích xuất trực tiếp từ các lớp và phương thức thực tế trong 4 assembly `Minecraft.Domain`, `Minecraft.Application`, `Minecraft.Infrastructure`, `Minecraft.Presentation`.
  - **Pure Dependency Injection & Tự động hóa Scene (Tiêu chí 4, 16, 17):** `GameBootstrap` đóng vai trò Composition Root duy nhất khởi tạo và tiêm phụ thuộc qua constructor mà không dùng bất kỳ Singleton nào; `EditorSceneSetup` sử dụng `SerializedObject` để tự động hóa việc kết nối các View trên Unity Inspector, giúp dự án chạy ngay lập tức trên mọi máy tính.

---

## [v0.6.0] - 2026-11-09 22:48 (+07:00) — Hoàn thành Tuần T4: Hoàn thiện tầng "Nên có" & Đông cứng tính năng (Đổi Resource Pack Runtime, Rơi đồ khi chết & Tự động thu nhặt, Sương mù & Ánh sáng môi trường)
- **Loại cập nhật:** Minor
- **Phân hệ:** World (A), Survival (B) & Presentation (Shared)
- **Thêm mới (Added):**
  - **Phân hệ World & Infrastructure A (Thành viên A):**
    - `Game/Assets/_Project/Scripts/Domain/ResourcePacks/ResourcePackManifest.cs` — Thực thể bất biến lưu thông tin gói tài nguyên (`PackId`, `DisplayName`, `Description`, `PackFormat`, `TileResolution` 16x/64x, `BlockTexturePaths`), kiểm tra hợp lệ đầu vào nghiêm ngặt.
    - `Game/Assets/_Project/Scripts/Domain/Contracts/ITextureAtlasProvider.cs` — Hợp đồng Strategy Pattern cho việc truy vấn và nạp gói tài nguyên (`ProviderName`, `GetAvailablePacks`, `LoadPack`).
    - `Game/Assets/_Project/Scripts/Infrastructure/ResourcePacks/BuiltInAtlasProvider.cs` — Chiến lược 1: gói mặc định `default` 16×16 tích hợp sẵn với đầy đủ ánh xạ texture chuẩn cho toàn bộ `BlockId`.
    - `Game/Assets/_Project/Scripts/Infrastructure/ResourcePacks/FolderResourcePackProvider.cs` — Chiến lược 2: quét thư mục `StreamingAssets/ResourcePacks`, đọc `pack.mcmeta` bằng `System.Text.Json` bọc `try-catch` an toàn, kiểm tra các tệp `.png` trong `assets/minecraft/textures/block/` và tự động fallback sang `BuiltInAtlasProvider` khi thiếu thư mục hoặc lỗi tệp (Tiêu chí 19).
    - `Game/Assets/_Project/Scripts/Application/World/ResourcePackService.cs` — Dịch vụ điều phối Use Case UC06b (Đổi Resource Pack lúc chạy), cho phép hoán đổi `ITextureAtlasProvider` runtime và phát sự kiện Observer `ResourcePackChanged`.
    - `Game/Assets/_Project/Tests/EditMode/ResourcePackTests.cs` — 5 Unit Test NUnit kiểm thử `ResourcePackManifest`, `BuiltInAtlasProvider`, `FolderResourcePackProvider` (đọc thư mục thật/tạm, xử lý `pack.mcmeta` hỏng JSON, thư mục không tồn tại) và `ResourcePackService.SwitchPack`.
  - **Phân hệ Survival & Presentation B (Thành viên B):**
    - `Game/Assets/_Project/Scripts/Domain/Entities/DroppedItemEntity.cs` — Thực thể vật phẩm rơi trên mặt đất kế thừa `Entity` (`DroppedItemEntity : Entity`), đóng gói `ItemStack Payload`, bộ đếm `DespawnTimeSeconds` (mặc định 300s) và ghi đè `Tick(float deltaTime)` để tự hủy khi hết hạn.
    - `Game/Assets/_Project/Scripts/Application/Survival/DeathDropService.cs` — Dịch vụ quản lý `ActiveDrops`, thực thi `DropInventoryOnDeath(Player)` (làm rơi toàn bộ 36 ô túi đồ và xóa sạch túi đồ khi chết ở Survival rồi hồi sinh về `SpawnPoint`, nhưng giữ nguyên túi đồ ở Creative) và `CollectNearbyDrops(Player, pickupRadius = 2.0f)` tự động thu nhặt vật phẩm khi lại gần.
    - `Game/Assets/_Project/Scripts/Presentation/World/ResourcePackView.cs` — MonoBehaviour lắng nghe `ResourcePackService.ResourcePackChanged`, đọc byte ảnh bằng `File.ReadAllBytes` + `ImageConversion.LoadImage` với `FilterMode.Point` và áp dụng lên Material của `WorldView`.
    - `Game/Assets/_Project/Scripts/Presentation/Environment/AtmosphereFogView.cs` — MonoBehaviour lắng nghe `DayNightCycle.PhaseChanged` và `TimeChanged` để nội suy mượt mà màu sương mù (`RenderSettings.fogColor`, `RenderSettings.fogDensity`) và ánh sáng môi trường (`RenderSettings.ambientLight`) giữa Ban ngày, Hoàng hôn, Ban đêm và Bình minh.
    - `Game/Assets/_Project/Tests/EditMode/DeathDropAndAtmosphereTests.cs` — 5 Unit Test NUnit kiểm thử kế thừa và hết hạn của `DroppedItemEntity`, rơi đồ khi chết ở Survival vs giữ đồ ở Creative, và tự động nhặt lại đồ trong bán kính `2.0f`.
    - `docs/superpowers/plans/2026-11-30-t4-resource-packs-drops-polish.md` — Kế hoạch triển khai chi tiết Tuần T4.
- **Chỉnh sửa (Changed):**
  - `Game/Assets/_Project/Scripts/Domain/Environment/DayNightCycle.cs` — Bổ sung bí danh sự kiện `TimeChanged` bên cạnh `TimeUpdated` phục vụ đồng bộ hiệu ứng môi trường.
  - `Game/Assets/_Project/Scripts/Presentation/World/WorldView.cs` — Bổ sung thuộc tính `TerrainMaterial` và phương thức `SetTerrainMaterial` để cập nhật chất liệu đồng loạt cho các `ChunkView` khi đổi gói tài nguyên.
  - `Game/Assets/_Project/Scripts/Presentation/Bootstrap/GameBootstrap.cs` — Khởi tạo `FolderResourcePackProvider` (kèm fallback `BuiltInAtlasProvider`), `ResourcePackService`, `DeathDropService`, tự động rơi đồ khi `Player.Health.Died` và nối phím tắt `F2` để đổi nhanh giữa gói `default` (16x) và `OptimumRealism_64x` (64x) ngay trong game.
- **Xóa bỏ (Removed):**
  - Không
- **Giải thích OOP / AI (dùng khi vấn đáp):**
  - **Strategy Pattern & Đảo ngược phụ thuộc (Tiêu chí 9, 10, 11, 16):** `ResourcePackService` làm việc hoàn toàn qua giao diện `ITextureAtlasProvider`, cho phép chuyển đổi giữa gói tích hợp sẵn (`BuiltInAtlasProvider`) và gói đọc từ đĩa (`FolderResourcePackProvider`) lúc runtime mà không sửa đổi tầng nghiệp vụ.
  - **Observer Pattern (Tiêu chí 9, 10, 11):** Khi gọi `ResourcePackService.SwitchPack`, sự kiện `ResourcePackChanged` tự động kích hoạt `ResourcePackView` nạp lại Texture2D; tương tự, `DayNightCycle` phát sự kiện `PhaseChanged` và `TimeChanged` để `AtmosphereFogView` nội suy sương mù mà Domain vẫn thuần C# 100%.
  - **Kế thừa, Đa hình & Phân quyền theo GameMode (Tiêu chí 4, 6, 14):** Mở rộng cây kế thừa `Entity` với `DroppedItemEntity : Entity` bên cạnh `Player` và `Mob`; `DeathDropService` dựa vào `IGameMode.ConsumesItemsOnPlace` để phân biệt hành vi rơi toàn bộ túi đồ ở Survival với việc bảo toàn vật phẩm ở Creative.
  - **Xử lý ngoại lệ an toàn (Tiêu chí 19):** `FolderResourcePackProvider` bọc `try-catch` nhiều lớp khi đọc `pack.mcmeta` và quét thư mục, tự động phục hồi về mô tả mặc định hoặc ủy quyền sang `BuiltInAtlasProvider` khi gặp JSON hỏng hoặc thiếu thư mục.

---

## [v0.5.0] - 2026-11-09 22:36 (+07:00) — Hoàn thành Tuần T3: Mốc 2 – MVP Xong (Lưu/Tải JSON, Đọc cấu hình try-catch, Phân cấp Mob đa hình & AI, Vòng lặp Ngày/Đêm, Spawner R11, GameModes & GameBootstrap Pure DI)
- **Loại cập nhật:** Minor
- **Phân hệ:** World (A), Survival (B) & Presentation (Shared)
- **Thêm mới (Added):**
  - **Phân hệ World & Infrastructure A (Thành viên A):**
    - `Game/Assets/_Project/Scripts/Domain/GameModes/SurvivalMode.cs` — Thực thi `IGameMode` cho chế độ Sinh tồn, chặn phá Bedrock theo Quy tắc R1.
    - `Game/Assets/_Project/Scripts/Domain/GameModes/CreativeMode.cs` — Thực thi `IGameMode` cho chế độ Sáng tạo, miễn nhiễm sát thương, phá được Bedrock, không tốn khối theo Quy tắc R12.
    - `Game/Assets/_Project/Scripts/Domain/GameModes/AdventureMode.cs` — Thực thi `IGameMode` cho chế độ Phiêu lưu (Bài mở rộng luyện thi vấn đáp dòng 12), cấm phá mọi khối địa hình.
    - `Game/Assets/_Project/Scripts/Domain/Configuration/GameConfig.cs` — Các lớp DTO cấu hình thế giới (`WorldConfig`), người chơi (`PlayerConfig`), gói tài nguyên (`ResourcePackConfig`) và `GameConfigDto.CreateDefault()` dự phòng.
    - `Game/Assets/_Project/Scripts/Domain/Contracts/IConfigLoader.cs` — Hợp đồng dịch vụ nạp cấu hình trò chơi từ đĩa (DIP).
    - `Game/Assets/_Project/Scripts/Infrastructure/Config/ConfigLoader.cs` — Cài đặt đọc cấu hình JSON từ file `StreamingAssets/config.json`, bọc `try-catch` an toàn xử lý thiếu file hoặc hỏng JSON, tự động fallback về cấu hình mặc định (Tiêu chí 19).
    - `Game/Assets/_Project/Scripts/Domain/Persistence/BlockChangeDto.cs` — DTO lưu tọa độ và mã khối bị thay đổi (`X, Y, Z, BlockId`).
    - `Game/Assets/_Project/Scripts/Domain/Persistence/InventorySlotDto.cs` — DTO lưu dữ liệu một ô túi đồ (`SlotIndex, ItemId, Count`).
    - `Game/Assets/_Project/Scripts/Domain/Persistence/PlayerSaveDto.cs` — DTO lưu vị trí, máu, đói, chế độ chơi và túi đồ người chơi.
    - `Game/Assets/_Project/Scripts/Domain/Persistence/WorldSaveDto.cs` — DTO tổng thể chứa toàn bộ dữ liệu file lưu thế giới.
    - `Game/Assets/_Project/Scripts/Domain/Contracts/IWorldRepository.cs` — Hợp đồng Repository lưu/tải thế giới (Tiêu chí 16).
    - `Game/Assets/_Project/Scripts/Infrastructure/Persistence/JsonWorldRepository.cs` — Thực thi `IWorldRepository` sử dụng `System.Text.Json`, bọc `try-catch` toàn diện các lỗi I/O ghi đọc đĩa và JSON (Tiêu chí 19).
    - `Game/Assets/_Project/Scripts/Application/World/WorldPersistenceService.cs` — Dịch vụ điều phối Use Case UC05 (Lưu thế giới) và UC06 (Tải thế giới), chuyển đổi 2 chiều giữa Entity/Aggregate Domain và DTO.
    - `Game/Assets/_Project/Tests/EditMode/GameModeTests.cs` — 3 Unit Test NUnit kiểm thử quyền hạn và quy tắc của SurvivalMode, CreativeMode, AdventureMode.
    - `Game/Assets/_Project/Tests/EditMode/ConfigLoaderTests.cs` — 3 Unit Test NUnit kiểm thử đọc file JSON hợp lệ, fallback khi thiếu file và fallback khi file hỏng cú pháp.
    - `Game/Assets/_Project/Tests/EditMode/PersistenceTests.cs` — 3 Unit Test NUnit kiểm thử tính toàn vẹn DTO, khả năng chịu lỗi file hỏng và khôi phục trọn vẹn thế giới/người chơi.
  - **Phân hệ Survival, Mobs, Environment & Presentation B (Thành viên B):**
    - `Game/Assets/_Project/Scripts/Domain/Environment/DayNightPhase.cs` — Enum 4 giai đoạn chu kỳ ngày/đêm (`Day`, `Sunset`, `Night`, `Sunrise`).
    - `Game/Assets/_Project/Scripts/Domain/Environment/DayNightCycle.cs` — Bộ đếm thời gian chu kỳ tuần hoàn thuần C#, cung cấp Observer event `PhaseChanged` và `TimeUpdated`, cờ `IsNight` phục vụ Quy tắc R11.
    - `Game/Assets/_Project/Scripts/Domain/Entities/Mobs/MobType.cs` — Enum phân loại giống loài sinh vật (`Zombie`, `Cow`, `Skeleton`).
    - `Game/Assets/_Project/Scripts/Domain/Entities/Mobs/MobState.cs` — Enum các trạng thái hành vi AI (`Idle`, `Wander`, `Chase`).
    - `Game/Assets/_Project/Scripts/Domain/Entities/Mobs/Mob.cs` — Lớp trừu tượng (`abstract class`) kế thừa `Entity`, chứa chỉ số sinh vật và phương thức ảo thuần `UpdateAI`.
    - `Game/Assets/_Project/Scripts/Domain/Entities/Mobs/Zombie.cs` — Mob thù địch cận chiến kế thừa `Mob`, AI tự động áp sát và chuyển trạng thái `Chase` khi người chơi trong bán kính 16 block.
    - `Game/Assets/_Project/Scripts/Domain/Entities/Mobs/Cow.cs` — Mob hiền lành kế thừa `Mob`, AI chỉ đi dạo tự do (`Wander`), không bao giờ tấn công người chơi.
    - `Game/Assets/_Project/Scripts/Domain/Entities/Mobs/Skeleton.cs` — Mob thù địch tầm xa (Bài mở rộng luyện thi vấn đáp dòng 12), AI giữ khoảng cách an toàn 6 block để tấn công người chơi.
    - `Game/Assets/_Project/Scripts/Domain/Contracts/IMobFactory.cs` — Hợp đồng Factory tạo sinh vật theo mẫu thiết kế Factory Pattern.
    - `Game/Assets/_Project/Scripts/Domain/Entities/Mobs/MobFactory.cs` — Triển khai cụ thể `IMobFactory` tạo đối tượng từ `MobType`.
    - `Game/Assets/_Project/Scripts/Application/Survival/MobSpawnerService.cs` — Dịch vụ điều phối UC12, thực thi triệt để Quy tắc R11 (chỉ spawn Zombie ban đêm, cách người chơi >= 24 block, giới hạn số lượng tối đa).
    - `Game/Assets/_Project/Scripts/Application/Survival/MobCombatService.cs` — Dịch vụ điều phối UC08 (Chiến đấu với Mob), kiểm tra cự ly va chạm, xét quyền miễn nhiễm ở Creative (Quy tắc R12) và kích hoạt tử vong khi máu về 0 (Quy tắc R9).
    - `Game/Assets/_Project/Scripts/Application/Survival/GameModeService.cs` — Dịch vụ điều phối UC11 (Đổi GameMode), chuyển đổi trạng thái người chơi an toàn giữa Survival, Creative, Adventure.
    - `Game/Assets/_Project/Scripts/Presentation/Environment/DayNightView.cs` — MonoBehaviour kết nối Observer với `DayNightCycle`, điều khiển góc quay Directional Light và cường độ/màu sắc ánh sáng.
    - `Game/Assets/_Project/Scripts/Presentation/Mobs/MobView.cs` — MonoBehaviour liên kết với thực thể `Mob`, đồng bộ vị trí 3D mượt mà và tự tiêu hủy GameObject khi sinh vật chết.
    - `Game/Assets/_Project/Scripts/Presentation/Bootstrap/GameBootstrap.cs` — Điểm khởi động Composition Root, kết nối Dependency Injection toàn bộ 4 tầng kiến trúc bằng tay (Pure DI), không dùng Singleton; tích hợp phím tắt F1 (Đổi GameMode), F5 (Lưu nhanh), F9 (Tải nhanh).
    - `Game/Assets/_Project/Tests/EditMode/DayNightCycleTests.cs` — 3 Unit Test NUnit kiểm thử chu kỳ ngày/đêm và các sự kiện Observer.
    - `Game/Assets/_Project/Tests/EditMode/MobHierarchyTests.cs` — 5 Unit Test NUnit kiểm thử phân cấp Mob, đa hình AI truy đuổi của Zombie, Cow và Skeleton.
    - `Game/Assets/_Project/Tests/EditMode/MobSpawnerTests.cs` — 5 Unit Test NUnit kiểm thử MobFactory và các điều kiện sinh quái theo Quy tắc R11.
    - `Game/Assets/_Project/Tests/EditMode/MobCombatAndGameModeServiceTests.cs` — 5 Unit Test NUnit kiểm thử chiến đấu, miễn nhiễm sát thương ở Creative (R12) và chuyển đổi chế độ chơi (UC11).
    - `docs/superpowers/plans/2026-11-23-t3-mvp-save-load-mobs.md` — Kế hoạch triển khai chi tiết Tuần T3.
- **Chỉnh sửa (Changed):**
  - `Game/Assets/_Project/Scripts/Domain/Entities/Health.cs` — Thêm bí danh `CurrentHealth`, hàm `TakeDamage(amount)` và `SetCurrentHealth(value)` phục vụ khôi phục trạng thái khi nạp save.
  - `Game/Assets/_Project/Scripts/Domain/Entities/Hunger.cs` — Thêm bí danh `CurrentHunger`, `ConsumeHunger(amount)` và `SetCurrentHunger(value)`.
  - `Game/Assets/_Project/Scripts/Domain/Items/Inventory.cs` — Thêm phương thức `Clear()` để làm trống túi đồ trước khi nạp lại dữ liệu từ save file.
  - `Game/Assets/_Project/Scripts/Domain/Items/ItemStack.cs` — Thêm thuộc tính bí danh `Item` trỏ tới `Definition`.
  - `Game/Assets/_Project/Scripts/Domain/Items/ItemRegistry.cs` — Thêm phương thức `GetItem(id)` trỏ tới `Get(id)`.
- **Xóa bỏ (Removed):**
  - Không
- **Giải thích OOP / AI (dùng khi vấn đáp):**
  - **State Pattern & Phân quyền tác nhân (Tiêu chí 4, 9, 10, 11):** Thể hiện sự khác biệt rành mạch giữa các tác nhân (Người chơi Survival, Creative, Adventure) thông qua hợp đồng `IGameMode` và các lớp cụ thể `SurvivalMode`, `CreativeMode`, `AdventureMode`, loại bỏ hoàn toàn câu lệnh `if/else` phân tán trên View.
  - **Factory Pattern (Tiêu chí 9, 10, 11):** `MobFactory` đóng gói khởi tạo các lớp con `Zombie`, `Cow`, `Skeleton` từ `MobType`, giúp `MobSpawnerService` không phụ thuộc vào các constructor cụ thể.
  - **Kế thừa & Đa hình (Tiêu chí 6, 14, 15):** Cây phân cấp `Entity -> Mob -> (Zombie, Cow, Skeleton)` cho phép quản lý danh sách sinh vật thống nhất qua kiểu cha `Mob` (LSP), trong khi mỗi loài tự quyết định hành vi AI và tầm đánh riêng biệt qua `UpdateAI` (Polymorphism).
  - **Repository Pattern & DIP (Tiêu chí 16):** `WorldPersistenceService` phụ thuộc vào `IWorldRepository`, cho phép chuyển đổi định dạng lưu từ JSON sang nhị phân mà không sửa đổi một dòng logic nghiệp vụ nào.
  - **Xử lý ngoại lệ an toàn & Khả năng chịu lỗi (Tiêu chí 19):** Cả `ConfigLoader` và `JsonWorldRepository` đều được bọc `try-catch` nhiều tầng, xử lý biệt hóa `JsonException` và `IOException`, tự động fallback cấu hình mặc định giúp game không bao giờ sập khi gặp file hỏng.
  - **Pure Dependency Injection (Tiêu chí 4, 8, 16):** Tuyệt đối không dùng Singleton cho logic nghiệp vụ; toàn bộ các phân hệ được `GameBootstrap` khởi tạo và tiêm thủ công qua constructor.

---

## [v0.4.0] - 2026-11-09 22:10 (+07:00) — Hoàn thành Tuần T2: Mốc 1 – Sandbox chơi được (Dò tia Voxel, Tương tác Thế giới, Sinh tồn, Chế tạo & HUD)
- **Loại cập nhật:** Minor
- **Phân hệ:** World (A) & Survival (B)
- **Thêm mới (Added):**
  - **Phân hệ World & Application A (Thành viên A):**
    - `Game/Assets/_Project/Scripts/Domain/World/VoxelRaycastHit.cs` — Struct bất biến (`readonly struct`) lưu trữ kết quả va chạm dò tia Voxel (`IsHit`, `BlockPosition`, `AdjacentPosition`, `HitFace`, `Distance`).
    - `Game/Assets/_Project/Scripts/Domain/World/VoxelRaycaster.cs` — Thuật toán 3D DDA (Amanatides & Woo) thuần C# với `System.Numerics.Vector3`, giới hạn tầm với 5.0 block theo Quy tắc R7.
    - `Game/Assets/_Project/Scripts/Application/World/BlockInteractionService.cs` — Dịch vụ điều phối UC03 (Phá block) và UC04 (Đặt block), thực thi Quy tắc R1 & R12 (Bedrock ở Survival vs Creative), R2 (chặn đặt đè lên chân/đầu người chơi), R3 (biên độ cao) và R5 (rơi vật phẩm vào túi đồ).
    - `Game/Assets/_Project/Scripts/Application/World/ChunkLoadResult.cs` — Gói dữ liệu kết quả sau chu kỳ nạp/gỡ Chunk.
    - `Game/Assets/_Project/Scripts/Application/World/ChunkLoadService.cs` — Dịch vụ điều phối UC02 (Sinh và khám phá Chunk theo vị trí người chơi), tự động giải phóng các Chunk ngoài bán kính tầm nhìn.
    - `Game/Assets/_Project/Tests/EditMode/VoxelInteractionTests.cs` — Bộ 6 Unit Test NUnit kiểm thử thuật toán DDA raycast và các dịch vụ tương tác khối, nạp Chunk.
  - **Phân hệ Survival, Crafting & Presentation B (Thành viên B):**
    - `Game/Assets/_Project/Scripts/Domain/Entities/Health.cs` — Đóng gói máu (0..Max), miễn nhiễm sát thương ở Creative (Quy tắc R12), phát sự kiện `HealthChanged` và `Died` (Quy tắc R9).
    - `Game/Assets/_Project/Scripts/Domain/Entities/Hunger.cs` — Đóng gói thanh đói, chu kỳ 4.0s: đói = 0 trừ máu, đói >= 18 hồi máu (Quy tắc R8).
    - `Game/Assets/_Project/Scripts/Domain/Entities/Entity.cs` — Lớp cơ sở trừu tượng (`abstract class`) cho mọi thực thể có vị trí và máu trong không gian 3 chiều.
    - `Game/Assets/_Project/Scripts/Domain/Entities/Player.cs` — Thực thể người chơi kế thừa `Entity`, Hợp thành `Health`/`Hunger`/`Inventory`, State Pattern theo `IGameMode` và phương thức `Respawn()` hồi sinh về SpawnPoint (Quy tắc R9).
    - `Game/Assets/_Project/Scripts/Domain/Crafting/RecipeIngredient.cs` — Struct bất biến đại diện cho thành phần nguyên liệu và số lượng yêu cầu.
    - `Game/Assets/_Project/Scripts/Domain/Crafting/Recipe.cs` — Thực thể công thức chế tạo gồm nguyên liệu đầu vào và sản phẩm đầu ra.
    - `Game/Assets/_Project/Scripts/Domain/Crafting/RecipeBook.cs` — Quan hệ Kết tập `RecipeBook ◇ Recipe`, đăng ký 5 công thức cơ bản (Ván gỗ, Bàn chế tạo, Cúp gỗ, Kiếm gỗ, Cúp đá).
    - `Game/Assets/_Project/Scripts/Application/Survival/CraftingService.cs` — Dịch vụ điều phối UC09 (Chế tạo vật phẩm) theo Quy tắc R10 ("đủ hết hoặc không làm gì" - Atomic Transaction).
    - `Game/Assets/_Project/Scripts/Application/Survival/SurvivalService.cs` — Dịch vụ điều phối UC10 (Ăn thức ăn) và cập nhật nhịp sinh tồn thời gian thực.
    - `Game/Assets/_Project/Scripts/Presentation/World/WorldView.cs` — MonoBehaviour lắng nghe Observer `World.BlockChanged`, quản lý các `ChunkView` và cập nhật lại lưới khi đặt/phá block.
    - `Game/Assets/_Project/Scripts/Presentation/Player/PlayerInteractionController.cs` — Controller bắt phím chuột, dùng `VoxelRaycaster` để phá block (chuột trái), ăn thức ăn / đặt block (chuột phải).
    - `Game/Assets/_Project/Scripts/Presentation/UI/SandboxHudView.cs` — HUD OnGUI kết nối Observer hiển thị tâm ngắm, thanh Máu, thanh Đói, 9 ô Hotbar và bảng chế tạo nhanh.
    - `Game/Assets/_Project/Tests/EditMode/SurvivalAndCraftingTests.cs` — Bộ 5 Unit Test NUnit kiểm thử độc lập hệ thống Máu, Đói, Player, Chế tạo và Sinh tồn.
    - `docs/superpowers/plans/2026-11-09-t2-raycast-interaction-survival-crafting.md` — Kế hoạch triển khai chi tiết Tuần T2.
- **Chỉnh sửa (Changed):**
  - Không
- **Xóa bỏ (Removed):**
  - Không
- **Giải thích OOP / AI (dùng khi vấn đáp):**
  - **Dependency Injection & Inversion (Tiêu chí 2, 16):** Toàn bộ các Application Service (`BlockInteractionService`, `ChunkLoadService`, `CraftingService`, `SurvivalService`) nhận các giao diện và thực thể Domain qua constructor, giúp các tầng độc lập hoàn toàn và dễ kiểm thử cô lập.
  - **Quan hệ Kế thừa & Hợp thành (Tiêu chí 6, 14):** `Player : Entity` (Kế thừa); `Player ◆ Inventory, Health, Hunger` (Hợp thành); `RecipeBook ◇ Recipe` (Kết tập).
  - **Thuật toán DDA thuần C# (Tiêu chí 4, 8):** `VoxelRaycaster` chạy độc lập với Unity Physics, bảo đảm tầng Domain tuân thủ `noEngineReferences: true`.
  - **Atomic Transaction (Quy tắc R10):** Phương thức `CraftingService.TryCraft` kiểm tra đầy đủ nguyên liệu trước khi tiến hành trừ, tránh mất mát vật phẩm nếu thiếu nguyên liệu.

---

## [v0.3.1] - 2026-11-09 21:52 (+07:00) — Khôi phục tiêu đề phiên bản v0.2.0 trong CHANGELOG và đồng bộ định dạng xuống dòng cuối file
- **Loại cập nhật:** Patch
- **Phân hệ:** Shared/Setup
- **Thêm mới (Added):**
  - Không
- **Chỉnh sửa (Changed):**
  - `CHANGELOG.md` — Khôi phục dòng phân cách và tiêu đề `## [v0.2.0]` bị ghi đè khi ghép nhật ký Tuần T1.
  - Chuẩn hóa ký tự xuống dòng cuối file (`insert_final_newline = true` theo `.editorconfig`) cho 19 file C# của Tuần T1.
- **Xóa bỏ (Removed):**
  - Không
- **Giải thích OOP / AI (dùng khi vấn đáp):**
  - Duy trì tính toàn vẹn của tài liệu truy vết phiên bản (`CHANGELOG.md`) và tuân thủ đồng nhất Coding Convention (`.editorconfig`) giữa các thành viên (Tiêu chí 15, 20).

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
