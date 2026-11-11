# Thiết Kế Kỹ Thuật (Design Document): Lộ Trình Nâng Cấp Minecraft 1.12
**Giai đoạn 1: Cơ Chế Va Chạm Xúc Giác, Vật Phẩm 3D Rơi & Độ Bền Công Cụ Phân Cấp (`v1.2.0`)**

---

## 1. Bối cảnh & Mục tiêu (Intent & The Why)
Người chơi muốn nâng tầm trải nghiệm trò chơi từ mức đồ án cơ bản hướng đến sự hoàn thiện tiệm cận bản **Minecraft Java Edition 1.12**. 
Theo định hướng đã thống nhất trong buổi Brainstorming:
- **Giai đoạn 1 (`v1.2.0` - Hiện tại):** Tạm bỏ qua âm thanh; tập trung toàn lực vào cảm giác đập/chặt khối (Tactile Feel): **Hiệu ứng Hạt vỡ khối (Particles)**, **Thực thể vật phẩm 3D rơi trên đất (3D Dropped Items with Magnetic Pickup)**, và **Hệ thống Cấp bậc & Độ bền công cụ (Tiered Mining & Durability)**.
- **Giai đoạn 2 (`v1.3.0` - Tiếp theo):** Xây dựng Giao diện Inventory 1.12 (Ba lô 27 ô kéo thả chuột + Lưới chế tạo 2x2/3x3 + Trang bị).
- **Giai đoạn 3 (`v1.4.0` - Sau cùng):** Thế giới sâu (Hang động ngầm 3D Perlin Caves + Động lực học nước chảy lan).

Tài liệu này đặc tả chi tiết thiết kế kỹ thuật cho **Giai đoạn 1 (`v1.2.0`)**.

---

## 2. Mục Tiêu & Giới Hạn Phạm Vi (Scope)

### Trong phạm vi (In-Scope - `v1.2.0`):
1. **Hiệu ứng Hạt vỡ khối (Block Destruction Voxel Particles):**
   - Khi phá một khối bất kỳ, một chùm 12–16 hạt voxel nhỏ (kích thước ~0.08m) bung ra theo hình nón ngược với vận tốc ngẫu nhiên, chịu trọng lực rơi xuống đất và biến mất sau 0.6–0.8 giây.
   - Màu sắc của các hạt tự động lấy từ màu đặc trưng của khối vừa bị đập vỡ.
2. **Thực thể Vật phẩm Rơi 3D (3D Dropped Items & Magnetic Suction):**
   - Khi khối bị phá (hoặc quái vật chết rơi thịt), vật phẩm không nhảy ngay lập tức vào túi đồ mà xuất hiện dưới dạng một khối 3D thu nhỏ (~0.25m) lơ lửng trên mặt đất.
   - Vật phẩm xoay đều quanh trục thẳng đứng (Y) và nhấp nhô lên xuống dạng sóng Sin.
   - Khi người chơi tiến vào bán kính nhặt (~2.0m), vật phẩm tự động kích hoạt hiệu ứng hút nam châm (Magnetic Acceleration) lao về phía người chơi và nhập vào Inventory.
3. **Độ bền Công cụ & Cấp bậc Khai thác (Tool Durability & Tiered Mining):**
   - Mỗi công cụ (`ToolItem`) có độ bền tối đa chuẩn 1.12: Cúp gỗ (59), Cúp đá (131), Kiếm gỗ (59), Cúp kim cương (1561).
   - Mỗi lần đào khối rắn hoặc chém trúng quái vật, công cụ giảm 1 điểm độ bền.
   - Khi độ bền về 0, công cụ vỡ nát và biến mất khỏi ô túi đồ.
   - **Thanh Độ Bền Trực Quan:** Hiển thị thanh đo mỏng dưới ô Hotbar (Màu Xanh lá `> 60%` ➔ Vàng `> 25%` ➔ Đỏ `< 25%`).
   - **Cấp bậc Khai thác (Tiered Harvesting):**
     - Đào `Stone`, `CoalOre`: Cần tối thiểu Cúp gỗ (Tier 0).
     - Đào `IronOre`: Cần tối thiểu Cúp đá (Tier 1).
     - Đào `GoldOre`, `DiamondOre`: Cần tối thiểu Cúp sắt/kim cương (Tier >= 2).
     - Nếu dùng tay không hoặc sai loại cúp: Khối vẫn bị vỡ nhưng **không rơi ra vật phẩm** (chuẩn 1.12).

### Ngoài phạm vi (Out-of-Scope - Dành cho các phiên bản sau):
- Hệ thống âm thanh Sound FX (sẽ bổ sung khi có bộ asset audio).
- Giao diện kéo thả 27 ô (thuộc Giai đoạn 2 `v1.3.0`).
- Hang động 3D ngầm và nước chảy (thuộc Giai đoạn 3 `v1.4.0`).

---

## 3. Kiến Trúc 4 Tầng & Nguyên Lý OOP Áp Dụng

```
[ Minecraft.Domain ]
    ├── Contracts: ToolTier (Wood=0, Stone=1, Iron=2, Diamond=3)
    ├── Blocks: BlockType.RequiredToolTier, BlockType.PreferredToolType
    ├── Items: ToolItem.MaxDurability, ItemStack.Durability, ItemStack.DamageDurability()
    └── Entities: DroppedItemEntity (Position, Velocity, Item, Age)
            ▲
            │ (chỉ tham chiếu Domain)
[ Minecraft.Application ]
    ├── World: BlockInteractionService (Kiểm tra Tier, trừ độ bền cúp, tạo DroppedItemEntity)
    └── Survival: DroppedItemService (Hút nam châm, thu gom vào Inventory khi chạm người chơi)
            ▲
            │ (tham chiếu Domain + Application)
[ Minecraft.Presentation ]
    ├── World: VoxelParticleEmitter (Spawn hạt vụn voxel màu tương ứng)
    ├── World: DroppedItemView (Khối 3D xoay quanh trục Y, lượn sóng Sin, bay vào người chơi)
    ├── Player: PlayerInteractionController (Tích hợp hiệu ứng hạt khi vỡ khối)
    └── UI: SandboxHudView (Vẽ thanh độ bền thanh ngang dưới ô Hotbar)
```

### Các nguyên lý OOP bắt buộc:
1. **Liskov Substitution Principle (LSP):** `ToolItem` kế thừa `Item`, ghi đè các thuộc tính `MaxDurability` và `Tier`. Hệ thống túi đồ xử lý `Item` trừu tượng mà không bị vỡ logic.
2. **Single Responsibility Principle (SRP):** Tách bạch rõ rệt giữa dữ liệu vật lý rơi của thực thể (`DroppedItemEntity` trong Domain) và đối tượng hiển thị 3D quay tròn trên Unity Scene (`DroppedItemView` trong Presentation).
3. **Đóng gói (Encapsulation):** Trường `_durability` trong `ItemStack` là `private readonly/encapsulated`, chỉ có thể biến đổi thông qua phương thức nghiệp vụ có kiểm tra biên `TryApplyDamage(int amount, out bool isBroken)`.

---

## 4. Kịch Bản Kiểm Thử & Tiêu Chí Nghiệm Thu (Verification Criteria)
1. **Unit Tests (TDD):**
   - Viết bài test kiểm tra đào quặng `IronOre` bằng tay không ➔ Khối biến mất nhưng không rơi quặng.
   - Viết bài test đào `IronOre` bằng `StonePickaxe` ➔ Rơi ra `ItemId.IronOre`.
   - Viết bài test dùng Cúp gỗ đào 59 khối ➔ Cúp bị phá hủy và ô Hotbar chuyển thành rỗng.
   - Viết bài test `DroppedItemEntity` bị hút khi người chơi nằm trong bán kính 2m.
2. **Kiểm tra trực quan trong Unity Editor:**
   - Đập một khối bất kỳ: Thấy các mảnh vụn nhỏ bay tung tóe đúng màu khối.
   - Thấy khối vật phẩm 3D thu nhỏ xoay tít và nhấp nhô trên mặt đất.
   - Đi lại gần: Khối vật phẩm bay vèo vào ngực người chơi và số lượng trong Hotbar nhảy số.
   - Nhìn ô công cụ trên Hotbar: Có thanh đo độ bền chuyển dần từ Xanh sang Vàng và Đỏ khi sử dụng.
