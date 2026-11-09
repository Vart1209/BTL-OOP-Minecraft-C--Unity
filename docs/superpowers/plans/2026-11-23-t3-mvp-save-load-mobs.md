# Tuần T3 — Mốc 2: MVP Xong (Save/Load JSON, Config, Mob & AI, Day/Night, GameModes, GameBootstrap DI) Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Hoàn thành trọn bộ Tuần T3 (Mốc 2: MVP Xong) gồm hệ thống Lưu/Tải JSON, Đọc cấu hình `config.json` an toàn bọc try-catch, Phân cấp Mob đa hình & AI cơ bản, Vòng lặp Ngày/Đêm, MobFactory & Spawner (Quy tắc R11), các chế độ chơi `SurvivalMode`/`CreativeMode`/`AdventureMode` (State Pattern), và `GameBootstrap` tiêm phụ thuộc (Dependency Injection) toàn hệ thống.

**Architecture:** Tuân thủ triệt để phân tầng 4 Assembly theo barem [S2]:
- `Minecraft.Domain`: C# thuần 100% (`noEngineReferences: true`), không phụ thuộc `UnityEngine`.
- `Minecraft.Application`: C# thuần 100%, chỉ tham chiếu `Minecraft.Domain`, điều phối các Use Case (UC05, UC06, UC07, UC08, UC11, UC12).
- `Minecraft.Infrastructure`: Tham chiếu `Domain` & `Application`, xử lý file I/O (JSON persistence, config loader) bọc `try-catch` an toàn và tự phục hồi (Tiêu chí 19).
- `Minecraft.Presentation`: Tham chiếu cả 3 tầng, chứa `MonoBehaviour`, `View`, `GameBootstrap` đóng vai trò Composition Root kết nối toàn bộ đối tượng qua constructor injection (không dùng Singleton).

**Tech Stack:** C# 12 / .NET 9, Unity 6 (6000.6.8f1), NUnit 3, System.Text.Json / System.IO, System.Numerics.

**Spec:** `docs/brainstorm.md` (Mục 3.1–3.6, Lộ trình Tuần T3), `AGENTS.md` (Quy tắc OOP, Barem [S2]).

## Global Constraints
- `Minecraft.Domain` và `Minecraft.Application` tuyệt đối KHÔNG `using UnityEngine;`.
- Mọi field phải là `private` (hoặc `protected` hợp lý cho lớp cha trừu tượng), truy cập qua property và kiểm tra hợp lệ dữ liệu đầu vào.
- Tuyệt đối KHÔNG dùng Singleton cho logic nghiệp vụ. Chỉ một `GameBootstrap` (Presentation) khởi tạo và tiêm phụ thuộc (DI) bằng tay qua constructor.
- Mỗi class/struct/interface/enum tách 1 file `.cs` riêng, có XML comment tiếng Việt thuần giải thích rõ nguyên lý OOP và Design Pattern.
- Áp dụng nghiêm ngặt TDD (viết test -> fail -> viết code -> pass).
- Tạo ít nhất 6 commits trên Git, chia nhánh `feature/t3-a-save-load-config` và `feature/t3-b-mobs-daynight`, merge vào `dev`, cập nhật `CHANGELOG.md` lên `v0.5.0` và push lên GitHub.

## Review Focus
1. **Khả năng tự phục hồi khi file cấu hình hoặc save bị hỏng/thiếu:** `ConfigLoader` và `JsonWorldRepository` phải bọc `try-catch`, ghi log cảnh báo và fallback về giá trị mặc định hợp lệ thay vì làm crash ứng dụng (Tiêu chí 19).
2. **Quy tắc R11 (Zombie Spawner):** Zombie chỉ sinh vào ban đêm (`DayNightPhase.Night`), cách người chơi $\ge 24$ block, và không vượt quá giới hạn số lượng (`MaxHostileMobs`).
3. **Quy tắc R12 & Phân quyền GameMode (State Pattern):** `CreativeMode` không mất máu khi bị Mob tấn công, phá được Bedrock, không tốn block khi đặt; `SurvivalMode` tuân thủ đầy đủ quy tắc sinh tồn; `AdventureMode` (bài mở rộng cho vấn đáp) không cho phép phá bất kỳ block nào.
4. **Đa hình và AI Mob (Lớp trừu tượng `Mob` kế thừa `Entity`):** `Zombie` chủ động đuổi người chơi khi trong phạm vi phát hiện ($16$ block), `Cow` hiền lành đi lang thang/chạy trốn, `Skeleton` (bài mở rộng) bắn tên/tấn công tầm xa.
5. **Tính toàn vẹn dữ liệu khi Lưu/Tải (UC05/UC06):** Dữ liệu thế giới (seed, danh sách block bị thay đổi, vị trí người chơi, máu/đói, túi đồ) được serialize/deserialize chính xác không mất mát dữ liệu.

---

## Task Decomposition

### PHẦN 1: PHÂN HỆ WORLD & INFRASTRUCTURE A (`feature/t3-a-save-load-config`)

- [ ] **Task 1.1: Cài đặt cụ thể các GameMode theo State Pattern (Domain)**
  - **Files:**
    - Create: `Game/Assets/_Project/Scripts/Domain/GameModes/SurvivalMode.cs`
    - Create: `Game/Assets/_Project/Scripts/Domain/GameModes/CreativeMode.cs`
    - Create: `Game/Assets/_Project/Scripts/Domain/GameModes/AdventureMode.cs`
    - Test: `Game/Assets/_Project/Tests/EditMode/GameModeTests.cs`
  - **Description:** Thực thi `IGameMode`:
    - `SurvivalMode`: `CanTakeDamage = true`, `ConsumesItemsOnPlace = true`, `InstantBreak = false`, `CanFly = false`, `CanBreakBlock(id)` trả về `false` nếu `id == Bedrock` (Quy tắc R1).
    - `CreativeMode`: `CanTakeDamage = false`, `ConsumesItemsOnPlace = false`, `InstantBreak = true`, `CanFly = true`, `CanBreakBlock(id)` trả về `true` cho mọi block bao gồm Bedrock (Quy tắc R12).
    - `AdventureMode` (Bài mở rộng thi vấn đáp): `CanTakeDamage = true`, `ConsumesItemsOnPlace = true`, `InstantBreak = false`, `CanFly = false`, `CanBreakBlock(id)` luôn trả về `false` (không cho phép phá block).

- [ ] **Task 1.2: Hệ thống cấu hình an toàn `ConfigLoader` (Infrastructure)**
  - **Files:**
    - Create: `Game/Assets/_Project/Scripts/Domain/Contracts/IConfigLoader.cs`
    - Create: `Game/Assets/_Project/Scripts/Infrastructure/Config/GameConfigDto.cs`
    - Create: `Game/Assets/_Project/Scripts/Infrastructure/Config/ConfigLoader.cs`
    - Test: `Game/Assets/_Project/Tests/EditMode/ConfigLoaderTests.cs`
  - **Description:** Đọc cấu hình từ file JSON (khớp `StreamingAssets/config.json`), bọc toàn bộ I/O trong `try-catch`. Nếu file không tồn tại, đường dẫn sai hoặc file JSON hỏng cú pháp, tự động trả về `GameConfigDto.CreateDefault()` và ghi nhận cảnh báo.

- [ ] **Task 1.3: DTO và Hợp đồng Lưu/Tải thế giới `IWorldRepository` (Domain & Infrastructure)**
  - **Files:**
    - Create: `Game/Assets/_Project/Scripts/Domain/Contracts/IWorldRepository.cs`
    - Create: `Game/Assets/_Project/Scripts/Infrastructure/Persistence/WorldSaveDto.cs`
    - Create: `Game/Assets/_Project/Scripts/Infrastructure/Persistence/BlockChangeDto.cs`
    - Create: `Game/Assets/_Project/Scripts/Infrastructure/Persistence/PlayerSaveDto.cs`
    - Create: `Game/Assets/_Project/Scripts/Infrastructure/Persistence/InventorySlotDto.cs`
  - **Description:** Định nghĩa cấu trúc dữ liệu lưu trữ tách biệt hoàn toàn giữa dữ liệu lưu (DTO) và đối tượng nghiệp vụ (Entity/Aggregate).

- [ ] **Task 1.4: Triển khai `JsonWorldRepository` (Infrastructure) & `WorldPersistenceService` (Application)**
  - **Files:**
    - Create: `Game/Assets/_Project/Scripts/Infrastructure/Persistence/JsonWorldRepository.cs`
    - Create: `Game/Assets/_Project/Scripts/Application/World/WorldPersistenceService.cs`
    - Test: `Game/Assets/_Project/Tests/EditMode/PersistenceTests.cs`
  - **Description:** Điều phối Use Case **UC05 (Lưu thế giới)** và **UC06 (Tải thế giới)**:
    - Lưu: lấy seed, các block bị sửa đổi trong `World`, vị trí người chơi, máu, đói, danh sách slot inventory -> serialize thành file JSON có format đẹp.
    - Tải: đọc file JSON (bọc `try-catch`), nếu lỗi trả về `false`, nếu thành công khôi phục lại thế giới, đặt lại các block đã sửa đổi và khôi phục trạng thái người chơi.

- [ ] **Task 1.5: Xác minh & Commit Phân hệ A**
  - Chạy toàn bộ test (`dotnet test`).
  - Commit từng bước và merge `feature/t3-a-save-load-config` vào `dev`.

---

### PHẦN 2: PHÂN HỆ SURVIVAL, MOBS, DAY/NIGHT B (`feature/t3-b-mobs-daynight`)

- [ ] **Task 2.1: Vòng lặp Ngày/Đêm `DayNightCycle` (Domain & Application)**
  - **Files:**
    - Create: `Game/Assets/_Project/Scripts/Domain/Environment/DayNightPhase.cs`
    - Create: `Game/Assets/_Project/Scripts/Domain/Environment/DayNightCycle.cs`
    - Test: `Game/Assets/_Project/Tests/EditMode/DayNightCycleTests.cs`
  - **Description:**
    - Chu kỳ thời gian thuần C# (mặc định 1 ngày trong game = 240 giây hoặc 1200 đơn vị tick).
    - `DayNightPhase`: `Day`, `Sunset`, `Night`, `Sunrise`.
    - Observer Pattern: Cung cấp sự kiện `PhaseChanged` và `TimeUpdated` để UI/Ánh sáng tự cập nhật mà Domain không cần biết Unity.

- [ ] **Task 2.2: Phân cấp Mob đa hình & AI Mob cơ bản (Domain)**
  - **Files:**
    - Create: `Game/Assets/_Project/Scripts/Domain/Entities/Mobs/MobType.cs`
    - Create: `Game/Assets/_Project/Scripts/Domain/Entities/Mobs/MobState.cs`
    - Create: `Game/Assets/_Project/Scripts/Domain/Entities/Mobs/Mob.cs`
    - Create: `Game/Assets/_Project/Scripts/Domain/Entities/Mobs/Zombie.cs`
    - Create: `Game/Assets/_Project/Scripts/Domain/Entities/Mobs/Cow.cs`
    - Create: `Game/Assets/_Project/Scripts/Domain/Entities/Mobs/Skeleton.cs`
    - Test: `Game/Assets/_Project/Tests/EditMode/MobHierarchyTests.cs`
  - **Description:**
    - `Mob : Entity`: Lớp cha trừu tượng cho mọi sinh vật, chứa `MobType`, `MobState` (`Idle`, `Wander`, `Chase`), `MovementSpeed`, `AttackPower`, `DetectionRadius`.
    - `Zombie`: Mob thù địch (`MobType.Zombie`, máu 20, dame 3, bán kính phát hiện 16). Khi phát hiện người chơi, chuyển sang `MobState.Chase` và di chuyển về phía người chơi; nếu khoảng cách <= 1.5 thì tấn công.
    - `Cow`: Mob hiền lành (`MobType.Cow`, máu 10, dame 0). Đi lang thang ngẫu nhiên (`MobState.Wander`), không bao giờ tấn công.
    - `Skeleton`: Mob thù địch tầm xa (bài mở rộng vấn đáp, máu 20, dame 2, giữ khoảng cách và tấn công tầm xa).

- [ ] **Task 2.3: Mob Factory & Spawner (Quy tắc R11) (Domain & Application)**
  - **Files:**
    - Create: `Game/Assets/_Project/Scripts/Domain/Contracts/IMobFactory.cs`
    - Create: `Game/Assets/_Project/Scripts/Domain/Entities/Mobs/MobFactory.cs`
    - Create: `Game/Assets/_Project/Scripts/Application/Survival/MobSpawnerService.cs`
    - Test: `Game/Assets/_Project/Tests/EditMode/MobSpawnerTests.cs`
  - **Description:**
    - `MobFactory` (Factory Pattern): Tạo thực thể `Mob` từ `MobType` và vị trí ban đầu.
    - `MobSpawnerService`: Điều phối **UC12 (Vòng lặp Ngày/Đêm & Sinh Mob)** tuân thủ triệt để **Quy tắc R11**:
      - Chỉ sinh `Zombie` vào ban đêm (`DayNightCycle.CurrentPhase == DayNightPhase.Night`).
      - Vị trí sinh phải cách vị trí người chơi tối thiểu $24$ block (`Vector3.Distance(spawnPos, playerPos) >= 24f`).
      - Kiểm tra giới hạn số lượng tối đa (`activeMobs.Count < maxCapacity`).
      - Tự động dọn dẹp mob đã chết (`!mob.IsAlive`).

- [ ] **Task 2.4: Dịch vụ Chiến đấu & Chuyển đổi GameMode (Application)**
  - **Files:**
    - Create: `Game/Assets/_Project/Scripts/Application/Survival/MobCombatService.cs`
    - Create: `Game/Assets/_Project/Scripts/Application/Survival/GameModeService.cs`
    - Test: `Game/Assets/_Project/Tests/EditMode/MobCombatAndGameModeServiceTests.cs`
  - **Description:**
    - `MobCombatService`: Điều phối **UC08 (Chiến đấu với Mob)**:
      - `TryMobAttackPlayer(Mob mob, Player player)`: Kiểm tra tầm đánh; nếu người chơi ở `CreativeMode` (`IGameMode.CanTakeDamage == false`) thì miễn nhiễm sát thương; nếu ở `SurvivalMode` thì trừ máu người chơi. Nếu máu về 0, kích hoạt chết & hồi sinh theo Quy tắc R9.
      - `TryPlayerAttackMob(Player player, Mob targetMob, int weaponDamage)`: Người chơi đánh Mob, trừ máu Mob, nếu máu <= 0 Mob chết.
    - `GameModeService`: Điều phối **UC11 (Đổi GameMode)**, chuyển đổi giữa Survival, Creative, Adventure an toàn.

- [ ] **Task 2.5: Presentation Views & GameBootstrap Dependency Injection (Presentation)**
  - **Files:**
    - Create: `Game/Assets/_Project/Scripts/Presentation/Environment/DayNightView.cs`
    - Create: `Game/Assets/_Project/Scripts/Presentation/Mobs/MobView.cs`
    - Create: `Game/Assets/_Project/Scripts/Presentation/Bootstrap/GameBootstrap.cs`
  - **Description:**
    - `DayNightView`: Cập nhật Directional Light và Skybox theo `DayNightCycle`.
    - `MobView`: Đồng bộ vị trí `Mob.Position` và animation đơn giản.
    - `GameBootstrap`: Composition Root kết nối toàn bộ Domain, Application, Infrastructure và Presentation thông qua Constructor Injection, hoàn toàn không sử dụng Singleton.

- [ ] **Task 2.6: Xác minh & Commit Phân hệ B**
  - Chạy toàn bộ test suite (`dotnet test`).
  - Commit từng bước và merge `feature/t3-b-mobs-daynight` vào `dev`.

---

### PHẦN 3: TỔNG HỢP, TÀI LIỆU & ĐỒNG BỘ MỐC 2 (v0.5.0)

- [ ] **Task 3.1: Kiểm thử tích hợp toàn diện**
  - Đảm bảo tất cả các test suite từ T0, T1, T2 và T3 đều chạy PASS 100% trên `dotnet test`.
- [ ] **Task 3.2: Cập nhật `CHANGELOG.md` lên `v0.5.0`**
  - Ghi nhận đầy đủ các mục Added, Changed, Removed, Giải thích OOP / AI phục vụ vấn đáp.
- [ ] **Task 3.3: Push lên GitHub**
  - `git push origin dev` cùng các nhánh feature `feature/t3-a-save-load-config` và `feature/t3-b-mobs-daynight`.
