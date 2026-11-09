# Brainstorm – Game voxel kiểu Minecraft (BTL Lập trình hướng đối tượng)

> Tổng hợp từ buổi brainstorm ngày 07/11/2026. Đây là tài liệu kế hoạch, chưa phải code.
> Nguồn đề: `Barem cham diem/Checklist_BTL_LTHDT_v2.xlsx`, sheet **"Nhóm 02 người"** (ký hiệu [S2]).

## 0. Các quyết định đã chốt

| Chủ đề | Quyết định |
|---|---|
| Nhóm | 2 người, áp dụng barem [S2] |
| Công nghệ | Unity + C#; asset ngoài và AI được phép nếu giải thích được |
| Deadline | **16/12/2026**, nộp qua GitHub (dự kiến nộp sớm nhất 15/12) |
| Build | Cả **macOS và Windows** |
| Báo cáo | Có, không giới hạn trang, viết sau khi xong sản phẩm; **có Use Case Diagram** |
| Mốc kiểm tra của GV | Không có, nhóm tự đặt mốc |
| Phạm vi | **Phương án A – Survival lõi** + Creative (làm sau khi Survival chạy) |
| Kiến trúc | **Phương án B – 3 tầng**, mỗi tầng 1 asmdef, Domain là C# thuần |
| Đồ họa | Texture import sẵn trong Editor (MVP) → đọc resource pack lúc chạy (Nên có) → URP post-processing |
| Phiên bản Unity | **Unity 6 (`6000.6.8f1` hoặc bản mới nhất thống nhất)**, cả 2 máy dùng đúng một bản |
| Ghi nhật ký | Mọi thay đổi ghi vào `CHANGELOG.md` theo quy tắc trong `AGENTS.md` |

**Còn mở:** cách hiểu "phân quyền tác nhân" ([S2] dòng 4). Đề xuất: phân quyền theo GameMode (Survival / Creative). Nên hỏi lại GV.

---

## 1. Tiêu chí của đề ([S2])

Thang điểm: 20 tiêu chí × 1 điểm → /20 → /10. Mỗi tiêu chí chấm A=1, B=0.75, C=0.5, D=0.25, F=0. Mô tả từng mức: không công bố.

| # | Nhóm | Yêu cầu | Mức tối thiểu [S2] |
|---|---|---|---|
| 1 | Phân tích & Code | Use Case khớp hàm điều khiển trong code | ≥8, chia đều 2 người |
| 2 | | Lớp thực thể dùng chung, giải thích được thuộc tính | ≥15 |
| 3 | | Quy tắc nghiệp vụ cài trong phương thức | ≥8, ở cả 2 phân hệ |
| 4 | | Tách lớp giao diện / nghiệp vụ, phân quyền tác nhân | ✓ |
| 5 | Thiết kế & Code | UML Class Diagram tổng thể khớp code | ≥16 lớp |
| 6 | | Đủ quan hệ: kế thừa, thực thi, kết tập, hợp thành, phụ thuộc (cả giữa 2 phân hệ) | ✓ |
| 7 | | Sơ đồ tuần tự/hoạt động khớp thứ tự gọi hàm | ≥4, mỗi người 2 |
| 8 | | Kiến trúc phân tầng khớp package và chiều gọi | ≥3 tầng |
| 9 | Design Pattern & Code | Chọn và giải thích lý do | ≥3, mỗi người 1–2 |
| 10 | | Vai trò lớp trong pattern ↔ code | cả 2 người |
| 11 | | Cơ chế phối hợp (đa hình, ủy quyền, khởi tạo) | xuyên suốt |
| 12 | | Đánh giá pattern + mở rộng chức năng/lớp mới ngay trên code | cả 2 người |
| 13 | Nguyên lý OOP | Đóng gói, trừu tượng, private, kiểm tra dữ liệu hợp lệ | toàn dự án |
| 14 | | Kế thừa, đa hình, override, lớp con thay được lớp cha | ✓ |
| 15 | | Một lớp một nhiệm vụ, interface gọn, coding convention thống nhất | ✓ |
| 16 | | Ghép nối 2 người qua interface, dễ mở rộng / đổi cài đặt | ✓ |
| 17 | Đóng gói | Build chạy được trên máy khác, đường dẫn tương đối | ✓ |
| 18 | | Dữ liệu mẫu + test case | ≥20, chia đều |
| 19 | | try-catch thống nhất, file cấu hình riêng, Git có nhánh | ≥20 commit, chia đều |
| 20 | | Báo cáo, README, kịch bản demo, thuyết trình, hiểu code của nhau | ✓ |

---

## 2. Phạm vi (Phương án A)

| Tầng | Tính năng |
|---|---|
| **MVP** | Sinh địa hình theo chunk (seed); đi/nhảy/nhìn; đặt/phá block; inventory + hotbar; máu và đói; chế tạo theo danh sách công thức; 1 mob thù địch (Zombie) + 1 mob hiền (Cow); ngày/đêm; lưu/tải thế giới; chế độ Creative |
| **Nên có** | Công cụ hợp block thì đào nhanh hơn; chết rơi đồ và hồi sinh; đổi resource pack trong menu; hậu kỳ URP (bloom, sương mù, chỉnh màu) |
| **Nếu còn thời gian** | Hang động, nhiều loại cây, âm thanh, lá cây đung đưa, biome |

---

## 3. Thiết kế OOP

### 3.1 Hai phân hệ

| | Phân hệ World (Thành viên A) | Phân hệ Survival (Thành viên B) |
|---|---|---|
| Domain | `BlockType`, `BlockRegistry`, `Chunk`, `World`, `BlockPos`, `ChunkCoord`, `ITerrainGenerator` → `PerlinTerrainGenerator` / `FlatTerrainGenerator`, `ChunkMeshBuilder`, `VoxelRaycaster` | `Entity` (abstract) → `Player`, `Mob` (abstract) → `Zombie` / `Cow`; `Health`, `Hunger`; `Item` (abstract) → `BlockItem` / `ToolItem` / `FoodItem`; `ItemStack`, `Inventory`, `Recipe`, `RecipeBook`, `IGameMode` → `SurvivalMode` / `CreativeMode`; `DayNightCycle`, `MobFactory`, `MobSpawner` |
| Application | `BlockInteractionService`, `SaveGameService` | `CraftingService`, `SurvivalService` |
| Infrastructure | `IWorldRepository` → `JsonWorldRepository`, `ConfigLoader`, `ITextureAtlasProvider` → `BuiltInAtlasProvider` / `FolderResourcePackProvider` | – |

**Hợp đồng giữa 2 phân hệ** (viết chung trong tuần T0, chỉ đổi khi cả hai đồng ý): `IWorldReader.GetBlock`, `IWorldWriter.SetBlock`, event `World.BlockChanged`, bảng ID block/item. B có thể làm song song bằng một thế giới phẳng giả.

### 3.2 Khái niệm OOP → vị trí trong code

| Khái niệm | Vị trí |
|---|---|
| Đóng gói | `Chunk` giữ mảng block `private`, `SetBlock` kiểm tra biên; `ItemStack.Count` `private set`, `Add` chặn >64; `Health` giới hạn 0..Max |
| Trừu tượng | `Entity`, `Item` abstract; `IWorldReader`, `ITerrainGenerator` |
| Kế thừa | `Zombie : Mob : Entity`; `FoodItem : Item` |
| Đa hình | `Item.Use(ctx)` override; `Mob.Tick()` override |
| Thay thế được lớp cha | Mọi `Item` con dùng được trong `ItemStack`; test dùng `FlatTerrainGenerator` |
| Một lớp một nhiệm vụ | `Chunk` (dữ liệu) / `ChunkMeshBuilder` (dựng mesh) / `ChunkView` (hiển thị) |
| Interface gọn | `IWorldReader` tách khỏi `IWorldWriter` |
| Phụ thuộc qua interface | Service nhận `IWorldRepository` qua constructor; đổi JSON → Binary chỉ thêm lớp |

### 3.3 Năm loại quan hệ

| Quan hệ | Ví dụ |
|---|---|
| Kế thừa | `Zombie → Mob → Entity` |
| Thực thi | `World ⇢ IWorldReader, IWorldWriter` |
| Hợp thành | `World ◆ Chunk`; `Player ◆ Inventory, Health, Hunger` |
| Kết tập | `ItemStack ◇ Item`; `RecipeBook ◇ Recipe` |
| Phụ thuộc | `CraftingService ┄> Inventory`; `BlockInteractionService ┄> IWorldWriter` |

### 3.4 Design Pattern

| Pattern | Người | Vị trí | Lý do |
|---|---|---|---|
| Strategy | A | `ITerrainGenerator`, `ITextureAtlasProvider` | Đổi thuật toán địa hình / nguồn texture không sửa `World` |
| Observer (C# event) | A | `World.BlockChanged`, `Inventory.Changed`, `Health.Changed` | Domain không biết UI; View tự cập nhật |
| Factory | B | `MobFactory`, `ItemFactory` | Tạo đối tượng từ ID khi spawn / tải save |
| State | B | `IGameMode`; AI mob (Idle → Wander → Chase) | Quyền theo gamemode gom một chỗ (phân quyền tác nhân) |

Không dùng Singleton: chỉ một `GameBootstrap` khởi tạo và nối các đối tượng bằng tay.

**Bài mở rộng luyện cho vấn đáp (dòng 12):** thêm block mới; thêm mob `Skeleton`; thêm gamemode `Adventure` (không được phá block).

### 3.5 Use Case và quy tắc nghiệp vụ

- **Tác nhân:** Người chơi Survival, Người chơi Creative (quyền mở rộng), Hệ thống (ngày/đêm, spawn mob).
- **UC của A:** Tạo thế giới (seed), Sinh và khám phá chunk, Phá block, Đặt block, Lưu/Tải thế giới, Đổi resource pack.
- **UC của B:** Di chuyển, Quản lý inventory/hotbar, Chế tạo, Ăn, Chiến đấu với mob, Đổi gamemode.

| Phân hệ | Quy tắc |
|---|---|
| World | R1 Không phá được bedrock · R2 Không đặt block vào ô đã có block hoặc có entity · R3 Tọa độ Y ngoài giới hạn bị từ chối · R4 Thời gian đào = độ cứng × hệ số công cụ · R5 Phá block rơi đúng item |
| Survival | R6 Stack tối đa 64, công cụ tối đa 1 · R7 Tầm với ≤5 block · R8 Đói = 0 thì mất máu, đói ≥18 thì hồi máu · R9 Máu = 0 thì chết và hồi sinh · R10 Chế tạo "đủ hết hoặc không làm gì" · R11 Zombie chỉ spawn ban đêm, cách người chơi ≥24 block, có giới hạn số lượng · R12 Creative: không mất máu, không tốn block, phá ngay, bay được |

### 3.6 Sơ đồ tuần tự

- **A:** Phá block; Sinh chunk khi di chuyển.
- **B:** Chế tạo; Zombie tấn công → chết → hồi sinh.

---

## 4. Kiến trúc Unity

Repo gốc chứa tài liệu; project Unity nằm trong thư mục con `Game/`.

```
MINECRAFT/                      (gốc repo)
  AGENTS.md  CHANGELOG.md  README.md
  docs/  Barem cham diem/
  Game/                         (project Unity)
    Assets/_Project/
      Scripts/
        Domain/          *.Domain.asmdef  (No Engine References = true)
          World/ Blocks/ Entities/ Items/ Crafting/ GameModes/
        Application/     → chỉ tham chiếu Domain
        Infrastructure/  → Domain, Application (JSON, config, resource pack)
        Presentation/    → tất cả (MonoBehaviour: Bootstrap/ Player/ World/ Mobs/ UI/)
      Data/              ScriptableObject: Blocks/ Items/ Recipes/
      Art/ Prefabs/
      Scenes/            MainMenu, Game, Sandbox_A, Sandbox_B
      Tests/EditMode/    *.Domain.Tests.asmdef (NUnit)
    Assets/StreamingAssets/   config.json, ResourcePacks/
```

- Domain bật *No Engine References*: trình biên dịch chặn Domain gọi `UnityEngine`. Toán vector dùng `System.Numerics` + struct `BlockPos` tự viết.
- ScriptableObject block/item ở Presentation, lúc khởi động chuyển thành `BlockType` thuần C#.
- Cấu hình đọc từ `Application.streamingAssetsPath`, file save ghi vào `Application.persistentDataPath`; mọi I/O bọc `try-catch`.
- Chunk 16×16×64, bắt buộc face culling; block đổi thì chỉ dựng lại chunk đó (+ chunk hàng xóm nếu ở mép). Không dùng greedy meshing.
- `ChunkMeshBuilder` trả dữ liệu thuần (test được, chạy luồng phụ được); tạo `Mesh` ở main thread, giới hạn N chunk/frame.
- `MeshCollider` chỉ dựng lại cho chunk vừa sửa; atlas dùng point filter + padding hoặc `Texture2DArray`; render distance 4–6 chunk trong `config.json`.
- Mob không dùng NavMesh: đi thẳng về phía người chơi, gặp vật cản thì nhảy.
- `VoxelRaycaster` (thuật toán DDA) ở Domain để chọn block, có test.
- Shader pack Minecraft (GLSL) không dùng được trong Unity → làm hiệu ứng bằng URP post-processing / Shader Graph.
- Resource pack: chỉ hỗ trợ PNG cùng kích thước, texture động lấy khung đầu, không hỗ trợ CTM. Đọc license từng pack và ghi credit trong README.

---

## 5. Phân công và Git

### 5.1 Phần mỗi người
Mỗi người: 1 phân hệ, khoảng 6 Use Case, 2 pattern, 2 sơ đồ tuần tự, ≥10 test, ≥10 commit. Mọi PR do người kia review.

### 5.2 Thiết lập một lần
1. Cùng một bản Unity (`6000.6.8f1` hoặc bản mới nhất thống nhất giữa 2 máy); commit `Game/ProjectSettings/ProjectVersion.txt`.
2. `.gitignore` theo template Unity của GitHub (đặt trong `Game/`) + `.gitignore` gốc cho `.DS_Store`.
3. *Project Settings → Editor*: Asset Serialization = **Force Text**, Version Control = **Visible Meta Files**. Luôn commit `.meta` kèm asset.
4. Chưa cần Git LFS (texture 16×16 rất nhẹ); GitHub chặn file >100MB.
5. Mỗi người `git config user.name` / `user.email` đúng tài khoản GitHub của mình.

### 5.3 Nhánh
```
main      ← chỉ merge ở các mốc, luôn chạy được
dev       ← nhánh tích hợp
feature/a-chunk-mesh, feature/b-inventory ...
```
- PR vào `dev`, người kia review rồi mới merge.
- Đầu buổi luôn `git pull`; không `force push` lên `main`/`dev`.
- Commit nhỏ, ít nhất 1 commit/ngày, message dạng `feat(world): face culling`.
- Mỗi người scene riêng (`Sandbox_A`, `Sandbox_B`); `Game.unity` mỗi lần một người sửa; đối tượng nào cũng làm prefab.

### 5.4 Quy ước code
`PascalCase` cho class/method/property; `_camelCase` cho field private; interface tiền tố `I`; mỗi file một class; namespace theo thư mục. Dùng chung `.editorconfig`.

---

## 6. Lộ trình (07/11 → 16/12/2026)

| Tuần | A (World) | B (Survival) | Mốc (Chủ nhật) |
|---|---|---|---|
| T0 7–8/11 | Repo, asmdef, khung thư mục | Interface chung, quy ước code, nháp Use Case Diagram | Clone về máy kia mở được |
| T1 9–15/11 | `Chunk`, `World`, sinh địa hình, mesh có face culling | Di chuyển, camera, `Item`/`ItemStack`/`Inventory` + test, hotbar | Đi được trên địa hình |
| T2 16–22/11 | Raycaster, đặt/phá block, tải/hủy chunk | Máu, đói, thức ăn, công cụ, chế tạo + UI | **Mốc 1 – Sandbox chơi được** |
| T3 23–29/11 | Lưu/tải JSON, `config.json`, try-catch | Mob, Factory, Spawner, ngày/đêm, chết/hồi sinh; **cả hai làm GameMode** | **Mốc 2 – MVP xong**, build thử Mac + Windows |
| T4 30/11–6/12 | Đọc resource pack lúc chạy | Công cụ đào nhanh, rơi đồ, hiệu ứng URP | Dừng thêm tính năng, đủ ≥20 test |
| T5 7–13/12 | Sửa lỗi, build cuối, README | Báo cáo, UML, kịch bản demo | Luyện vấn đáp chéo + 3 bài mở rộng |
| 14–16/12 | Dự phòng, nộp muộn nhất 15/12 — **v1.0.0** | | |

---

## 7. Rủi ro

| Rủi ro | Cách phòng |
|---|---|
| Giật do mesh chunk | Face culling từ đầu, chunk cao 64, render distance 4; đo trên máy yếu nhất |
| 3 tầng làm chậm | Application mỏng; chỉ tạo interface ở chỗ barem cần |
| Mới dùng Git, xung đột | Scene sandbox riêng, prefab, PR nhỏ; tập merge thử ở T0 |
| Build Mac/Win lệch | Build thử từ T3; app Mac chưa ký mở bằng chuột phải → Open (ghi README) |
| Resource pack mod không đọc được | Chỉ hỗ trợ PNG cùng kích thước, bỏ CTM / texture động |
| Mải làm đồ họa | Chỉ làm đồ họa sau Mốc 2 |
| Dùng code AI không giải thích được | Merge code thì phải giải thích được khi review PR; ghi "AI đã làm" trong CHANGELOG |

**Thứ tự cắt khi trễ:** đọc resource pack lúc chạy → Cow → chế tạo dạng danh sách đơn giản → rơi đồ khi chết.
**Không cắt:** lưu/tải, test, GameMode, 3 pattern.

---

## 8. Ánh xạ tiêu chí → phần việc

| # | Đáp ứng bởi |
|---|---|
| 1 | 12 Use Case ↔ hàm public của `BlockInteractionService`, `CraftingService`, `SurvivalService` |
| 2 | ~21 lớp domain mục 3.1 |
| 3 | R1–R12 trong `World.SetBlock`, `ItemStack.Add`, `Hunger.Tick`, `CraftingService.Craft`… |
| 4 | Presentation tách Application/Domain; quyền theo `IGameMode` |
| 5–6 | UML xuất từ code, đủ 5 quan hệ mục 3.3 |
| 7 | 4 sơ đồ tuần tự mục 3.6 |
| 8 | 4 asmdef, Domain *No Engine References* |
| 9–11 | Strategy, Observer, Factory, State |
| 12 | Block mới, Skeleton, Adventure mode |
| 13–16 | Bảng mục 3.2 |
| 17 | Build Mac + Windows, `streamingAssetsPath` / `persistentDataPath` |
| 18 | ≥20 test EditMode (Inventory, Crafting, Chunk, Raycaster, Health, GameMode…) |
| 19 | try-catch ở Infrastructure, `config.json`, nhánh `feature/*`, ≥20 commit |
| 20 | README, báo cáo, kịch bản demo, review PR chéo, `CHANGELOG.md` |
