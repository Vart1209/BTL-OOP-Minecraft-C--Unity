# Week T0 Setup & Domain Contracts Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Chuẩn hóa cấu hình Unity `6000.6.8f1`, di chuyển Texture Pack vào `StreamingAssets/ResourcePacks`, và xây dựng bộ Hợp đồng chung tầng `Minecraft.Domain` (gồm các file tách rời trong thư mục `Domain/Contracts/` có comment giải thích OOP đầy đủ + bộ Unit Test NUnit kiểm chứng).

**Architecture:** Tầng `Minecraft.Domain` hoàn toàn độc lập với `UnityEngine` (`noEngineReferences: true`). Toàn bộ giao tiếp giữa Phân hệ A (World) và Phân hệ B (Survival) đi qua bộ hợp đồng trong `Game/Assets/_Project/Scripts/Domain/Contracts/`, mỗi struct/enum/interface tách 1 file `.cs` riêng kèm XML documentation tiếng Việt giải thích rõ nguyên lý OOP phục vụ vấn đáp.

**Tech Stack:** C# (.NET Standard 2.1 / .NET 9), Unity `6000.6.8f1`, NUnit 3.

**Spec:** `docs/brainstorm.md`

## Global Constraints

- `Minecraft.Domain`: **C# thuần 100%**, bật `noEngineReferences: true`. Tuyệt đối không `using UnityEngine;`.
- Mọi field trong Domain phải là `private` (hoặc `protected` nếu cần cho kế thừa hợp lý), truy cập qua property và kiểm tra hợp lệ dữ liệu đầu vào tại các phương thức.
- `PascalCase` cho Class, Struct, Interface (`I...`), Method, Property, Public Event; `_camelCase` cho private field; mỗi class/interface chính tách 1 file `.cs` riêng khớp tên.
- Cập nhật `CHANGELOG.md` theo đúng định dạng Semantic Versioning (`v0.2.0`) trước khi kết thúc.

## Review Focus

- Tọa độ âm khi quy đổi `BlockPos` sang `ChunkCoord` và tọa độ cục bộ trong chunk (`-1` phải thuộc chunk `-1`, local index `15`, không bị lệch về `0` hay số âm do phép chia `/` và `%` mặc định của C#).
- Kiểm tra biên chiều cao thế giới (`Y < 0` hoặc `Y >= MaxHeight`) trong hợp đồng `IWorldReader.IsInBounds` và `IWorldWriter.SetBlock` (Quy tắc R3).
- Không cho phép phá khối `BlockId.Bedrock` ở chế độ Survival nhưng cho phép ở chế độ Creative thông qua `IGameMode.CanBreakBlock(BlockId)` (Quy tắc R1 & R12).
- Sự kiện `IWorldReader.BlockChanged` chỉ phát ra khi block tại vị trí đó thực sự thay đổi (Observer Pattern).
- So sánh giá trị (`Equals`, `GetHashCode`, `==`, `!=`) của `BlockPos` và `ChunkCoord` khi dùng làm key trong `Dictionary<ChunkCoord, Chunk>`.

---

### Task 1: Cập nhật cấu hình Unity `6000.6.8f1` & Di chuyển Resource Pack

**Files:**
- Modify: `docs/brainstorm.md`
- Modify: `README.md`
- Modify: `Game/Assets/StreamingAssets/config.json`
- Create: `Game/ProjectSettings/ProjectVersion.txt`
- Move: `Game/MAIN TEXTURE PACK Optimum Realism R3.7.0 64x 2` -> `Game/Assets/StreamingAssets/ResourcePacks/OptimumRealism_64x`

**Interfaces:**
- Consumes: none
- Produces: Chuẩn cấu hình project Unity `6000.6.8f1` và thư mục `Game/Assets/StreamingAssets/ResourcePacks/OptimumRealism_64x`

- [x] **Step 1: Cập nhật `docs/brainstorm.md` và `README.md` sang phiên bản `6000.6.8f1` (hoặc bản Unity 6 mới nhất thống nhất giữa 2 máy)**
- [x] **Step 2: Tạo `Game/ProjectSettings/ProjectVersion.txt` ghi nhận `6000.6.8f1`**
- [x] **Step 3: Di chuyển thư mục Texture Pack vào `Game/Assets/StreamingAssets/ResourcePacks/OptimumRealism_64x` và cập nhật `config.json`**

---

### Task 2: Kiểu giá trị tọa độ `BlockPos` và `ChunkCoord`

**Files:**
- Create: `Game/Assets/_Project/Scripts/Domain/Contracts/ChunkCoord.cs`
- Create: `Game/Assets/_Project/Scripts/Domain/Contracts/BlockPos.cs`
- Test: `Game/Assets/_Project/Tests/EditMode/DomainContractsTests.cs`

**Interfaces:**
- Consumes: none
- Produces:
  - `Minecraft.Domain.Contracts.ChunkCoord(int x, int z)` (`X`, `Z`, `Equals`, `GetHashCode`, `==`, `!=`)
  - `Minecraft.Domain.Contracts.BlockPos(int x, int y, int z)` (`X`, `Y`, `Z`, `Offset(int dx, int dy, int dz)`, `ToChunkCoord(int chunkWidth = 16)`, `ToLocalPos(int chunkWidth = 16)`, `+`, `-`, `==`, `!=`)

- [x] **Step 1: Viết failing tests cho `BlockPos` và `ChunkCoord` (bao gồm tọa độ âm và `ArgumentOutOfRangeException` khi `chunkWidth <= 0`)**
- [x] **Step 2: Chạy test xác nhận FAIL (RED)**
- [x] **Step 3: Cài đặt `ChunkCoord.cs` và `BlockPos.cs` với đầy đủ comment tiếng Việt giải thích OOP**
- [x] **Step 4: Chạy test xác nhận PASS (GREEN)**

---

### Task 3: Bảng mã định danh `BlockId`, `ItemId` và `BlockChangedEventArgs`

**Files:**
- Create: `Game/Assets/_Project/Scripts/Domain/Contracts/BlockId.cs`
- Create: `Game/Assets/_Project/Scripts/Domain/Contracts/ItemId.cs`
- Create: `Game/Assets/_Project/Scripts/Domain/Contracts/BlockChangedEventArgs.cs`
- Modify: `Game/Assets/_Project/Tests/EditMode/DomainContractsTests.cs`

**Interfaces:**
- Consumes: `Minecraft.Domain.Contracts.BlockPos`
- Produces:
  - `enum BlockId : byte` (`Air = 0`, `Bedrock = 1`, `Stone = 2`, `Dirt = 3`, `Grass = 4`, `Wood = 5`, `Leaves = 6`, `Sand = 7`, `Planks = 8`, `CraftingTable = 9`, `Cobblestone = 10`)
  - `enum ItemId : ushort` (khớp ID block `0..10` và định danh công cụ/vật phẩm `100+`, thức ăn `200+`)
  - `readonly struct BlockChangedEventArgs(BlockPos position, BlockId oldBlock, BlockId newBlock)` (`Position`, `OldBlock`, `NewBlock`)

- [x] **Step 1: Viết failing tests cho `BlockId`, `ItemId` và `BlockChangedEventArgs`**
- [x] **Step 2: Chạy test xác nhận FAIL (RED)**
- [x] **Step 3: Cài đặt `BlockId.cs`, `ItemId.cs`, `BlockChangedEventArgs.cs` kèm comment tiếng Việt chi tiết**
- [x] **Step 4: Chạy test xác nhận PASS (GREEN)**

---

### Task 4: Bộ Interface Hợp đồng (`IWorldReader`, `IWorldWriter`, `ITerrainGenerator`, `IGameMode`)

**Files:**
- Create: `Game/Assets/_Project/Scripts/Domain/Contracts/IWorldReader.cs`
- Create: `Game/Assets/_Project/Scripts/Domain/Contracts/IWorldWriter.cs`
- Create: `Game/Assets/_Project/Scripts/Domain/Contracts/ITerrainGenerator.cs`
- Create: `Game/Assets/_Project/Scripts/Domain/Contracts/IGameMode.cs`
- Modify: `Game/Assets/_Project/Tests/EditMode/DomainContractsTests.cs`

**Interfaces:**
- Consumes: `BlockPos`, `ChunkCoord`, `BlockId`, `BlockChangedEventArgs`
- Produces:
  - `IWorldReader`: `int MaxHeight { get; }`, `bool IsInBounds(BlockPos position)`, `BlockId GetBlock(BlockPos position)`, `event Action<BlockChangedEventArgs> BlockChanged`
  - `IWorldWriter`: `bool SetBlock(BlockPos position, BlockId blockId)`
  - `ITerrainGenerator`: `BlockId GetBlockAt(BlockPos worldPosition, int seed)`
  - `IGameMode`: `string ModeName { get; }`, `bool CanTakeDamage { get; }`, `bool ConsumesItemsOnPlace { get; }`, `bool InstantBreak { get; }`, `bool CanFly { get; }`, `bool CanBreakBlock(BlockId blockId)`

- [x] **Step 1: Viết failing tests kiểm thử hợp đồng `IWorldReader`, `IWorldWriter`, `ITerrainGenerator`, `IGameMode` (dùng lớp giả lập `InMemoryFlatWorld` trong file test để kiểm chứng Quy tắc R1, R3 và Observer `BlockChanged`)**
- [x] **Step 2: Chạy test xác nhận FAIL (RED)**
- [x] **Step 3: Cài đặt 4 file interface trong `Game/Assets/_Project/Scripts/Domain/Contracts/` kèm comment OOP đầy đủ**
- [x] **Step 4: Chạy toàn bộ test suite xác nhận PASS (GREEN) và cập nhật `CHANGELOG.md` lên `v0.2.0`**
