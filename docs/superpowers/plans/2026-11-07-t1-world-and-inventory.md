# Week T1 Implementation Plan: World & Inventory (Phân hệ A & B)

> **For agentic workers:** REQUIRED SUB-SKILL: Use test-driven-development (Red -> Green -> Refactor) and commit in separate git branches (`feature/t1-a-world-chunk` and `feature/t1-b-items-inventory`) before merging to `dev`.

**Goal:** Triển khai toàn bộ Phân hệ A (Block, Chunk, Terrain Generator, World, ChunkMeshBuilder with Face Culling) và Phân hệ B (Item hierarchy, ItemStack, Inventory, Hotbar selection) cùng các lớp Presentation cơ bản (`ChunkView`, `FirstPersonPlayerController`), bám sát barem [S2] và quy tắc OOP.

**Architecture:**
- `Minecraft.Domain`: 100% C# thuần (`noEngineReferences: true`), không `using UnityEngine;`. Tọa độ và vector 3D sử dụng `BlockPos`, `ChunkCoord` và `System.Numerics.Vector3`.
- `Minecraft.Presentation`: Chứa MonoBehaviour kết nối Domain với Unity (`ChunkView` tạo `Mesh` từ `ChunkMeshData`, `FirstPersonPlayerController` điều khiển camera/nhân vật).
- Mẫu thiết kế áp dụng: **Strategy Pattern** (`ITerrainGenerator`), **Observer Pattern** (`IWorldReader.BlockChanged`, `Inventory.InventoryChanged`), **Polymorphism & LSP** (`Item -> BlockItem, ToolItem, FoodItem`), **Aggregation** (`ItemStack ◇ Item`, `Inventory ◇ ItemStack`).

**Tech Stack:** C# (.NET Standard 2.1 / .NET 9), Unity 6 (`6000.6.8f1`), NUnit 3.

**Spec:** `docs/brainstorm.md`

## Global Constraints

- Không `using UnityEngine;` trong bất kỳ file nào thuộc tầng Domain.
- Mọi field trong Domain phải là `private` (hoặc `protected` khi cần cho lớp cha trừu tượng), truy cập qua Property.
- Mọi phương thức phải kiểm tra tính hợp lệ của tham số đầu vào (ném ngoại lệ `ArgumentOutOfRangeException` / `ArgumentNullException` hoặc trả về `false`).
- Mỗi class/struct/enum/interface tách 1 file `.cs` riêng.
- Có XML comment tiếng Việt giải thích rõ các nguyên lý OOP phục vụ vấn đáp.

---

## Phase 1: Phân hệ World – Thành viên A (`feature/t1-a-world-chunk`)

### Task 1.1: Hệ thống Block (`BlockFace`, `BlockType`, `BlockRegistry`)
- **Files to create:**
  - `Game/Assets/_Project/Scripts/Domain/Blocks/BlockFace.cs`
  - `Game/Assets/_Project/Scripts/Domain/Blocks/BlockType.cs`
  - `Game/Assets/_Project/Scripts/Domain/Blocks/BlockRegistry.cs`
- **Tests to write:**
  - `Game/Assets/_Project/Tests/EditMode/WorldDomainTests.cs` (phần BlockType & BlockRegistry)
- **Steps:**
  1. Viết failing tests kiểm tra tạo `BlockType`, tính đóng gói, `GetTextureIndex` cho 6 mặt, và `BlockRegistry.CreateDefault()` (Quy tắc R5: rơi đồ đúng loại).
  2. Chạy test xác nhận FAIL (RED).
  3. Cài đặt `BlockFace.cs`, `BlockType.cs`, `BlockRegistry.cs`.
  4. Chạy test xác nhận PASS (GREEN).
  5. Commit `feat(world): add block types and default block registry`.

### Task 1.2: Hệ thống Chunk & Thuật toán Sinh địa hình (`Chunk`, `FlatTerrainGenerator`, `PerlinTerrainGenerator`)
- **Files to create:**
  - `Game/Assets/_Project/Scripts/Domain/World/Chunk.cs`
  - `Game/Assets/_Project/Scripts/Domain/World/FlatTerrainGenerator.cs`
  - `Game/Assets/_Project/Scripts/Domain/World/PerlinTerrainGenerator.cs`
- **Tests to add:**
  - Kiểm tra `Chunk` (kích thước 16x64x16, mảng 1D tính chỉ số, kiểm tra biên, cờ `IsDirty`).
  - Kiểm tra `FlatTerrainGenerator` (Bedrock -> Dirt -> Grass -> Air).
  - Kiểm tra `PerlinTerrainGenerator` (thuần C#, tất định cùng seed, dải chiều cao 16..40).
- **Steps:**
  1. Viết failing tests.
  2. Chạy test xác nhận FAIL (RED).
  3. Cài đặt `Chunk.cs`, `FlatTerrainGenerator.cs`, `PerlinTerrainGenerator.cs`.
  4. Chạy test xác nhận PASS (GREEN).
  5. Commit `feat(world): add chunk data structure and terrain generators`.

### Task 1.3: Quản lý Thế giới Voxel & Dựng Lưới Mesh Face Culling (`World`, `ChunkMeshData`, `ChunkMeshBuilder`)
- **Files to create:**
  - `Game/Assets/_Project/Scripts/Domain/World/World.cs`
  - `Game/Assets/_Project/Scripts/Domain/World/ChunkMeshData.cs`
  - `Game/Assets/_Project/Scripts/Domain/World/ChunkMeshBuilder.cs`
- **Tests to add:**
  - Kiểm tra `World` (GenerateChunk, SetBlock, GetBlock qua biên âm/dương, Quy tắc R3, Observer `BlockChanged`).
  - Kiểm tra `ChunkMeshBuilder` Face Culling: 1 block = 6 mặt (24 đỉnh / 36 indices); 2 block kề nhau = 10 mặt (40 đỉnh / 60 indices).
- **Steps:**
  1. Viết failing tests.
  2. Chạy test xác nhận FAIL (RED).
  3. Cài đặt `World.cs`, `ChunkMeshData.cs`, `ChunkMeshBuilder.cs`.
  4. Chạy test xác nhận PASS (GREEN).
  5. Commit `feat(world): add voxel world manager and mesh builder with face culling`.
  6. Merge nhánh `feature/t1-a-world-chunk` vào `dev`.

---

## Phase 2: Phân hệ Survival – Thành viên B (`feature/t1-b-items-inventory`)

### Task 2.1: Cây kế thừa Vật phẩm (`ToolType`, `ItemUseContext`, `Item`, `BlockItem`, `ToolItem`, `FoodItem`, `ItemRegistry`)
- **Files to create:**
  - `Game/Assets/_Project/Scripts/Domain/Items/ToolType.cs`
  - `Game/Assets/_Project/Scripts/Domain/Items/ItemUseContext.cs`
  - `Game/Assets/_Project/Scripts/Domain/Items/Item.cs`
  - `Game/Assets/_Project/Scripts/Domain/Items/BlockItem.cs`
  - `Game/Assets/_Project/Scripts/Domain/Items/ToolItem.cs`
  - `Game/Assets/_Project/Scripts/Domain/Items/FoodItem.cs`
  - `Game/Assets/_Project/Scripts/Domain/Items/ItemRegistry.cs`
- **Tests to write:**
  - `Game/Assets/_Project/Tests/EditMode/InventoryAndItemsTests.cs` (phần Item hierarchy)
- **Steps:**
  1. Viết failing tests kiểm tra đa hình `Item.Use()`, Quy tắc R6 (công cụ stack 1, khối stack 64), Quy tắc R2 (đặt khối), Quy tắc R4 (tính thời gian đào công cụ).
  2. Chạy test xác nhận FAIL (RED).
  3. Cài đặt 7 files trên.
  4. Chạy test xác nhận PASS (GREEN).
  5. Commit `feat(survival): add polymorphic item hierarchy and item registry`.

### Task 2.2: Ngăn xếp vật phẩm & Quản lý Túi đồ (`ItemStack`, `Inventory`)
- **Files to create:**
  - `Game/Assets/_Project/Scripts/Domain/Items/ItemStack.cs`
  - `Game/Assets/_Project/Scripts/Domain/Items/Inventory.cs`
- **Tests to add:**
  - Kiểm tra `ItemStack` (kết tập, `Add` trả phần dư, `Remove` kiểm tra âm).
  - Kiểm tra `Inventory` (36 ô, 9 ô Hotbar, `AddItem` cộng dồn rồi sang ô mới, `RemoveItem` kiểm tra đủ số lượng, `SelectHotbarSlot`, sự kiện `InventoryChanged`).
- **Steps:**
  1. Viết failing tests.
  2. Chạy test xác nhận FAIL (RED).
  3. Cài đặt `ItemStack.cs` và `Inventory.cs`.
  4. Chạy test xác nhận PASS (GREEN).
  5. Commit `feat(survival): add itemstack and inventory with hotbar selection`.

### Task 2.3: Lớp Trình diễn Tầng Presentation (`ChunkView`, `FirstPersonPlayerController`) & Tích hợp
- **Files to create:**
  - `Game/Assets/_Project/Scripts/Presentation/World/ChunkView.cs`
  - `Game/Assets/_Project/Scripts/Presentation/Player/FirstPersonPlayerController.cs`
- **Steps:**
  1. Cài đặt `ChunkView.cs` (chuyển `ChunkMeshData` thành `UnityEngine.Mesh`, gán cho `MeshFilter` & `MeshCollider`).
  2. Cài đặt `FirstPersonPlayerController.cs` (di chuyển, nhảy, bay theo gamemode, cuộn phím Hotbar).
  3. Commit `feat(presentation): add chunk view mesh renderer and first person player controller`.
  4. Merge nhánh `feature/t1-b-items-inventory` vào `dev`.

---

## Phase 3: Hoàn tất, Cập nhật CHANGELOG & Đẩy mã nguồn
- Chạy toàn bộ test suite (EditMode NUnit tests) đảm bảo 100% test cases (≥ 16 tests mới) đều PASS.
- Cập nhật `CHANGELOG.md` lên phiên bản `v0.3.0` ghi nhận toàn bộ tiến độ Tuần T1.
- Push cả nhánh `dev` và các nhánh tính năng `feature/*` lên `origin`.
