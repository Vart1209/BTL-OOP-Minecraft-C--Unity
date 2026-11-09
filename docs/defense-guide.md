# Cẩm nang Bảo vệ Đồ án, Ánh xạ 20/20 Tiêu chí Barem [S2] & Hướng dẫn Vấn đáp (`v1.0.0`)

> Tài liệu này tổng hợp toàn bộ minh chứng học thuật, ánh xạ trực tiếp tới từng tệp mã nguồn của dự án **Minecraft Voxel Survival & Creative (Unity 6 C#)** để 2 thành viên tra cứu tức thì tại bàn bảo vệ Bài Tập Lớn Lập trình Hướng Đối tượng (OOP).

---

## 1. Bảng Ánh xạ 20/20 Tiêu chí Barem [S2] (Sheet `"Nhóm 02 người"`)

| # | Nhóm tiêu chí | Yêu cầu của Barem [S2] | Kết quả Đạt được của Nhóm | File Code & Tài liệu Minh chứng Cụ thể |
| :---: | :--- | :--- | :--- | :--- |
| **1** | **Phân tích & Code** | $\ge 8$ Use Case khớp hàm điều khiển trong code, chia đều 2 người | **12 Use Case** (6 UC Thành viên A + 6 UC Thành viên B), ánh xạ 1-1 tới các phương thức trong tầng `Minecraft.Application`. | - Sơ đồ & Đặc tả: [`docs/uml/use-case-diagram.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/docs/uml/use-case-diagram.md)<br/>- Code A: [`BlockInteractionService.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Application/World/BlockInteractionService.cs), [`ChunkLoadService.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Application/World/ChunkLoadService.cs), [`WorldPersistenceService.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Application/World/WorldPersistenceService.cs), [`ResourcePackService.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Application/World/ResourcePackService.cs)<br/>- Code B: [`CraftingService.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Application/Survival/CraftingService.cs), [`SurvivalService.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Application/Survival/SurvivalService.cs), [`MobSpawnerService.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Application/Survival/MobSpawnerService.cs), [`MobCombatService.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Application/Survival/MobCombatService.cs), [`DeathDropService.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Application/Survival/DeathDropService.cs), [`GameModeService.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Application/Survival/GameModeService.cs) |
| **2** | **Phân tích & Code** | $\ge 15$ lớp thực thể dùng chung, giải thích được thuộc tính | **22 lớp thực thể / giá trị** trong `Minecraft.Domain`, đóng gói `private` toàn bộ và có bảng giải thích thuộc tính chi tiết. | - Bảng giải thích 22 lớp: [`docs/uml/class-diagram.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/docs/uml/class-diagram.md)<br/>- Code: `World`, `Chunk`, `BlockType`, `BlockRegistry`, `Entity`, `Player`, `Mob`, `Zombie`, `Cow`, `Skeleton`, `DroppedItemEntity`, `Health`, `Hunger`, `Item`, `BlockItem`, `ToolItem`, `FoodItem`, `ItemStack`, `Inventory`, `Recipe`, `RecipeBook`, `DayNightCycle` |
| **3** | **Phân tích & Code** | $\ge 8$ quy tắc nghiệp vụ cài trong phương thức ở cả 2 phân hệ | **12 quy tắc nghiệp vụ (R1–R12)** (5 quy tắc Phân hệ A + 7 quy tắc Phân hệ B), được kiểm chứng bằng 73 Unit Tests. | Xem Mục 2 bên dưới (Bảng 12 Quy tắc Nghiệp vụ R1–R12). |
| **4** | **Phân tích & Code** | Tách lớp giao diện / nghiệp vụ, phân quyền tác nhân | Tầng `Presentation` tách rời hoàn toàn khỏi `Domain`/`Application`; phân quyền 3 tác nhân (`SurvivalMode`, `CreativeMode`, `AdventureMode`) qua hợp đồng `IGameMode`. | [`IGameMode.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Contracts/IGameMode.cs), [`SurvivalMode.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/GameModes/SurvivalMode.cs), [`CreativeMode.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/GameModes/CreativeMode.cs), [`AdventureMode.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/GameModes/AdventureMode.cs) |
| **5** | **Thiết kế & Code** | UML Class Diagram tổng thể khớp code ($\ge 16$ lớp) | Sơ đồ lớp Mermaid chứa **35 lớp/interface/struct** khớp 100% tên lớp, thuộc tính và phương thức trong code. | [`docs/uml/class-diagram.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/docs/uml/class-diagram.md) |
| **6** | **Thiết kế & Code** | Đủ 5 quan hệ: kế thừa, thực thi, kết tập, hợp thành, phụ thuộc (cả giữa 2 phân hệ) | Đủ cả 5 mối quan hệ OOP, bao gồm quan hệ phụ thuộc/giao tiếp giữa Phân hệ A và Phân hệ B qua `IWorldReader`, `IWorldWriter`, `IGameMode`. | [`docs/uml/class-diagram.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/docs/uml/class-diagram.md) (Mục 3) |
| **7** | **Thiết kế & Code** | $\ge 4$ Sơ đồ tuần tự khớp thứ tự gọi hàm (mỗi người 2) | **4 Sequence Diagrams** chi tiết (2 sơ đồ Thành viên A + 2 sơ đồ Thành viên B) khớp 100% luồng gọi hàm runtime. | [`docs/uml/sequence-diagrams.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/docs/uml/sequence-diagrams.md) |
| **8** | **Thiết kế & Code** | Kiến trúc phân tầng khớp package và chiều gọi ($\ge 3$ tầng) | **4 tầng Assembly (`.asmdef`)**: `Domain` (`noEngineReferences: true`) $\leftarrow$ `Application` $\leftarrow$ `Infrastructure` $\leftarrow$ `Presentation`. | - [`Minecraft.Domain.asmdef`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Minecraft.Domain.asmdef)<br/>- [`Minecraft.Application.asmdef`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Application/Minecraft.Application.asmdef)<br/>- [`Minecraft.Infrastructure.asmdef`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Infrastructure/Minecraft.Infrastructure.asmdef)<br/>- [`Minecraft.Presentation.asmdef`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Presentation/Minecraft.Presentation.asmdef) |
| **9** | **Design Pattern** | Chọn và giải thích lý do ($\ge 3$ pattern, mỗi người 1–2) | Áp dụng **5 Design Pattern**: **Strategy** (A), **Observer** (A & B), **Repository** (A), **Factory** (B), **State** (B). Tuyệt đối không dùng Singleton. | Xem Mục 3 bên dưới (Phân tích 5 Design Pattern). |
| **10** | **Design Pattern** | Vai trò lớp trong pattern $\leftrightarrow$ code (cả 2 người) | Bảng ánh xạ đầy đủ Context, Strategy, Subject, Observer, Creator, ConcreteProduct, State cho cả A và B. | Xem Mục 3 bên dưới. |
| **11** | **Design Pattern** | Cơ chế phối hợp (đa hình, ủy quyền, khởi tạo) xuyên suốt | Khởi tạo và tiêm phụ thuộc thủ công (Pure DI) tại [`GameBootstrap.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Presentation/Bootstrap/GameBootstrap.cs), ủy quyền qua interface và phát sự kiện `event Action`. | [`GameBootstrap.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Presentation/Bootstrap/GameBootstrap.cs) |
| **12** | **Design Pattern** | Đánh giá pattern + mở rộng chức năng/lớp mới ngay trên code | Đã cài sẵn `Skeleton`, `AdventureMode` và chuẩn bị kịch bản code trực tiếp 3 bài mở rộng trong $< 5$ phút/bài. | Xem Mục 4 bên dưới (Hướng dẫn 3 Bài Mở rộng Code trực tiếp). |
| **13** | **Nguyên lý OOP** | Đóng gói, trừu tượng, `private`, kiểm tra dữ liệu hợp lệ | 100% trường dữ liệu là `private`/`private readonly`, kiểm tra `ArgumentNullException` / `ArgumentOutOfRangeException` ở mọi constructor và method. | [`Chunk.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/World/Chunk.cs), [`Health.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Entities/Health.cs), [`ItemStack.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Items/ItemStack.cs), [`ResourcePackManifest.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/ResourcePacks/ResourcePackManifest.cs) |
| **14** | **Nguyên lý OOP** | Kế thừa, đa hình, `override`, lớp con thay được lớp cha (LSP) | 2 cây kế thừa đa hình lớn: `Entity -> (Player, DroppedItemEntity, Mob -> Zombie/Cow/Skeleton)` và `Item -> (BlockItem, ToolItem, FoodItem)`. | [`Entity.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Entities/Entity.cs), [`Mob.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Entities/Mobs/Mob.cs), [`Item.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Items/Item.cs) |
| **15** | **Nguyên lý OOP** | Một lớp một nhiệm vụ (SRP), interface gọn (ISP), coding convention | Tách bạch `Chunk` (dữ liệu) / `ChunkMeshBuilder` (thuật toán lưới) / `ChunkView` (hiển thị); tách `IWorldReader` khỏi `IWorldWriter`; tuân thủ `.editorconfig`. | [`IWorldReader.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Contracts/IWorldReader.cs), [`IWorldWriter.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Contracts/IWorldWriter.cs), [`.editorconfig`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/.editorconfig) |
| **16** | **Nguyên lý OOP** | Ghép nối 2 người qua interface (DIP), dễ mở rộng / đổi cài đặt | Toàn bộ giao tiếp giữa A và B đi qua namespace `Minecraft.Domain.Contracts` (`IWorldReader`, `IWorldWriter`, `ITerrainGenerator`, `IGameMode`, `IWorldRepository`, `ITextureAtlasProvider`, `IMobFactory`). | Thư mục [`Game/Assets/_Project/Scripts/Domain/Contracts/`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Contracts/) |
| **17** | **Đóng gói** | Build chạy được trên máy khác, đường dẫn tương đối | Mọi truy cập file dùng đường dẫn tương đối qua `Application.streamingAssetsPath` và `Application.persistentDataPath`; kèm menu 1-click [`EditorSceneSetup.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Presentation/Bootstrap/EditorSceneSetup.cs). | [`GameBootstrap.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Presentation/Bootstrap/GameBootstrap.cs), [`EditorSceneSetup.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Presentation/Bootstrap/EditorSceneSetup.cs) |
| **18** | **Đóng gói** | Dữ liệu mẫu + test case ($\ge 20$, chia đều) | **73 Unit Tests NUnit PASS 100%** (chia đều 14 file test cho cả 2 phân hệ A & B), có sẵn `config.json` và gói tài nguyên `OptimumRealism_64x`. | Thư mục [`Game/Assets/_Project/Tests/EditMode/`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Tests/EditMode/) & [`Game/Assets/StreamingAssets/`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/StreamingAssets/) |
| **19** | **Đóng gói** | `try-catch` thống nhất, file cấu hình riêng, Git có nhánh ($\ge 20$ commit) | Bọc `try-catch` nhiều tầng tại `ConfigLoader`, `JsonWorldRepository`, `FolderResourcePackProvider`, `ResourcePackView`; đọc `StreamingAssets/config.json`; Git có nhánh `main`, `dev` và **9 nhánh `feature/*`** với trên 35 commits rõ ràng. | [`ConfigLoader.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Infrastructure/Config/ConfigLoader.cs), [`JsonWorldRepository.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Infrastructure/Persistence/JsonWorldRepository.cs), [`FolderResourcePackProvider.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Infrastructure/ResourcePacks/FolderResourcePackProvider.cs) |
| **20** | **Đóng gói** | Báo cáo, README, kịch bản demo, thuyết trình, hiểu code của nhau | Trọn bộ `README.md`, `CHANGELOG.md`, `docs/uml/*.md` và `docs/defense-guide.md` kèm kịch bản demo 7 bước và 10 câu hỏi vấn đáp. | [`README.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/README.md), [`CHANGELOG.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/CHANGELOG.md), [`docs/defense-guide.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/docs/defense-guide.md) |

---

## 2. Bảng 12 Quy tắc Nghiệp vụ (R1–R12) & Vị trí Cài đặt trong Code (Đáp ứng Tiêu chí 3)

| Mã | Phân hệ | Nội dung Quy tắc Nghiệp vụ | Lớp & Phương thức Cài đặt Chính xác | File Unit Test Kiểm chứng |
| :---: | :---: | :--- | :--- | :--- |
| **R1** | World (A) | **Không phá được khối đá nền (`Bedrock`) ở chế độ Survival.** | [`SurvivalMode.CanBreakBlock(BlockId)`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/GameModes/SurvivalMode.cs) & [`BlockInteractionService.TryBreakBlock`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Application/World/BlockInteractionService.cs) | `BlockInteractionTests.cs`, `GameModeTests.cs` |
| **R2** | World (A) | **Không đặt khối vào ô đã có khối rắn hoặc trùng vị trí chân/đầu người chơi.** | [`BlockInteractionService.TryPlaceBlock`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Application/World/BlockInteractionService.cs) | `BlockInteractionTests.cs` |
| **R3** | World (A) | **Mọi tọa độ $Y < 0$ hoặc $Y \ge 64$ (`MaxHeight`) đều bị từ chối đọc/ghi.** | [`World.IsInBounds(BlockPos)`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/World/World.cs), [`World.SetBlock`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/World/World.cs), [`Chunk.IsInBounds`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/World/Chunk.cs) | `ChunkAndWorldTests.cs` |
| **R4** | World (A) | **Thời gian đào khối = Độ cứng (`Hardness`) $\div$ Hệ số tốc độ công cụ (`ToolItem`).** | [`BlockInteractionService.CalculateBreakDuration`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Application/World/BlockInteractionService.cs) & [`ToolItem.GetDestroySpeedMultiplier`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Items/ToolItem.cs) | `BlockInteractionTests.cs` |
| **R5** | World (A) | **Phá khối ở Survival tự động rơi đúng vật phẩm (`DropItemId`, vd: `Stone` $\rightarrow$ `Cobblestone`, `Grass` $\rightarrow$ `Dirt`).** | [`BlockInteractionService.TryBreakBlock`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Application/World/BlockInteractionService.cs) & [`BlockRegistry.CreateDefault`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Blocks/BlockRegistry.cs) | `BlockInteractionTests.cs` |
| **R6** | Survival (B) | **Ngăn xếp vật phẩm thường tối đa 64 cái/ô; công cụ (`ToolItem`) tối đa 1 cái/ô.** | [`Item..ctor`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Items/Item.cs), [`ToolItem..ctor`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Items/ToolItem.cs), [`ItemStack.Add`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Items/ItemStack.cs) | `ItemAndInventoryTests.cs` |
| **R7** | World/Survival | **Tầm với tương tác tối đa $\le 5.0$ block tính từ mắt người chơi.** | [`VoxelRaycaster.Raycast(origin, dir, 5.0f)`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/World/VoxelRaycaster.cs) | `VoxelRaycasterTests.cs` |
| **R8** | Survival (B) | **Độ đói $\ge 18$ thì tự động hồi máu; độ đói $= 0$ thì bị rút máu định kỳ.** | [`Hunger.Tick(deltaTime, targetHealth, gameMode)`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Entities/Hunger.cs) | `HealthAndHungerTests.cs` |
| **R9** | Survival (B) | **Máu giảm về $0$ thì phát sự kiện `Died`, làm rơi toàn bộ túi đồ ở Survival và hồi sinh (`Respawn`) về `SpawnPoint` với đầy máu/đói.** | [`Health.TakeDamage`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Entities/Health.cs), [`DeathDropService.DropInventoryOnDeath`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Application/Survival/DeathDropService.cs), [`Player.Respawn`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Entities/Player.cs) | `HealthAndHungerTests.cs`, `DeathDropAndAtmosphereTests.cs` |
| **R10** | Survival (B) | **Chế tạo theo cơ chế nguyên tử ("đủ hết hoặc không làm gì" — Atomic Transaction).** | [`CraftingService.CanCraft`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Application/Survival/CraftingService.cs) & [`CraftingService.TryCraft`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Application/Survival/CraftingService.cs) | `CraftingServiceTests.cs` |
| **R11** | Survival (B) | **Quái thù địch (`Zombie`) chỉ sinh ra vào ban đêm (`IsNight`), cách người chơi $\ge 24$ block và giới hạn tối đa 10 con.** | [`MobSpawnerService.TrySpawnNightMob`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Application/Survival/MobSpawnerService.cs) | `MobSpawnerTests.cs` |
| **R12** | Chung (A & B) | **Chế độ Creative: miễn nhiễm sát thương, phá ngay cả Bedrock, đặt khối/ăn không tốn vật phẩm, chết giữ đồ, bay tự do.** | [`CreativeMode.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/GameModes/CreativeMode.cs), [`FirstPersonPlayerController.HandleMovement`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Presentation/Player/FirstPersonPlayerController.cs) | `GameModeTests.cs`, `MobCombatAndGameModeServiceTests.cs` |

---

## 3. Phân tích 5 Design Pattern Đã Áp dụng (Đáp ứng Tiêu chí 9, 10, 11)

| STT | Tên Design Pattern | Phụ trách | Lý do chọn (Giải quyết bài toán gì?) | Vai trò các Lớp trong Code | Cơ chế Phối hợp lúc Runtime |
| :---: | :--- | :---: | :--- | :--- | :--- |
| 1 | **Strategy Pattern**<br/>*(Mẫu Chiến lược)* | **Thành viên A** | Cho phép hoán đổi thuật toán sinh địa hình (đồi núi tự nhiên Perlin vs mặt phẳng kiểm thử) và hoán đổi nguồn nạp gói tài nguyên (gói mặc định tích hợp sẵn vs gói đọc từ thư mục ngoài) mà không sửa đổi lớp `World` hay `ResourcePackService`. | - **Strategy Interface:** `ITerrainGenerator`, `ITextureAtlasProvider`<br/>- **Concrete Strategies:** `PerlinTerrainGenerator`, `FlatTerrainGenerator`; `BuiltInAtlasProvider`, `FolderResourcePackProvider`<br/>- **Context:** `World`, `ResourcePackService` | `GameBootstrap` khởi tạo chiến lược cụ thể và tiêm qua constructor của `World` / `ResourcePackService`. Khi `World.GenerateChunk` chạy, nó ủy quyền đa hình qua `_terrainGenerator.GetBlockAt(pos, seed)`. |
| 2 | **Observer Pattern**<br/>*(Mẫu Quan sát qua C# `event`)* | **Thành viên A & B** | Giữ cho tầng `Minecraft.Domain` thuần C# 100% (`noEngineReferences: true`), hoàn toàn không biết tới sự tồn tại của `UnityEngine`, nhưng các View và UI ở tầng `Presentation` vẫn tự động cập nhật tức thì khi dữ liệu thay đổi. | - **Subject (Nguồn phát):** `World` (`BlockChanged`), `ResourcePackService` (`ResourcePackChanged`), `Health` (`HealthChanged`, `Died`), `Hunger` (`HungerChanged`), `Inventory` (`InventoryChanged`), `DayNightCycle` (`PhaseChanged`, `TimeUpdated`)<br/>- **Concrete Observers:** `WorldView`, `ResourcePackView`, `SandboxHudView`, `DayNightView`, `AtmosphereFogView` | Khi khởi tạo tại `GameBootstrap`, các View đăng ký hàm callback (`+=`) vào sự kiện của Domain/Application. Khi trạng thái biến đổi (ví dụ `World.SetBlock`), Subject kích hoạt `Invoke`, các Observer tự động dựng lại Mesh hoặc cập nhật thanh máu/sương mù. |
| 3 | **State Pattern**<br/>*(Mẫu Trạng thái)* | **Thành viên B** | Đóng gói toàn bộ quy tắc phân quyền tác nhân theo chế độ chơi (`Survival`, `Creative`, `Adventure`) vào các lớp riêng biệt, xóa bỏ hoàn toàn cấu trúc `if/else` hoặc `switch` kiểm tra chế độ chơi rải rác trong code. | - **State Interface:** `IGameMode`<br/>- **Concrete States:** `SurvivalMode`, `CreativeMode`, `AdventureMode`<br/>- **Context:** `Player` (`CurrentGameMode`), điều phối chuyển đổi bởi `GameModeService` | Khi người chơi nhấn `F1`, `GameModeService` gọi `player.ChangeGameMode(new CreativeMode())`. Mọi hành vi bay, nhận sát thương, tiêu hao khối và phá Bedrock lập tức đổi theo đối tượng State hiện hành. |
| 4 | **Factory Pattern**<br/>*(Mẫu Nhà máy)* | **Thành viên B** | Tách biệt logic khởi tạo các loài sinh vật cụ thể (`Zombie`, `Cow`, `Skeleton`) khỏi dịch vụ `MobSpawnerService`, tuân thủ nguyên lý Đảo ngược phụ thuộc (DIP) và Đóng/Mở (OCP). | - **Creator Interface:** `IMobFactory`<br/>- **Concrete Creator:** `MobFactory`<br/>- **Abstract Product:** `Mob`<br/>- **Concrete Products:** `Zombie`, `Cow`, `Skeleton` | Khi đến ban đêm, `MobSpawnerService` chỉ gọi `_mobFactory.CreateMob(MobType.Zombie, spawnPos)` và nhận về kiểu trừu tượng `Mob`, không phụ thuộc vào constructor của từng loài. |
| 5 | **Repository Pattern**<br/>*(Mẫu Kho lưu trữ)* | **Thành viên A** | Cô lập hoàn toàn chi tiết kỹ thuật tuần tự hóa JSON và đọc/ghi tệp trên đĩa (`System.IO`, `System.Text.Json`) ở tầng `Infrastructure`, giúp tầng `Application` (`WorldPersistenceService`) chỉ làm việc với dữ liệu thuần. | - **Repository Interface:** `IWorldRepository`<br/>- **Concrete Repository:** `JsonWorldRepository`<br/>- **Client Service:** `WorldPersistenceService`<br/>- **Data Transfer Objects:** `WorldSaveDto`, `PlayerSaveDto`, `BlockChangeDto`, `InventorySlotDto` | Khi nhấn `F5`/`F9`, `WorldPersistenceService` ánh xạ trạng thái `World` và `Player` sang `WorldSaveDto` rồi gọi `_repository.Save/Load`. Nếu sau này đổi từ JSON sang Binary/SQLite, chỉ cần viết lớp mới thực thi `IWorldRepository`. |

---

## 4. Hướng dẫn Thực hành Trực tiếp 3 Bài Mở rộng Code Ngay Tại Bàn Vấn đáp (Tiêu chí 12 — $< 5$ phút/bài)

Giảng viên chấm **Tiêu chí 12** thường yêu cầu sinh viên mở IDE và thêm nhanh 1 tính năng mới để kiểm tra xem kiến trúc OOP và Design Pattern có thực sự dễ mở rộng (Open/Closed Principle) hay không. Dưới đây là 3 bài thực hành chuẩn:

### Bài Mở rộng 1 (Thành viên A): Thêm Khối mới `Obsidian` (Hắc diện thạch — Độ cứng rất cao `50.0f`)
Chỉ cần sửa đúng **4 vị trí khai báo danh mục** mà không chạm vào bất kỳ dòng logic nào của `World`, `Chunk` hay `BlockInteractionService`:
1. Mở [`BlockId.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Contracts/BlockId.cs): Thêm `Obsidian = 11` vào enum `BlockId`.
2. Mở [`ItemId.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Contracts/ItemId.cs): Thêm `Obsidian = 11` vào enum `ItemId`.
3. Mở [`BlockRegistry.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Blocks/BlockRegistry.cs) (trong `CreateDefault()`):
   ```csharp
   registry.Register(new BlockType(
       id: BlockId.Obsidian,
       name: "Obsidian",
       isSolid: true,
       isTransparent: false,
       hardness: 50.0f,
       dropItemId: ItemId.Obsidian,
       topTextureIndex: 5, bottomTextureIndex: 5, sideTextureIndex: 5));
   ```
4. Mở [`ItemRegistry.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Items/ItemRegistry.cs) (trong `CreateDefault()`):
   ```csharp
   registry.Register(new BlockItem(ItemId.Obsidian, "Obsidian", BlockId.Obsidian));
   ```

### Bài Mở rộng 2 (Thành viên B): Thêm Loài Quái mới kế thừa `Mob` & Đăng ký vào `MobFactory`
Dự án đã cài sẵn lớp minh chứng [`Skeleton.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Entities/Mobs/Skeleton.cs). Nếu giảng viên yêu cầu thêm tại chỗ loài mới (ví dụ `Creeper`):
1. Mở [`MobType.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Entities/Mobs/MobType.cs): Thêm `Creeper = 3`.
2. Tạo lớp `Creeper : Mob` ghi đè `UpdateAI(Vector3 playerPos, bool isPlayerTargetable, float deltaTime)`:
   ```csharp
   public sealed class Creeper : Mob
   {
       public Creeper(Vector3 spawnPos)
           : base(MobType.Creeper, spawnPos, maxHealth: 20, moveSpeed: 3.0f, attackDamage: 15, isHostile: true) { }

       public override void UpdateAI(Vector3 playerPos, bool isPlayerTargetable, float deltaTime)
       {
           // Áp sát người chơi trong bán kính 16 block
           if (!IsAlive || !isPlayerTargetable) { CurrentState = MobState.Idle; return; }
           Vector3 diff = playerPos - Position;
           if (diff.Length() <= 16f && diff.Length() > 0.001f)
           {
               CurrentState = MobState.Chase;
               MoveTo(Position + Vector3.Normalize(diff) * (MoveSpeed * deltaTime));
           }
       }
   }
   ```
3. Mở [`MobFactory.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/Entities/Mobs/MobFactory.cs): Thêm nhánh `MobType.Creeper => new Creeper(spawnPosition)` vào biểu thức `switch`. Toàn bộ `MobSpawnerService` và `MobCombatService` hoạt động ngay lập tức nhờ tính đa hình (LSP)!

### Bài Mở rộng 3 (Cả 2 Thành viên): Thêm Chế độ Chơi mới thực thi `IGameMode` (State Pattern)
Dự án đã cài sẵn lớp minh chứng [`AdventureMode.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Domain/GameModes/AdventureMode.cs) (cấm phá mọi khối). Nếu giảng viên yêu cầu thêm `SpectatorMode` (Chế độ Khán giả: bay được, bất tử, không phá được khối nào):
1. Tạo lớp `SpectatorMode : IGameMode`:
   ```csharp
   public sealed class SpectatorMode : IGameMode
   {
       public string ModeName => "Spectator";
       public bool CanFly => true;
       public bool IsInstantBreak => false;
       public bool CanTakeDamage => false;
       public bool ConsumesItemsOnPlace => false;
       public bool CanBreakBlock(BlockId blockId) => false;
   }
   ```
2. Thêm phương thức `SwitchToSpectator(Player player)` trong [`GameModeService.cs`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Game/Assets/_Project/Scripts/Application/Survival/GameModeService.cs): `player.ChangeGameMode(new SpectatorMode());`. Không cần sửa bất kỳ dòng code nào trong `Player`, `Health`, hay `BlockInteractionService`!

---

## 5. Kịch bản Demo 7 Bước Trên Máy Khi Bảo vệ (Mất ~5 phút)

1. **Bước 1 — Khởi tạo Scene 1-Click & Bấm Play (UC01, UC02):**
   - Trên thanh menu Unity Editor, bấm **`Minecraft OOP -> 1. Auto-Setup Playable Game Scene`** rồi nhấn nút **Play (▶)**.
   - Chỉ vào Console log `[GameBootstrap] Khởi tạo hệ thống thành công!` và giới thiệu địa hình đồi núi sinh tự động bằng `PerlinTerrainGenerator` kèm剔 trừ mặt khuất (Face Culling).
2. **Bước 2 — Di chuyển, Dò tia DDA, Phá Khối & Đặt Khối (UC03, UC04, R1–R5, R7):**
   - Dùng `WASD` + `Space` di chuyển trên địa hình.
   - Nhấn **Chuột trái** đập khối Cỏ/Đá trong tầm $\le 5$ block $\rightarrow$ khối biến mất, Mesh cập nhật tức thì (Observer `BlockChanged`) và vật phẩm rơi vào thanh Hotbar dưới đáy màn hình.
   - Chọn ô Đất (`Phím 1`), nhấn **Chuột phải** đặt khối lên mặt đất; thử đặt trùng vào chân nhân vật để chứng minh **Quy tắc R2** chặn lại.
3. **Bước 3 — Mở Bàn Chế Tạo & Chế tạo Vật phẩm Atomic (UC08, UC09, R6, R10):**
   - Nhấn phím **`C`** (hoặc **`E`**) mở menu **Bàn Chế Tạo** góc phải màn hình.
   - Bấm chế tạo `Planks x4` từ `Wood`, sau đó chế tạo `CraftingTable x1` và `WoodenPickaxe x1`. Chỉ vào ô Cúp gỗ chỉ xếp tối đa `x1` (**Quy tắc R6**) và các công thức thiếu nguyên liệu tự động bị khóa mờ (**Quy tắc R10**).
4. **Bước 4 — Ăn Thức ăn & Kiểm tra Nhịp Sinh học Máu/Đói (UC10, R8):**
   - Bấm **`Phím 3`** chọn quả Táo (`Apple`), nhấn **Chuột phải** để ăn ketika đói giảm $\rightarrow$ điểm đói và máu tăng lên, số lượng táo giảm đi 1.
5. **Bước 5 — Nhấn `F1` Đổi Chế độ Chơi Survival $\rightarrow$ Creative $\rightarrow$ Adventure (UC11, R1, R12 — State Pattern):**
   - Nhấn **`F1`** chuyển sang **Creative**: giữ `Space` để bay lên cao, đặt khối không bị trừ số lượng, phá được cả khối `Bedrock` ở tầng $Y=0$.
   - Nhấn **`F1`** lần nữa chuyển sang **Adventure**: thử nhấn Chuột trái đập khối $\rightarrow$ bị chặn hoàn toàn (`CanBreakBlock == false`). Nhấn **`F1`** quay về **Survival**.
6. **Bước 6 — Nhấn `F2` Đổi Gói Tài nguyên Runtime `16x` $\leftrightarrow$ `64x` (UC06 — Strategy + Observer Pattern):**
   - Nhấn phím **`F2`**: hệ thống lập tức chuyển từ gói mặc định `default (16x)` sang gói `OptimumRealism_64x (64x)` đọc từ `StreamingAssets/ResourcePacks/`, bề mặt khối đổi sang họa tiết độ phân giải cao ngay khi đang chơi mà không cần khởi động lại.
7. **Bước 7 — Nhấn `F5` Lưu Thế giới & `F9` Tải Thế giới (UC05 — Repository Pattern + `try-catch`):**
   - Xây một cột khối làm dấu, nhấn **`F5`** để lưu nhanh ra `quicksave.json`.
   - Phá hủy cột khối vừa xây và di chuyển ra chỗ khác, sau đó nhấn **`F9`** $\rightarrow$ toàn bộ khối đã xây, vị trí, máu, đói và túi đồ được khôi phục nguyên trạng 100%.

---

## 6. Bộ 10 Câu hỏi Giảng viên Hay Hỏi Xoáy Khi Vấn đáp & Câu Trả lời Mẫu Ngắn gọn

### Phần A — Câu hỏi dành cho Thành viên A (Phân hệ World & Infrastructure)

1. **Hỏi:** *"Tại sao nhóm lại bật `noEngineReferences: true` ở `Minecraft.Domain.asmdef`? Làm vậy có tự làm khó mình không?"*
   - **Trả lời:** Bật `noEngineReferences: true` giúp trình biên dịch C# cưỡng chế tầng Domain không được phép `using UnityEngine;`. Nhờ vậy, toàn bộ 48 lớp lõi nghiệp vụ (World, Chunk, Inventory, Mob, Raycaster) là C# thuần 100%, không phụ thuộc vào vòng đời MonoBehaviour, giúp nhóm chạy được **73 Unit Tests trên .NET 9 chỉ mất chưa tới 1 giây** mà không cần bật Unity Editor.
2. **Hỏi:** *"Tại sao nhóm tách `IWorldReader` và `IWorldWriter` thành 2 interface riêng mà không gộp chung vào `IWorld`?"*
   - **Trả lời:** Để tuân thủ nguyên lý **Phân tách giao diện (Interface Segregation Principle - Tiêu chí 15)**. Các lớp chỉ cần đọc dữ liệu khối như `ChunkMeshBuilder` (dựng lưới) hay `VoxelRaycaster` (dò tia) chỉ được nhận `IWorldReader`, đảm bảo về mặt biên dịch là các lớp đó không bao giờ vô tình gọi hàm `SetBlock` làm hỏng dữ liệu thế giới.
3. **Hỏi:** *"Khi người chơi đập vỡ 1 khối trong Chunk chứa $16 \times 64 \times 16 = 16.384$ khối, làm sao để game không bị giật (lag)?"*
   - **Trả lời:** Nhóm áp dụng 2 kỹ thuật tối ưu: (1) **Face Culling 6 hướng** trong `ChunkMeshBuilder.BuildMesh` chỉ sinh tam giác cho những mặt tiếp giáp với khối trong suốt/không khí, giảm hơn 85% số đỉnh; (2) **Observer cục bộ** trong `WorldView.OnBlockChanged` chỉ dựng lại Mesh cho đúng 1 Chunk chứa khối bị đập (và Chunk láng giềng nếu khối nằm sát biên $X=0, 15$ hoặc $Z=0, 15$), tuyệt đối không dựng lại toàn bộ thế giới.
4. **Hỏi:** *"Nếu giảng viên cố tình xóa file `config.json` hoặc sửa file `quicksave.json` thành chuỗi rác `{{{abc` thì game có bị crash (văng) không?"*
   - **Trả lời:** Hoàn toàn không bị crash (**Tiêu chí 19**). Trong `ConfigLoader.LoadConfig` và `JsonWorldRepository.Load`, nhóm bọc `try-catch` bắt riêng `JsonException` và `IOException`. Nếu `config.json` lỗi hoặc mất, hệ thống tự động trả về `GameConfigDto.CreateDefault()`. Nếu file save lỗi, `Load` trả về `false` và giữ nguyên trạng thái thế giới hiện tại.
5. **Hỏi:** *"Khi bấm `F2` đổi Resource Pack từ 16x sang 64x, Strategy Pattern và Observer Pattern phối hợp với nhau như thế nào?"*
   - **Trả lời:** `ResourcePackService` đóng vai trò Context giữ giao diện chiến lược `ITextureAtlasProvider` (`FolderResourcePackProvider` kèm fallback `BuiltInAtlasProvider`). Khi gọi `SwitchPack("OptimumRealism_64x")`, Strategy nạp `ResourcePackManifest` từ đĩa, sau đó `ResourcePackService` phát sự kiện Observer `ResourcePackChanged`. Lớp `ResourcePackView` ở tầng Presentation lắng nghe sự kiện này sẽ đọc tệp `.png` và cập nhật `Material` của `WorldView` ngay lập tức.

### Phần B — Câu hỏi dành cho Thành viên B (Phân hệ Survival, Entities & Crafting)

6. **Hỏi:** *"Barem yêu cầu 'Phân quyền tác nhân' (Tiêu chí 4) và 'Không dùng Singleton'. Nhóm đã giải quyết việc phân quyền giữa người chơi Survival và Creative như thế nào?"*
   - **Trả lời:** Nhóm áp dụng **State Pattern** thông qua interface `IGameMode` với 3 lớp cụ thể: `SurvivalMode`, `CreativeMode` và `AdventureMode`. Thực thể `Player` giữ một tham chiếu `_currentGameMode`. Khi cần kiểm tra quyền bay (`CanFly`), nhận sát thương (`CanTakeDamage`), tiêu hao khối (`ConsumesItemsOnPlace`) hay phá Bedrock (`CanBreakBlock`), các Service hỏi trực tiếp qua đa hình `IGameMode` thay vì dùng biến toàn cục Singleton hay câu lệnh `if/else`.
7. **Hỏi:** *"Giải thích sự khác biệt giữa quan hệ Hợp thành (Composition) và Kết tập (Aggregation) thông qua các lớp `Player`, `Inventory`, `ItemStack` và `Item` trong code của bạn?"*
   - **Trả lời:**
     - **Hợp thành (`*--`):** `Player` tự khởi tạo `new Inventory()` trong constructor và nắm giữ vòng đời của nó; `Inventory` khởi tạo mảng 36 `ItemStack`. Khi `Player` bị hủy, `Inventory` và các ô `ItemStack` đó mất theo.
     - **Kết tập (`o--`):** Mỗi `ItemStack` chỉ giữ tham chiếu `_definition` trỏ tới đối tượng `Item` dùng chung trong `ItemRegistry`. Khi người chơi vứt bỏ hoặc dùng hết `ItemStack` (`Count == 0`), định nghĩa `Item` gốc trong `ItemRegistry` vẫn tồn tại nguyên vẹn.
8. **Hỏi:** *"Quy tắc R10 ('Chế tạo đủ hết hoặc không làm gì') được cài đặt trong `CraftingService.TryCraft` như thế nào để tránh lỗi trừ một nửa nguyên liệu rồi mới phát hiện thiếu món thứ hai?"*
   - **Trả lời:** Nhóm tách thành 2 pha rõ rệt trong `CraftingService`: Pha 1 gọi `CanCraft(recipe, inventory)` duyệt qua toàn bộ danh sách `recipe.Ingredients` và kiểm tra `inventory.CountItem(id) >= RequiredCount`. Chỉ khi **tất cả** nguyên liệu đều đủ (`CanCraft == true`), Pha 2 mới bắt đầu vòng lặp gọi `inventory.RemoveItem` và `inventory.AddItem`. Nhờ vậy giao dịch luôn có tính nguyên tử (Atomic).
9. **Hỏi:** *"Tính đa hình (Polymorphism) và nguyên lý Thay thế Liskov (LSP - Tiêu chí 14) được thể hiện ở đâu trong hệ thống Mob và Item?"*
   - **Trả lời:**
     - Ở nhánh `Entity -> Mob`: `MobSpawnerService` quản lý danh sách chung `List<Mob> _activeMobs`. Trong hàm `Tick`, vòng lặp chỉ gọi `mob.UpdateAI(...)` trên kiểu cha `Mob`, nhưng lúc runtime `Zombie` tự động lao vào đuổi đánh cận chiến, `Skeleton` giữ khoảng cách 6 block, còn `Cow` chỉ đi dạo hiền lành.
     - Ở nhánh `Item`: `Inventory` chỉ lưu trữ kiểu cha `Item`, nhưng khi gọi `item.Use(context)` hoặc kiểm tra `MaxStackSize`, `BlockItem` cho phép xếp 64 khối, `ToolItem` giới hạn xếp 1 công cụ và nhân tốc độ đào, còn `FoodItem` hồi máu và độ đói.
10. **Hỏi:** *"Khi người chơi bị Zombie đánh chết ở chế độ Survival (Quy tắc R9), luồng sự kiện từ lúc máu về 0 đến khi rơi đồ và hồi sinh diễn ra qua những lớp nào?"*
    - **Trả lời:** Khi `MobCombatService` gọi `player.Health.TakeDamage()`, nếu `_current` giảm về `0`, lớp `Health` lập tức phát sự kiện `Died`. Hàm lắng nghe tại `GameBootstrap` gọi `DeathDropService.DropInventoryOnDeath(player)`: dịch vụ kiểm tra `player.CurrentGameMode.ConsumesItemsOnPlace == true`, duyệt 36 ô túi đồ tạo ra các thực thể `DroppedItemEntity : Entity` nằm trên mặt đất, gọi `player.Inventory.Clear()`, và cuối cùng gọi `player.Respawn()` đưa người chơi về `SpawnPoint` với 20 HP và 20 Hunger.
