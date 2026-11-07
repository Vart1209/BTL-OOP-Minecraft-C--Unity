# Minecraft Voxel Survival – Bài Tập Lớn Lập Trình Hướng Đối Tượng (OOP)

Dự án game thế giới voxel kiểu Minecraft (Survival + Creative) xây dựng bằng **C# trên Unity 6.3 LTS (`6000.3.28f1`)**, thiết kế theo kiến trúc phân tầng (Domain / Application / Infrastructure / Presentation) bám sát barem chấm điểm nhóm 2 người.

## Tài liệu quan trọng
- Kế hoạch & Thiết kế Brainstorm: [`docs/brainstorm.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/docs/brainstorm.md)
- Quy tắc dự án & Tự động ghi log AI: [`AGENTS.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/AGENTS.md)
- Nhật ký phiên bản tự động: [`CHANGELOG.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/CHANGELOG.md)
- Barem chấm điểm gốc: [`Barem cham diem/Checklist_BTL_LTHDT_v2.xlsx`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/Barem%20cham%20diem/Checklist_BTL_LTHDT_v2.xlsx)

## Cấu trúc thư mục
```text
MINECRAFT/
├── AGENTS.md                 # Rule cho AI (tự động ghi CHANGELOG.md + chuẩn OOP)
├── CHANGELOG.md              # Nhật ký phiên bản tự động (Semantic Versioning)
├── README.md                 # Hướng dẫn cài đặt và chạy dự án
├── docs/
│   └── brainstorm.md         # Tài liệu thiết kế tổng thể (Bước 1 -> Bước 3)
└── Game/                     # Thư mục gốc của Unity Project
    └── Assets/
        ├── _Project/
        │   ├── Scripts/
        │   │   ├── Domain/           # C# thuần (noEngineReferences = true)
        │   │   ├── Application/      # Các Service điều phối Use Case
        │   │   ├── Infrastructure/   # Lưu/Tải JSON, Config, ResourcePack
        │   │   └── Presentation/     # MonoBehaviour, ScriptableObject, UI, View
        │   ├── Data/                 # Asset dữ liệu Block, Item, Recipe
        │   ├── Art/                  # Texture, Material, Shader
        │   ├── Prefabs/              # Prefab dùng chung
        │   ├── Scenes/               # MainMenu, Game, Sandbox_A, Sandbox_B
        │   └── Tests/EditMode/       # Bộ Unit Test NUnit (>= 20 test cases)
        └── StreamingAssets/
            ├── config.json           # Cấu hình game ngoài code (đường dẫn tương đối)
            └── ResourcePacks/        # Thư mục bỏ texture pack lúc chạy
```

## Hướng dẫn cài đặt cho 2 thành viên

### 1. Cài đặt Unity Editor (bắt buộc đúng phiên bản)
1. Mở **Unity Hub** → tab **Installs** → **Install Editor**.
2. Chọn bản **Unity 6.3 LTS (`6000.3.28f1`)** (Apple Silicon trên máy Mac M-series).
3. Tick chọn thêm module:
   - **Windows Build Support (Mono)** (để máy Mac xuất được file `.exe` cho Windows).
   - **Mac Build Support (Mono / IL2CPP)**.

### 2. Khởi tạo / Mở project trong Unity Hub
- **Thành viên đầu tiên (tạo project lần đầu):**
  1. Trong Unity Hub → **Projects** → **New project** → chọn template **Universal 3D (URP)**.
  2. Đặt tên project hoặc trỏ vào thư mục `Game/` (hoặc tạo xong rồi chép `Packages/` và `ProjectSettings/` vào `Game/`, giữ nguyên `Game/Assets/_Project/` và `Game/Assets/StreamingAssets/`).
  3. Vào **Edit → Project Settings → Editor**:
     - **Asset Serialization Mode**: `Force Text`
     - **Version Control Mode**: `Visible Meta Files`
- **Thành viên thứ hai (clone từ GitHub):**
  1. `git clone <repo-url>`
  2. Mở Unity Hub → **Add** → **Add project from disk** → chọn thư mục `MINECRAFT/Game`.

### 3. Cấu hình Git định danh cá nhân (để tính đủ >= 20 commits chia đều 2 người)
Chạy trong Terminal tại thư mục dự án:
```bash
git config user.name "Ten Cua Ban"
git config user.email "email-github-cua-ban@example.com"
```
