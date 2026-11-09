# Minecraft Voxel Survival & Creative – Bài Tập Lớn Lập Trình Hướng Đối Tượng (OOP) `v1.0.0`

Dự án game thế giới Voxel kiểu Minecraft (hỗ trợ đầy đủ 3 chế độ **Survival**, **Creative**, **Adventure**) xây dựng bằng **C# trên Unity 6 (`6000.6.8f1`)**, thiết kế theo kiến trúc phân tầng Clean Architecture 4 Assembly (`Domain` / `Application` / `Infrastructure` / `Presentation`) bám sát 100% **20/20 tiêu chí Barem chấm điểm nhóm 2 người ([S2])**.

---

## 📊 Thống kê Tổng quan Dự án (`v1.0.0`)

| Hạng mục | Chỉ số Đạt được | Yêu cầu Barem [S2] |
| :--- | :--- | :--- |
| **Phiên bản phát hành** | **`v1.0.0`** (Hoàn thành cả tầng MVP + tầng "Nên có") | Hoàn chỉnh, chạy ổn định |
| **Kiến trúc Phân tầng (`.asmdef`)** | **4 tầng độc lập** (`Domain` bật `noEngineReferences: true`) | $\ge 3$ tầng (Tiêu chí 8) |
| **Tổng số file mã nguồn C#** | **88 files `.cs`** (74 files Production + 14 files Unit Test) | Mỗi lớp 1 file `.cs` riêng |
| **Số lượng Use Case** | **12 Use Cases** (6 UC Thành viên A + 6 UC Thành viên B) | $\ge 8$ Use Cases (Tiêu chí 1) |
| **Lớp thực thể / giá trị Domain** | **22 lớp thực thể/giá trị** đóng gói `private` 100% | $\ge 15$ lớp (Tiêu chí 2) |
| **Quy tắc nghiệp vụ** | **12 quy tắc (R1–R12)** trải đều cả 2 phân hệ | $\ge 8$ quy tắc (Tiêu chí 3) |
| **Design Patterns áp dụng** | **5 Patterns**: Strategy, Observer, State, Factory, Repository (0 Singleton) | $\ge 3$ Patterns (Tiêu chí 9–12) |
| **Kiểm thử tự động (NUnit)** | **73 Unit Tests PASS 100%** (chạy độc lập trên .NET 9 & Unity Test Runner) | $\ge 20$ Test Cases (Tiêu chí 18) |

---

## 📚 Hồ sơ Tài liệu, Sơ đồ UML & Cẩm nang Vấn đáp

1. **Kế hoạch & Thiết kế tổng thể:** [`docs/brainstorm.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/docs/brainstorm.md)
2. **Sơ đồ Use Case & Đặc tả 12 Use Case (Tiêu chí 1 & 4):** [`docs/uml/use-case-diagram.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/docs/uml/use-case-diagram.md)
3. **Sơ đồ Lớp UML Tổng thể & 5 Mối Quan hệ OOP (Tiêu chí 2, 5, 6):** [`docs/uml/class-diagram.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/docs/uml/class-diagram.md)
4. **Bộ 4 Sơ đồ Tuần tự — Sequence Diagrams (Tiêu chí 7):** [`docs/uml/sequence-diagrams.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/docs/uml/sequence-diagrams.md)
5. **Cẩm nang Bảo vệ, Ánh xạ 20 Tiêu chí, 3 Bài Mở rộng & 10 Câu hỏi Vấn đáp:** [`docs/defense-guide.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/docs/defense-guide.md)
6. **Nhật ký Phiên bản Tự động (`v0.1.0` $\rightarrow$ `v1.0.0`):** [`CHANGELOG.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/CHANGELOG.md)
7. **Quy tắc Kiến trúc & OOP của Dự án:** [`AGENTS.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/AGENTS.md)
8. **Barem chấm điểm gốc:** [`Barem cham diem/Checklist_BTL_LTHDT_v2.xlsx`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Barem%20cham%20diem/Checklist_BTL_LTHDT_v2.xlsx)

---

## 🚀 Hướng dẫn Mở Dự án & Chơi Ngay Trong 1 Click (Unity 6)

Dự án trang bị sẵn công cụ Editor tự động dựng toàn bộ Scene và kết nối Dependency Injection chỉ với **1 cú nhấp chuột**:

1. Mở **Unity Hub** $\rightarrow$ **Add project from disk** $\rightarrow$ chọn thư mục `MINECRAFT/Game` (sử dụng **Unity 6 `6000.6.8f1`** hoặc bản Unity 6 tương đương).
2. Trên thanh Menu trên cùng của Unity Editor, chọn:
   - **`Minecraft OOP -> 1. Auto-Setup Playable Game Scene`**: Tự động tạo `Directional Light` (`DayNightView`, `AtmosphereFogView`), `WorldRoot` (`WorldView`, `ResourcePackView`), `Player` tại `(0, 42, 0)` (`FirstPersonPlayerController`, `PlayerInteractionController`, `Main Camera`), `HUD` (`SandboxHudView`) và nối toàn bộ tham chiếu vào `GameBootstrap`.
   - *(Tùy chọn)* **`Minecraft OOP -> 2. Generate All 4 Scenes (MainMenu, Game, Sandbox_A, Sandbox_B)`**: Tự động khởi tạo và lưu đủ 4 file `.unity` vào `Assets/_Project/Scenes/`.
3. Nhấn nút **Play (▶)** trong Unity Editor để chơi ngay lập tức!

---

## 🎮 Bảng Phím Tắt Điều Khiển Trong Game

| Phím / Thao tác | Chức năng trong Game | Use Case / Quy tắc Minh chứng |
| :--- | :--- | :--- |
| **`W`, `A`, `S`, `D`** | Di chuyển nhân vật theo hướng nhìn Camera | UC07 — Di chuyển FPS |
| **`Chuột (Mouse X/Y)`** | Xoay góc nhìn thứ nhất 360 độ | UC07 — Điều khiển Camera |
| **`Space`** | Nhảy (ở Survival) / Bay lên cao (ở Creative) | UC07, Quy tắc R12 (`IGameMode.CanFly`) |
| **`Left Shift`** | Hạ độ cao khi đang bay (ở Creative) | UC07, Quy tắc R12 |
| **`Chuột trái (LMB)`** | Phá hủy khối đang nhắm tới (tầm với $\le 5$ block) | UC03, Quy tắc R1, R3, R4, R5, R7 |
| **`Chuột phải (RMB)`** | Ăn thức ăn đang cầm (`Apple`) hoặc Đặt khối vào ô liền kề | UC10 (Ăn), UC04 (Đặt khối), Quy tắc R2, R3 |
| **`Phím số 1 .. 9` / `Con lăn`** | Chọn ô vật phẩm trên thanh Hotbar | UC08 — Quản lý Hotbar |
| **`Phím C` hoặc `Phím E`** | Bật / Tắt bảng **Bàn Chế Tạo (Crafting Menu)** | UC09 — Chế tạo Atomic (Quy tắc R10) |
| **`Phím F1`** | Chuyển đổi xoay vòng chế độ chơi: **Survival $\rightarrow$ Creative $\rightarrow$ Adventure** | UC11 — State Pattern (`IGameMode`) |
| **`Phím F2`** | Đổi nóng Gói Tài nguyên (Resource Pack): **`default (16x)` $\leftrightarrow$ `OptimumRealism_64x (64x)`** | UC06 — Strategy + Observer Pattern |
| **`Phím F5`** | **Lưu nhanh thế giới (Quick Save)** ra `persistentDataPath/quicksave.json` | UC05 — Repository Pattern + `try-catch` |
| **`Phím F9`** | **Tải nhanh thế giới (Quick Load)** từ `persistentDataPath/quicksave.json` | UC05 — Khôi phục Thế giới & Người chơi |

---

## 🗂️ Cấu trúc Thư mục Dự án

```text
MINECRAFT/
├── AGENTS.md                        # Quy tắc kiến trúc OOP & tự động ghi CHANGELOG.md
├── CHANGELOG.md                     # Nhật ký phiên bản tự động (v0.1.0 -> v1.0.0)
├── README.md                        # Tài liệu hướng dẫn cài đặt, điều khiển & tổng quan
├── docs/
│   ├── brainstorm.md                # Tài liệu phân tích & thiết kế ban đầu
│   ├── defense-guide.md             # Cẩm nang bảo vệ 20 tiêu chí, 3 bài mở rộng & câu hỏi vấn đáp
│   ├── superpowers/plans/           # Kế hoạch triển khai chi tiết từng tuần (T0 -> T5)
│   └── uml/
│       ├── use-case-diagram.md      # Sơ đồ Use Case & đặc tả 12 Use Cases (Tiêu chí 1, 4)
│       ├── class-diagram.md         # Sơ đồ lớp 35 classes, 22 thực thể & 5 quan hệ (Tiêu chí 2, 5, 6)
│       └── sequence-diagrams.md     # 4 Sơ đồ tuần tự của A và B (Tiêu chí 7)
└── Game/                            # Thư mục gốc của Unity 6 Project
    └── Assets/
        ├── _Project/
        │   ├── Scripts/
        │   │   ├── Domain/          # Minecraft.Domain.asmdef (C# thuần 100%, noEngineReferences: true)
        │   │   ├── Application/     # Minecraft.Application.asmdef (11 Services điều phối 12 Use Cases)
        │   │   ├── Infrastructure/  # Minecraft.Infrastructure.asmdef (JSON Save/Load, Config, ResourcePack)
        │   │   └── Presentation/    # Minecraft.Presentation.asmdef (Views, Controllers, GameBootstrap, EditorSceneSetup)
        │   ├── Art/                 # Materials & Textures mặc định
        │   ├── Scenes/              # MainMenu, Game, Sandbox_A, Sandbox_B
        │   └── Tests/EditMode/      # 14 files NUnit Test (73 Unit Tests PASS 100%)
        └── StreamingAssets/
            ├── config.json          # Cấu hình Seed, RenderDistance, MaxHealth/Hunger ngoài code
            └── ResourcePacks/
                └── OptimumRealism_64x/ # Gói tài nguyên mẫu 64x64 kèm pack.mcmeta & textures
```

---

## 🧪 Hướng dẫn Chạy Kiểm thử Tự động (73 Unit Tests)

- **Cách 1 (Trong Unity Editor):** Mở **Window $\rightarrow$ General $\rightarrow$ Test Runner** $\rightarrow$ chọn tab **EditMode** $\rightarrow$ bấm **Run All** (73/73 PASS).
- **Cách 2 (Bằng CLI `.NET 9` không cần mở Unity):** Toàn bộ các lớp `Domain`, `Application`, `Infrastructure` đều độc lập hoàn toàn với `UnityEngine`, có thể biên dịch và chạy toàn bộ 73 bài kiểm thử NUnit trực tiếp qua `dotnet test` chỉ trong $< 1$ giây.

---

## 📜 Giấy phép & Ghi nhận Tài nguyên (Credits)

- Gói tài nguyên mẫu trong `Game/Assets/StreamingAssets/ResourcePacks/OptimumRealism_64x/` được trích xuất phục vụ mục đích học tập và minh họa cơ chế nạp gói tài nguyên động (`FolderResourcePackProvider`).
- Mã nguồn dự án được phát triển phục vụ học phần **Lập trình Hướng Đối tượng (OOP)**.
