# Kế Hoạch Triển Khai (Implementation Plan): Tactile Gameplay, 3D Dropped Items & Tiered Tool Durability

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Hiện thực hóa cơ chế xúc giác chuẩn Minecraft Java 1.12: đập khối bung hạt màu vỡ (Particles), sinh vật phẩm rơi 3D nhấp nhô và hút nam châm vào người chơi, công cụ có độ bền thực tế và phân cấp khai thác quặng (Tiered Mining).

**Architecture:** Mở rộng tầng `Minecraft.Domain` (hợp đồng `ToolTier`, phân cấp quặng `RequiredToolTier`, độ bền `Durability` trên `ToolItem` & `ItemStack`), cập nhật tầng `Minecraft.Application` (`BlockInteractionService` phối hợp với `DeathDropService` sinh `DroppedItemEntity` thay vì cộng trực tiếp vào túi), và nâng cấp tầng `Minecraft.Presentation` (`VoxelParticleEmitter`, `DroppedItemView` xoay nhấp nhô 3D, và thanh độ bền trực quan trên `SandboxHudView`).

**Tech Stack:** C# thuần (Domain & Application), Unity 6 (Presentation MonoBehaviours, LineRenderer, Mesh/Particle), .NET 9.0 Test Runner (NUnit).

**Spec:** [`docs/superpowers/specs/2026-11-10-tactile-gameplay-and-tool-durability-design.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/docs/superpowers/specs/2026-11-10-tactile-gameplay-and-tool-durability-design.md)

## Global Constraints
- `Minecraft.Domain` tuyệt đối không chứa `using UnityEngine;` (`noEngineReferences: true`).
- Mọi trường dữ liệu mới phải đóng gói `private readonly`, truy cập qua Property và kiểm tra hợp lệ đầu vào.
- Không dùng Singleton tĩnh cho logic nghiệp vụ.
- TDD Red -> Green -> Refactor: Viết test thất bại trước khi viết code thực thi.
- Cập nhật nhật ký phiên bản [`CHANGELOG.md`](file:///Users/admin/Documents/CodeProJect/MINECRAFT/CHANGELOG.md) theo chuẩn SemVer trước khi kết thúc.

## Review Focus
1. **Đào quặng sai cấp bậc công cụ:** Dùng cúp gỗ đào `IronOre` hoặc tay không đập đá ➔ Khối vỡ nhưng không rơi ra quặng.
2. **Cúp hết độ bền bị gãy:** Dùng cúp đào đến lượt cuối cùng ➔ Cúp gãy, xóa khỏi ô Hotbar và không gây ngoại lệ NullReferenceException ở lượt đào kế tiếp.
3. **Thanh độ bền hiển thị đúng tỷ lệ:** Công cụ đầy máu không vẽ thanh; công cụ bị hao mòn đổi màu mượt từ Xanh (`> 60%`) ➔ Vàng (`> 25%`) ➔ Đỏ (`< 25%`).
4. **Hút nam châm vật phẩm 3D không bị kẹt:** Vật phẩm rơi cách người chơi 2m tự động bay mượt vào túi đồ; nếu túi đồ đầy thì dừng ở cự ly gần và không biến mất.
5. **Hiệu ứng hạt vỡ khối không làm tụt FPS:** Tái sử dụng hoặc hủy GameObject mảnh vụn sau 0.7 giây, không để lọt rác bộ nhớ.

---

### Task 1: Domain - Tool Durability & Mining Tier Contracts

**Files:**
- Create: `Game/Assets/_Project/Scripts/Domain/Contracts/ToolTier.cs`
- Modify: `Game/Assets/_Project/Scripts/Domain/Blocks/BlockType.cs`
- Modify: `Game/Assets/_Project/Scripts/Domain/Blocks/BlockRegistry.cs`
- Modify: `Game/Assets/_Project/Scripts/Domain/Items/ToolItem.cs`
- Modify: `Game/Assets/_Project/Scripts/Domain/Items/ItemStack.cs`
- Test: `Game/Assets/_Project/Tests/EditMode/ToolDurabilityAndTierDomainTests.cs`

**Interfaces:**
- Produces:
  - `public enum ToolTier : byte { Hand = 0, Wood = 1, Stone = 2, Iron = 3, Diamond = 4 }`
  - `BlockType.RequiredToolTier` (default `ToolTier.Hand`, `Stone/CoalOre = Wood`, `IronOre = Stone`, `GoldOre/DiamondOre = Iron`)
  - `ToolItem.Tier`, `ToolItem.MaxDurability`
  - `ItemStack.Durability`, `ItemStack.MaxDurability`, `ItemStack.TryDamageTool(int damage, out bool isBroken)`

- [ ] **Step 1: Write the failing tests in `ToolDurabilityAndTierDomainTests.cs`**

```csharp
[Test]
public void ToolItem_DurabilityAndTier_AreConfiguredCorrectly()
{
    ToolItem woodPick = new ToolItem(ItemId.WoodenPickaxe, "Wooden Pickaxe", ToolType.Pickaxe, 2.0f, 2, ToolTier.Wood, 59);
    Assert.That(woodPick.Tier, Is.EqualTo(ToolTier.Wood));
    Assert.That(woodPick.MaxDurability, Is.EqualTo(59));

    ItemStack stack = new ItemStack(woodPick, 1);
    Assert.That(stack.Durability, Is.EqualTo(59));
    bool broken = stack.TryDamageTool(10, out bool isBroken);
    Assert.That(broken, Is.True);
    Assert.That(isBroken, Is.False);
    Assert.That(stack.Durability, Is.EqualTo(49));

    stack.TryDamageTool(49, out bool finalBreak);
    Assert.That(finalBreak, Is.True);
    Assert.That(stack.IsEmpty, Is.True);
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test /tmp/mc_unity_verify/RunTests.csproj --nologo -v minimal`
Expected: FAIL with "ToolTier does not exist" and "ToolItem does not contain definition for Tier".

- [ ] **Step 3: Implement `ToolTier.cs`, update `ToolItem.cs`, `ItemStack.cs`, `BlockType.cs`, and `BlockRegistry.cs`**
- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test /tmp/mc_unity_verify/RunTests.csproj --nologo -v minimal`
Expected: PASS (all tests pass).

- [ ] **Step 5: Commit Task 1**

```bash
git add Game/Assets/_Project/Scripts/Domain/ Game/Assets/_Project/Tests/
git commit -m "feat(domain): add ToolTier, block tier requirements and tool durability to ItemStack (v1.2.0)"
```

---

### Task 2: Application - Tiered Harvesting & Tool Wear in `BlockInteractionService`

**Files:**
- Modify: `Game/Assets/_Project/Scripts/Application/World/BlockInteractionService.cs`
- Modify: `Game/Assets/_Project/Scripts/Application/Survival/DeathDropService.cs`
- Test: `Game/Assets/_Project/Tests/EditMode/TieredMiningAndDurabilityApplicationTests.cs`

**Interfaces:**
- Consumes: `ToolTier`, `ToolItem.Tier`, `BlockType.RequiredToolTier`, `DeathDropService.SpawnDrop`
- Produces: `BlockInteractionService.TryBreakBlock(BlockPos pos, Inventory inv, IGameMode mode, out ItemId droppedItem, out bool toolBroke)`

- [ ] **Step 1: Write the failing tests in `TieredMiningAndDurabilityApplicationTests.cs`**

```csharp
[Test]
public void BlockInteraction_MiningIronOre_WithHand_DropsNothing_WithStonePickaxe_DropsOre()
{
    // Đào IronOre bằng tay -> không rơi quặng
    // Đào IronOre bằng StonePickaxe -> rơi quặng và trừ 1 độ bền cúp
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test /tmp/mc_unity_verify/RunTests.csproj --nologo -v minimal`
Expected: FAIL with missing overloads or logic.

- [ ] **Step 3: Implement Tier checking and durability damage in `BlockInteractionService.cs`**
  - Check `held.Definition as ToolItem`.
  - Compare `tool.Tier >= block.RequiredToolTier`. If not met, `dropItem = ItemId.None`.
  - If in Survival and tool was used, call `held.TryDamageTool(1, out bool isBroken)`. If `isBroken`, clear the hotbar slot.
  - Spawn drop through `DeathDropService` if configured, or pass drop information to Presentation.
- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test /tmp/mc_unity_verify/RunTests.csproj --nologo -v minimal`
Expected: PASS.

- [ ] **Step 5: Commit Task 2**

```bash
git add Game/Assets/_Project/Scripts/Application/ Game/Assets/_Project/Tests/
git commit -m "feat(application): enforce tiered mining requirements and tool wear in BlockInteractionService"
```

---

### Task 3: Presentation - Voxel Block Breaking Particle Emitter

**Files:**
- Create: `Game/Assets/_Project/Scripts/Presentation/World/VoxelParticleEmitter.cs`
- Modify: `Game/Assets/_Project/Scripts/Presentation/Player/PlayerInteractionController.cs`
- Modify: `Game/Assets/_Project/Scripts/Presentation/Bootstrap/GameBootstrap.cs`

**Interfaces:**
- Produces: `VoxelParticleEmitter.SpawnBreakParticles(Vector3 blockWorldPos, Color blockColor, int count = 16)`

- [ ] **Step 1: Create `VoxelParticleEmitter.cs`**
  - Sinh 14–18 khối lập phương con kích thước 0.08m tại tâm khối vừa vỡ.
  - Áp dụng vận tốc ban đầu hướng lên và tản ra xung quanh (`Random.insideUnitSphere`).
  - Gán màu sắc đặc trưng của khối (`blockColor`).
  - Tự động rơi và biến mất sau 0.65 giây.
- [ ] **Step 2: Wire `VoxelParticleEmitter` vào `PlayerInteractionController.cs`**
  - Khi đập vỡ khối thành công, gọi `SpawnBreakParticles` với tọa độ tâm khối và màu sắc lấy từ `ResourcePackView`.
- [ ] **Step 3: Compile verification**

Run: `dotnet test /tmp/mc_unity_verify/VerifyAll.csproj --nologo -v minimal`
Expected: PASS with 0 compilation errors.

- [ ] **Step 4: Commit Task 3**

```bash
git add Game/Assets/_Project/Scripts/Presentation/
git commit -m "feat(presentation): add VoxelParticleEmitter for block breaking visual feedback"
```

---

### Task 4: Presentation & Application - 3D Dropped Item Entity Rendering & Magnetic Suction

**Files:**
- Create: `Game/Assets/_Project/Scripts/Presentation/World/DroppedItemView.cs`
- Modify: `Game/Assets/_Project/Scripts/Presentation/Bootstrap/GameBootstrap.cs`
- Modify: `Game/Assets/_Project/Scripts/Application/Survival/DeathDropService.cs`

**Interfaces:**
- Consumes: `DroppedItemEntity`, `DeathDropService.ActiveDrops`
- Produces: `DroppedItemView.Initialize(DroppedItemEntity entity, Material blockMaterial)`

- [ ] **Step 1: Create `DroppedItemView.cs`**
  - Khối 3D lập phương thu nhỏ (kích thước 0.28m) hiển thị vật phẩm rơi.
  - Trong hàm `Update()`: xoay quanh trục Y với tốc độ 90°/s; nhấp nhô lượn sóng Sin (`0.08m * Sin(Time.time * 3)`).
  - Khi khoảng cách tới người chơi `< 2.2m`: kích hoạt lực hút nam châm `Vector3.MoveTowards / Lerp` lao về phía người chơi.
  - Khi `entity.IsExpired` hoặc `entity.Payload.IsEmpty`: tự động hủy GameObject.
- [ ] **Step 2: Wire dropped item spawning in `GameBootstrap.cs`**
  - Khi đập khối ở chế độ Survival, `BlockInteractionService` sinh `DroppedItemEntity` thông qua `DeathDropService`.
  - `GameBootstrap` quản lý `Dictionary<DroppedItemEntity, DroppedItemView>` để sinh GameObject 3D tương ứng.
- [ ] **Step 3: Compile and Test verification**

Run: `dotnet test /tmp/mc_unity_verify/VerifyAll.csproj --nologo -v minimal && dotnet test /tmp/mc_unity_verify/RunTests.csproj --nologo -v minimal`
Expected: PASS.

- [ ] **Step 4: Commit Task 4**

```bash
git add Game/Assets/_Project/Scripts/
git commit -m "feat(presentation): render floating 3D dropped items with rotating, bobbing and magnetic pickup"
```

---

### Task 5: Presentation - Tool Durability Bar on Hotbar & HUD Integration

**Files:**
- Modify: `Game/Assets/_Project/Scripts/Presentation/UI/SandboxHudView.cs`
- Modify: `CHANGELOG.md`

**Interfaces:**
- Consumes: `ItemStack.Durability`, `ItemStack.MaxDurability`
- Produces: Visual durability progress bar under hotbar slots

- [ ] **Step 1: Update `SandboxHudView.cs`**
  - Trong vòng lặp vẽ 9 ô Hotbar: kiểm tra nếu ô chứa `ToolItem` và `stack.Durability < stack.MaxDurability`:
  - Tính tỷ lệ `durabilityPercent = (float)stack.Durability / stack.MaxDurability`.
  - Chọn màu: `> 0.6f` Xanh lá (`#4CAF50`), `> 0.25f` Vàng (`#FFC107`), `<= 0.25f` Đỏ (`#F44336`).
  - Vẽ một thanh ngang 48px x 4px ở mép đáy của ô slot.
- [ ] **Step 2: Update `CHANGELOG.md` with version `v1.2.0`**
- [ ] **Step 3: Full Verification of all 4 layers & all Unit Tests**

Run: `dotnet test /tmp/mc_unity_verify/VerifyAll.csproj --nologo -v minimal && dotnet test /tmp/mc_unity_verify/RunTests.csproj --nologo -v minimal`
Expected: 100% tests pass, 0 compile errors.

- [ ] **Step 4: Final Git Tag & Push**

```bash
git checkout main && git merge --no-ff dev -m "Merge branch 'dev' into main for release v1.2.0"
git tag -a v1.2.0 -m "Release v1.2.0: Tactile Gameplay, Particles, 3D Dropped Items & Tiered Tool Durability"
git push origin dev main v1.2.0
git checkout dev
```
