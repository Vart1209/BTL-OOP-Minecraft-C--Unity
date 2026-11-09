# Tuần T5 — Hoàn thiện 100% Dự án, Bộ Sơ đồ UML, Hồ sơ Báo cáo/Vấn đáp, Editor Scene Builder & Phát hành (`v1.0.0`) Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Hoàn thành trọn bộ Tuần T5 và phát hành phiên bản chính thức `v1.0.0` cho Bài Tập Lớn Lập trình Hướng Đối tượng (Minecraft Voxel Survival & Creative), bao gồm: Công cụ tự động dựng Scene chơi được trong 1 cú nhấp chuột (`EditorSceneSetup.cs`), Bộ 3 tài liệu Sơ đồ UML đồng bộ 100% với code (`use-case-diagram.md`, `class-diagram.md`, `sequence-diagrams.md`), Cẩm nang Bảo vệ & Vấn đáp 20/20 tiêu chí (`defense-guide.md`), cập nhật `README.md`, `CHANGELOG.md` và phát hành Git Tag `v1.0.0`.

**Architecture:** Giữ vững kiến trúc 4 tầng Assembly (`.asmdef`):
- `Minecraft.Domain`: C# thuần 100% (`noEngineReferences: true`), 48 lớp/interface/struct/enum.
- `Minecraft.Application`: C# thuần 100%, 11 lớp dịch vụ điều phối 12 Use Case.
- `Minecraft.Infrastructure`: C# chuẩn (`System.IO`, `System.Text.Json`), 4 lớp I/O bọc `try-catch` an toàn.
- `Minecraft.Presentation`: 11 lớp `MonoBehaviour` View/Controller/Bootstrap/EditorSetup, kết nối Pure Dependency Injection tại `GameBootstrap`.

**Tech Stack:** C# 12 / .NET 9, Unity 6 (`6000.6.8f1`), NUnit 3 (73 Unit Tests), Mermaid UML, Git Flow (`main`, `dev`, `feature/*`, tag `v1.0.0`).

**Spec:** `docs/brainstorm.md` (Mục 1–8), `Barem cham diem/Checklist_BTL_LTHDT_v2.xlsx` (sheet `Nhóm 02 người` [S2]), `AGENTS.md`.

---

## Task Decomposition

### PHẦN 1: TỰ ĐỘNG HÓA DỰNG SCENE UNITY TRONG 1 CLICK (`feature/t5-uml-report-and-scene-setup`)

- [x] **Task 1.1: Hoàn thiện `GameBootstrap` & Tạo `EditorSceneSetup` (Presentation)**
  - **Files:**
    - Modify: `Game/Assets/_Project/Scripts/Presentation/Bootstrap/GameBootstrap.cs`
    - Create: `Game/Assets/_Project/Scripts/Presentation/Bootstrap/EditorSceneSetup.cs`
  - **Description:**
    - Cập nhật `GameBootstrap.cs` bổ sung đầy đủ các trường `[SerializeField]` (`_worldView`, `_resourcePackView`, `_playerController`, `_interactionController`, `_hudView`, `_dayNightView`, `_atmosphereFogView`), tích hợp `ChunkLoadService` để tự động sinh và nạp Mesh các Chunk xung quanh người chơi ngay khi bấm Play, đồng thời truyền đúng tham số cho `FirstPersonPlayerController.Initialize`, `PlayerInteractionController.Initialize`, `SandboxHudView.Initialize`.
    - Tạo `EditorSceneSetup.cs` bọc trong `#if UNITY_EDITOR ... #endif` cung cấp 2 menu:
      - `[MenuItem("Minecraft OOP/1. Auto-Setup Playable Game Scene")]`: Tự động tạo/làm sạch `Directional Light` (`DayNightView`, `AtmosphereFogView`), `WorldRoot` (`WorldView`, `ResourcePackView`, Material URP/Standard), `Player` tại `(0, 42, 0)` (`CharacterController`, `FirstPersonPlayerController`, `PlayerInteractionController`, `Main Camera` tại `(0, 1.6f, 0)`), `HUD` (`SandboxHudView`), và `GameBootstrap` nối sẵn mọi `SerializedProperty`.
      - `[MenuItem("Minecraft OOP/2. Generate All 4 Scenes (MainMenu, Game, Sandbox_A, Sandbox_B)")]`: Tự động dựng và lưu 4 file `.unity` vào `Game/Assets/_Project/Scenes/`.

---

### PHẦN 2: BỘ TÀI LIỆU SƠ ĐỒ UML ĐỒNG BỘ 100% VỚI CODE (`docs/uml/`)

- [x] **Task 2.1: Sơ đồ Use Case & Đặc tả 12 Use Case (`docs/uml/use-case-diagram.md`)**
  - **Files:**
    - Create: `docs/uml/use-case-diagram.md`
  - **Description:**
    - Vẽ sơ đồ Use Case tổng thể bằng Mermaid (`flowchart LR`) với 3 tác nhân (Người chơi Survival, Người chơi Creative, Hệ thống Tự động) và 12 Use Case (UC01–UC06 thuộc Phân hệ A, UC07–UC12 thuộc Phân hệ B).
    - Lập bảng đặc tả chi tiết 12 Use Case và giải thích cơ chế phân quyền tác nhân qua `IGameMode` (Tiêu chí 1 & 4).

- [x] **Task 2.2: Sơ đồ Lớp UML Tổng thể & 5 Mối Quan hệ (`docs/uml/class-diagram.md`)**
  - **Files:**
    - Create: `docs/uml/class-diagram.md`
  - **Description:**
    - Vẽ sơ đồ lớp Mermaid (`classDiagram`) chứa trên 30 lớp/interface/struct thực tế trong code, khớp 100% tên lớp, thuộc tính và phương thức.
    - Bảng giải thích ý nghĩa từng thuộc tính của 22 lớp thực thể/giá trị trong tầng Domain (Tiêu chí 2).
    - Bảng phân tích đủ 5 mối quan hệ OOP (Kế thừa, Thực thi, Hợp thành, Kết tập, Phụ thuộc) xuyên suốt 2 phân hệ A & B (Tiêu chí 5 & 6).

- [x] **Task 2.3: Bộ 4 Sơ đồ Tuần tự (`docs/uml/sequence-diagrams.md`)**
  - **Files:**
    - Create: `docs/uml/sequence-diagrams.md`
  - **Description:**
    - Vẽ 4 sơ đồ tuần tự bằng Mermaid (`sequenceDiagram`) khớp 100% thứ tự gọi hàm thực tế trong code (2 sơ đồ cho Thành viên A, 2 sơ đồ cho Thành viên B) đáp ứng Tiêu chí 7.

---

### PHẦN 3: BÁO CÁO TỔNG KẾT, CẨM NANG VẤN ĐÁP & PHÁT HÀNH `v1.0.0`

- [x] **Task 3.1: Cẩm nang Bảo vệ, 20 Tiêu chí, 3 Bài Mở rộng & README (`docs/defense-guide.md`, `README.md`)**
  - **Files:**
    - Create: `docs/defense-guide.md`
    - Modify: `README.md`
  - **Description:**
    - Lập bảng ánh xạ 20/20 tiêu chí Barem [S2], bảng 12 quy tắc nghiệp vụ (R1–R12), phân tích 5 Design Pattern (Strategy, Observer, Factory, State, Repository), hướng dẫn code trực tiếp 3 bài mở rộng trong < 5 phút (Tiêu chí 12), kịch bản Demo 7 bước và 10 câu hỏi xoáy kèm đáp án mẫu cho A & B.
    - Cập nhật `README.md` đầy đủ thống kê `v1.0.0` (73 Unit Tests, 80+ files C#, phím tắt, hướng dẫn 1-click Scene Setup, liên kết tài liệu UML & Vấn đáp).

- [x] **Task 3.2: Kiểm thử NUnit (.NET 9), Cập nhật `CHANGELOG.md` & Phát hành `v1.0.0`**
  - **Files:**
    - Modify: `CHANGELOG.md`
  - **Description:**
    - Chạy bộ kiểm thử NUnit xác nhận 100% test PASS.
    - Merge `feature/t5-uml-report-and-scene-setup` vào `dev`, cập nhật `CHANGELOG.md` lên `[v1.0.0]`, merge `dev` sang `main`, gắn tag `v1.0.0` và push toàn bộ lên `origin`.
