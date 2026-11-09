# Tuần T4 — Hoàn thiện tầng "Nên có" & Đông cứng tính năng (`v0.6.0`) Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Hoàn thành trọn bộ Tuần T4 (Hoàn thiện tầng "Nên có" & Đông cứng tính năng `v0.6.0`) gồm hệ thống Đổi Resource Pack Runtime (Strategy + Observer Pattern), Rơi vật phẩm khi người chơi chết ở Survival & Tự động thu nhặt (`DroppedItemEntity : Entity`), cùng lớp hiển thị Texture Pack và Hậu kỳ Sương mù/Ánh sáng môi trường theo chu kỳ Ngày/Đêm.

**Architecture:** Tuân thủ triệt để phân tầng 4 Assembly theo barem [S2]:
- `Minecraft.Domain`: C# thuần 100% (`noEngineReferences: true`), chứa `ResourcePackManifest`, `ITextureAtlasProvider`, `DroppedItemEntity : Entity`.
- `Minecraft.Application`: C# thuần 100%, chỉ tham chiếu `Minecraft.Domain`, chứa `ResourcePackService` (UC06b) và `DeathDropService` (R9 mở rộng).
- `Minecraft.Infrastructure`: C# chuẩn (`System.IO`, `System.Text.Json`), chứa `BuiltInAtlasProvider` và `FolderResourcePackProvider` đọc `pack.mcmeta` và kiểm tra file `.png` trong `StreamingAssets/ResourcePacks/` với `try-catch` an toàn (Tiêu chí 19).
- `Minecraft.Presentation`: Chứa `ResourcePackView`, `AtmosphereFogView` và cập nhật `GameBootstrap` nối phím tắt `F2` đổi gói tài nguyên.

**Tech Stack:** C# 12 / .NET 9, Unity 6 (6000.6.8f1), NUnit 3, System.Text.Json / System.IO, System.Numerics.

**Spec:** `docs/brainstorm.md` (Mục 2, 3.1–3.5, Lộ trình Tuần T4), `AGENTS.md` (Quy tắc OOP, Barem [S2]).

---

## Task Decomposition

### PHẦN 1: PHÂN HỆ WORLD & INFRASTRUCTURE A (`feature/t4-a-resource-packs`)

- [x] **Task 1.1: Hợp đồng & Thực thể Gói Tài nguyên (Domain)**
  - **Files:**
    - Create: `Game/Assets/_Project/Scripts/Domain/ResourcePacks/ResourcePackManifest.cs`
    - Create: `Game/Assets/_Project/Scripts/Domain/Contracts/ITextureAtlasProvider.cs`
  - **Description:**
    - `ResourcePackManifest`: Thực thể bất biến lưu `PackId`, `DisplayName`, `Description`, `PackFormat`, `TileResolution` (mặc định 16 hoặc 64), `IReadOnlyDictionary<BlockId, string> BlockTexturePaths`. Kiểm tra `PackId` không rỗng và `TileResolution > 0`.
    - `ITextureAtlasProvider`: Hợp đồng Strategy Pattern gồm `ProviderName`, `GetAvailablePacks()`, `LoadPack(string packId)`.

- [x] **Task 1.2: Hai Chiến lược Nạp Gói Tài nguyên & Dịch vụ Đổi Resource Pack (Infrastructure & Application)**
  - **Files:**
    - Create: `Game/Assets/_Project/Scripts/Infrastructure/ResourcePacks/BuiltInAtlasProvider.cs`
    - Create: `Game/Assets/_Project/Scripts/Infrastructure/ResourcePacks/FolderResourcePackProvider.cs`
    - Create: `Game/Assets/_Project/Scripts/Application/World/ResourcePackService.cs`
    - Test: `Game/Assets/_Project/Tests/EditMode/ResourcePackTests.cs`
  - **Description:**
    - `BuiltInAtlasProvider`: Trả về manifest chuẩn `default` (`TileResolution = 16`) với ánh xạ texture mặc định cho mọi `BlockId`.
    - `FolderResourcePackProvider`: Quét các thư mục con trong `_resourcePacksRootPath`, đọc `pack.mcmeta` bọc `try-catch` an toàn (fallback về mô tả mặc định hoặc `BuiltInAtlasProvider`), kiểm tra các file `.png` trong `assets/minecraft/textures/block/` (`stone.png`, `dirt.png`, `grass_block_top.png`, `oak_planks.png`, `cobblestone.png`, `bedrock.png`, `sand.png`, `oak_log.png`, `oak_leaves.png`, `crafting_table_top.png`).
    - `ResourcePackService`: Điều phối UC06b, nhận `ITextureAtlasProvider` qua constructor, cho phép `SetProvider`, giữ `CurrentPack`, phát sự kiện `ResourcePackChanged` khi gọi `SwitchPack(string packId)`.

---

### PHẦN 2: PHÂN HỆ SURVIVAL & PRESENTATION B (`feature/t4-b-death-drops-atmosphere`)

- [x] **Task 2.1: Thực thể Vật phẩm Rơi & Dịch vụ Rơi/Nhặt Đồ Khi Chết (Domain & Application)**
  - **Files:**
    - Create: `Game/Assets/_Project/Scripts/Domain/Entities/DroppedItemEntity.cs`
    - Create: `Game/Assets/_Project/Scripts/Application/Survival/DeathDropService.cs`
    - Test: `Game/Assets/_Project/Tests/EditMode/DeathDropAndAtmosphereTests.cs`
  - **Description:**
    - `DroppedItemEntity : Entity`: Giữ `ItemStack Payload` (kiểm tra `!payload.IsEmpty`), `DespawnTimeSeconds = 300f`, `ElapsedSeconds`, `IsExpired`. Ghi đè `Tick(float deltaTime)` tăng thời gian trôi và tự hủy (`Health.TakeDamage(Health.Max, null)`) khi hết hạn.
    - `DeathDropService`: Quản lý `ActiveDrops`, `DropInventoryOnDeath(Player)` làm rơi toàn bộ đồ trong 36 ô túi đồ nếu `ConsumesItemsOnPlace == true` rồi xóa túi đồ và gọi `player.Respawn()`; giữ nguyên đồ nếu ở Creative. `CollectNearbyDrops(Player, float pickupRadius = 2.0f)` tự động nhặt lại các vật phẩm trong bán kính `2.0f`.

- [x] **Task 2.2: Presentation Views & GameBootstrap (Presentation)**
  - **Files:**
    - Create: `Game/Assets/_Project/Scripts/Presentation/World/ResourcePackView.cs`
    - Create: `Game/Assets/_Project/Scripts/Presentation/Environment/AtmosphereFogView.cs`
    - Modify: `Game/Assets/_Project/Scripts/Presentation/World/WorldView.cs`
    - Modify: `Game/Assets/_Project/Scripts/Presentation/Bootstrap/GameBootstrap.cs`
  - **Description:**
    - `ResourcePackView`: Lắng nghe `ResourcePackService.ResourcePackChanged`, đọc byte ảnh `.png` bằng `File.ReadAllBytes` + `ImageConversion.LoadImage` với `FilterMode.Point` và gán vào Material của `WorldView`.
    - `AtmosphereFogView`: Lắng nghe `DayNightCycle.PhaseChanged` và `TimeUpdated` để nội suy mượt màu sương mù (`RenderSettings.fogColor`, `RenderSettings.fogDensity`) và ánh sáng môi trường (`RenderSettings.ambientLight`).
    - `GameBootstrap`: Khởi tạo `FolderResourcePackProvider`, `ResourcePackService`, `DeathDropService` và nối phím tắt `F2` đổi nhanh giữa `default` và `OptimumRealism_64x`.
