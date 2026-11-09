# Week T2 Implementation Plan: Sandbox Playable Milestone (Mốc 1)

> **For agentic workers:** REQUIRED SUB-SKILL: Use test-driven-development (Red -> Green -> Refactor) and commit in separate git branches (`feature/t2-a-raycast-world-interaction` and `feature/t2-b-survival-crafting-ui`) before merging to `dev`.

**Goal:** Hoàn thiện Mốc 1 – Sandbox chơi được: Thuật toán dò tia Voxel DDA 3D (R7), Dịch vụ Phá/Đặt khối (R1, R2, R3, R5, R12) và Quản lý sinh/gỡ Chunk (UC02, UC03, UC04) cho Phân hệ A; Hệ thống Máu & Đói (R8, R9), Thực thể Player (Kế thừa & Hợp thành), Hệ thống Chế tạo (Recipe, RecipeBook, CraftingService R10), Dịch vụ Sinh tồn (UC09, UC10) và Presentation (WorldView, PlayerInteractionController, SandboxHudView) cho Phân hệ B.

**Architecture:**
- `Minecraft.Domain`: 100% C# thuần (`noEngineReferences: true`), không `using UnityEngine;`. Chứa `VoxelRaycastHit`, `VoxelRaycaster`, `Health`, `Hunger`, `Entity`, `Player`, `RecipeIngredient`, `Recipe`, `RecipeBook`.
- `Minecraft.Application`: 100% C# thuần, chỉ phụ thuộc `Domain`. Chứa các Service điều phối Use Case: `BlockInteractionService`, `ChunkLoadService`, `CraftingService`, `SurvivalService`.
- `Minecraft.Presentation`: MonoBehaviour kết nối Domain/Application với Unity (`WorldView`, `PlayerInteractionController`, `SandboxHudView`).

**Tech Stack:** C# (.NET Standard 2.1 / .NET 9), Unity 6 (`6000.6.8f1`), NUnit 3.

**Spec:** `docs/brainstorm.md`

## Global Constraints

- Không `using UnityEngine;` trong bất kỳ file nào thuộc tầng Domain và Application.
- Mọi field trong Domain và Application phải là `private` (hoặc `protected` khi cần cho lớp cha trừu tượng), truy cập qua Property.
- Mọi phương thức phải kiểm tra tính hợp lệ của tham số đầu vào.
- Mỗi class/struct/enum/interface tách 1 file `.cs` riêng.
- Có XML comment tiếng Việt giải thích rõ các nguyên lý OOP phục vụ vấn đáp.

---

## Phase 1: Phân hệ World & Application A (`feature/t2-a-raycast-world-interaction`)

### Task 1.1: Thuật toán Dò tia Voxel DDA (`VoxelRaycastHit`, `VoxelRaycaster`)
- **Files to create:**
  - `Game/Assets/_Project/Scripts/Domain/World/VoxelRaycastHit.cs`
  - `Game/Assets/_Project/Scripts/Domain/World/VoxelRaycaster.cs`
- **Tests to write:**
  - `Game/Assets/_Project/Tests/EditMode/VoxelInteractionTests.cs` (phần Raycaster)
- **Steps:**
  1. Viết failing tests kiểm tra DDA raycast: bắn trúng khối trong tầm 5 block (Quy tắc R7), trượt khi ngoài 5 block, xác định đúng `BlockPosition`, `AdjacentPosition`, `HitFace`, `Distance`.
  2. Chạy test xác nhận FAIL (RED).
  3. Cài đặt `VoxelRaycastHit.cs` và `VoxelRaycaster.cs`.
  4. Chạy test xác nhận PASS (GREEN).
  5. Commit `feat(world): add 3d dda voxel raycaster and raycasthit struct`.

### Task 1.2: Dịch vụ Tương tác Khối & Quản lý Tải Chunk (`BlockInteractionService`, `ChunkLoadService`)
- **Files to create:**
  - `Game/Assets/_Project/Scripts/Application/World/BlockInteractionService.cs`
  - `Game/Assets/_Project/Scripts/Application/World/ChunkLoadService.cs`
- **Tests to add:**
  - Kiểm tra `BlockInteractionService.TryBreakBlock` (chặn phá Bedrock ở Survival R1, cho phép ở Creative R12, phá Stone rơi Cobblestone vào Inventory R5).
  - Kiểm tra `BlockInteractionService.TryPlaceBlock` (chặn đặt đè lên chân/đầu người chơi R2, trừ item ở Survival, không trừ ở Creative R12).
  - Kiểm tra `ChunkLoadService.UpdateActiveChunks` (sinh và gỡ chunk theo bán kính quanh người chơi).
- **Steps:**
  1. Viết failing tests.
  2. Chạy test xác nhận FAIL (RED).
  3. Cài đặt `BlockInteractionService.cs` và `ChunkLoadService.cs`.
  4. Chạy test xác nhận PASS (GREEN).
  5. Commit `feat(application): add block interaction and chunk load services`.
  6. Merge nhánh `feature/t2-a-raycast-world-interaction` vào `dev`.

---

## Phase 2: Phân hệ Survival, Crafting & Presentation B (`feature/t2-b-survival-crafting-ui`)

### Task 2.1: Hệ thống Máu, Đói & Thực thể Người chơi (`Health`, `Hunger`, `Entity`, `Player`)
- **Files to create:**
  - `Game/Assets/_Project/Scripts/Domain/Entities/Health.cs`
  - `Game/Assets/_Project/Scripts/Domain/Entities/Hunger.cs`
  - `Game/Assets/_Project/Scripts/Domain/Entities/Entity.cs`
  - `Game/Assets/_Project/Scripts/Domain/Entities/Player.cs`
- **Tests to write:**
  - `Game/Assets/_Project/Tests/EditMode/SurvivalAndCraftingTests.cs` (phần Health, Hunger, Player)
- **Steps:**
  1. Viết failing tests kiểm tra `Health` (sát thương, miễn sát thương Creative R12, sự kiện Died R9), `Hunger` (ăn hồi đói, đói = 0 trừ máu và đói >= 18 hồi máu R8), `Player` (kế thừa `Entity`, hợp thành `Health`/`Hunger`/`Inventory`, đổi GameMode, `Respawn` về `SpawnPoint`).
  2. Chạy test xác nhận FAIL (RED).
  3. Cài đặt 4 files trên trong `Domain/Entities/`.
  4. Chạy test xác nhận PASS (GREEN).
  5. Commit `feat(survival): add health, hunger and player entity`.

### Task 2.2: Hệ thống Chế tạo & Dịch vụ Sinh tồn (`RecipeIngredient`, `Recipe`, `RecipeBook`, `CraftingService`, `SurvivalService`)
- **Files to create:**
  - `Game/Assets/_Project/Scripts/Domain/Crafting/RecipeIngredient.cs`
  - `Game/Assets/_Project/Scripts/Domain/Crafting/Recipe.cs`
  - `Game/Assets/_Project/Scripts/Domain/Crafting/RecipeBook.cs`
  - `Game/Assets/_Project/Scripts/Application/Survival/CraftingService.cs`
  - `Game/Assets/_Project/Scripts/Application/Survival/SurvivalService.cs`
- **Tests to add:**
  - Kiểm tra `CraftingService.TryCraft` (thành công 1 Wood -> 4 Planks, thất bại khi thiếu nguyên liệu bảo toàn vật phẩm theo Quy tắc R10).
  - Kiểm tra `SurvivalService.TryEatSelectedFood` (ăn táo tăng điểm đói và trừ 1 táo).
- **Steps:**
  1. Viết failing tests.
  2. Chạy test xác nhận FAIL (RED).
  3. Cài đặt 5 files trên.
  4. Chạy test xác nhận PASS (GREEN).
  5. Commit `feat(crafting): add recipe book, crafting service and survival service`.

### Task 2.3: Lớp Trình diễn & Giao diện Sandbox (`WorldView`, `PlayerInteractionController`, `SandboxHudView`)
- **Files to create:**
  - `Game/Assets/_Project/Scripts/Presentation/World/WorldView.cs`
  - `Game/Assets/_Project/Scripts/Presentation/Player/PlayerInteractionController.cs`
  - `Game/Assets/_Project/Scripts/Presentation/UI/SandboxHudView.cs`
- **Steps:**
  1. Cài đặt `WorldView.cs` (lắng nghe `World.BlockChanged`, gọi `ChunkMeshBuilder` và cập nhật các `ChunkView` kề).
  2. Cài đặt `PlayerInteractionController.cs` (chuột trái phá block, chuột phải đặt block / ăn thức ăn).
  3. Cài đặt `SandboxHudView.cs` (kết nối Observer `HealthChanged`, `HungerChanged`, `InventoryChanged` vẽ HUD Máu, Đói, Hotbar và phím chế tạo).
  4. Commit `feat(presentation): add world view, player interaction controller and sandbox hud view`.
  5. Merge nhánh `feature/t2-b-survival-crafting-ui` vào `dev`.

---

## Phase 3: Hoàn tất, Cập nhật CHANGELOG & Đẩy mã nguồn
- Chạy toàn bộ test suite (EditMode NUnit tests) đảm bảo 100% test cases (≥ 15 tests mới) đều PASS.
- Cập nhật `CHANGELOG.md` lên phiên bản `v0.4.0` (giữ nguyên tiêu đề `## [v0.3.1]` và các phiên bản trước).
- Push cả nhánh `dev` và các nhánh tính năng `feature/*` lên `origin`.
