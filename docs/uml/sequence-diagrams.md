# Bộ 4 Sơ đồ Tuần tự (Sequence Diagrams) Đồng bộ 100% Thứ tự Gọi hàm Thực tế (Tiêu chí 7 — Barem [S2])

> Tài liệu này trình bày **4 Sơ đồ Tuần tự (Sequence Diagrams)** bằng Mermaid `sequenceDiagram`, chia đều **2 sơ đồ cho Thành viên A (Phân hệ World)** và **2 sơ đồ cho Thành viên B (Phân hệ Survival)**, bám sát 100% tên lớp, tên phương thức và thứ tự gọi hàm thực tế khi chương trình vận hành.

---

## Sơ đồ 1 (Thành viên A — Phân hệ World): Luồng Dò tia DDA & Phá Khối Rơi Vật phẩm (UC03 — Quy tắc R1, R3, R5, R7, R12)

**Kịch bản:** Người chơi nhấn chuột trái để đập vỡ một khối Đá (`BlockId.Stone`) trong tầm với $\le 5.0$ block. Hệ thống dò tia bằng thuật toán DDA (`VoxelRaycaster`), kiểm tra quyền phá khối của `IGameMode`, cập nhật dữ liệu `World`, kích hoạt sự kiện `BlockChanged` để `WorldView` dựng lại lưới Mesh của Chunk, đồng thời rơi vật phẩm `ItemId.Cobblestone` vào `Inventory` của người chơi.

```mermaid
sequenceDiagram
    autonumber
    actor User as Người chơi
    participant Controller as PlayerInteractionController<br/>(Presentation)
    participant Raycaster as VoxelRaycaster<br/>(Domain)
    participant Service as BlockInteractionService<br/>(Application)
    participant Mode as IGameMode<br/>(Survival/Creative)
    participant World as World<br/>(IWorldReader/IWorldWriter)
    participant View as WorldView<br/>(Presentation Observer)
    participant Inv as Inventory<br/>(Domain)

    User->>Controller: Nhấn Chuột trái (Input.GetMouseButtonDown(0))
    Controller->>Raycaster: Raycast(rayOrigin, rayDir, maxDistance = 5.0f)
    loop Duyệt ô lưới 3D DDA (distance <= 5.0f - Quy tắc R7)
        Raycaster->>World: IsInBounds(currentPos) & GetBlock(currentPos)
        World-->>Raycaster: BlockId (ví dụ: BlockId.Stone)
    end
    Raycaster-->>Controller: VoxelRaycastHit (IsHit = true, BlockPosition)

    alt hit.IsHit == true
        Controller->>Service: TryBreakBlock(hit.BlockPosition, player.Inventory, player.CurrentGameMode)
        Service->>World: IsInBounds(targetPos) (Quy tắc R3)
        World-->>Service: true
        Service->>World: GetBlock(targetPos)
        World-->>Service: currentBlock (BlockId.Stone)
        Service->>Mode: CanBreakBlock(currentBlock) (Quy tắc R1 & R12)
        Mode-->>Service: true (Survival cho phép phá Stone, chặn Bedrock)

        Service->>World: SetBlock(targetPos, BlockId.Air)
        World->>World: chunk.SetBlock(localX, localY, localZ, BlockId.Air)
        World--)View: Phát sự kiện BlockChanged(BlockChangedEventArgs)
        activate View
        View->>View: OnBlockChanged(args) -> RebuildChunkMesh(chunk, chunkView)
        deactivate View
        World-->>Service: true (đã xóa khối thành công)

        opt gameMode.ConsumesItemsOnPlace == true (Chế độ Survival - Quy tắc R5)
            Service->>Service: _blockRegistry.TryGet(Stone) -> DropItemId = ItemId.Cobblestone
            Service->>Service: _itemRegistry.TryGet(ItemId.Cobblestone, out dropItem)
            Service->>Inv: AddItem(dropItem, 1)
            Inv--)Inv: Phát sự kiện InventoryChanged()
            Inv-->>Service: remainder = 0
        end

        Service-->>Controller: true
    end
```

---

## Sơ đồ 2 (Thành viên A — Phân hệ World): Luồng Di chuyển Sinh Chunk & Dựng Lưới Face Culling (UC02 — Strategy Pattern)

**Kịch bản:** Khi khởi động hoặc khi người chơi di chuyển sang một tọa độ Chunk mới, `GameBootstrap` gọi `ChunkLoadService.UpdateActiveChunks(centerChunk)`. Dịch vụ tự động sinh các Chunk mới trong bán kính tầm nhìn thông qua chiến lược `PerlinTerrainGenerator`, giải phóng các Chunk ở xa, sau đó `WorldView` sử dụng `ChunkMeshBuilder` để loại bỏ các mặt bị che khuất (Face Culling 6 hướng) và đẩy lưới vào `ChunkView`.

```mermaid
sequenceDiagram
    autonumber
    participant Bootstrap as GameBootstrap<br/>(Presentation)
    participant LoadService as ChunkLoadService<br/>(Application)
    participant World as World<br/>(Domain)
    participant Generator as PerlinTerrainGenerator<br/>(ITerrainGenerator Strategy)
    participant WorldView as WorldView<br/>(Presentation)
    participant MeshBuilder as ChunkMeshBuilder<br/>(Domain)
    participant ChunkView as ChunkView<br/>(Presentation)

    Bootstrap->>Bootstrap: UpdateStreamingChunks() -> tính currentChunk từ Player.Position
    Bootstrap->>LoadService: UpdateActiveChunks(currentChunk)

    loop Duyệt dx, dz trong [-renderDistance .. +renderDistance]
        alt Chunk chưa có trong _loadedChunks
            LoadService->>World: GenerateChunk(coord)
            World->>World: new Chunk(coord)
            loop Duyệt x(0..15), z(0..15), y(0..63)
                World->>Generator: GetBlockAt(worldPos, _seed)
                Generator-->>World: BlockId (Bedrock / Stone / Dirt / Grass / Air)
                opt block != BlockId.Air
                    World->>World: chunk.SetBlock(x, y, z, block)
                end
            end
            World->>World: chunk.MarkClean() & _chunks.Add(coord, chunk)
            World-->>LoadService: Chunk
            LoadService->>LoadService: _loadedChunks.Add(coord) & newlyLoaded.Add(coord)
        end
    end

    loop Duyệt các Chunk vượt quá bán kính (renderDistance + 1)
        LoadService->>World: UnloadChunk(loadedCoord)
        LoadService->>LoadService: unloaded.Add(loadedCoord)
    end

    LoadService-->>Bootstrap: ChunkLoadResult(NewlyLoaded, Unloaded)

    loop Với mỗi coord trong result.NewlyLoaded
        Bootstrap->>WorldView: LoadChunkView(coord)
        WorldView->>World: TryGetChunk(coord, out chunk)
        World-->>WorldView: true, chunk
        WorldView->>MeshBuilder: BuildMesh(chunk)
        loop Duyệt từng khối rắn & kiểm tra 6 mặt láng giềng (Face Culling)
            MeshBuilder->>World: GetBlock(neighborPos)
            World-->>MeshBuilder: neighborBlockId (chỉ vẽ mặt nếu láng giềng trong suốt/Air)
        end
        MeshBuilder-->>WorldView: ChunkMeshData (Vertices, Triangles, Uvs, Normals)
        WorldView->>ChunkView: RenderMesh(meshData)
        ChunkView->>ChunkView: Cập nhật MeshFilter.sharedMesh & MeshCollider.sharedMesh
    end

    loop Với mỗi coord trong result.Unloaded
        Bootstrap->>WorldView: UnloadChunkView(coord)
        WorldView->>ChunkView: Destroy(view.gameObject)
    end
```

---

## Sơ đồ 3 (Thành viên B — Phân hệ Survival): Luồng Chế tạo Vật phẩm Atomic theo Quy tắc R10 (UC09)

**Kịch bản:** Người chơi nhấn phím `C` mở bảng Bàn Chế Tạo trên `SandboxHudView` và bấm nút chế tạo công thức **Cúp gỗ (`WoodenPickaxe`)** cần `3 Planks + 2 Wood`. `CraftingService` thực thi **Quy tắc R10 ("Đủ hết hoặc không làm gì" — Atomic Transaction)**: kiểm tra đủ tất cả nguyên liệu qua `CanCraft` trước khi trừ từng nguyên liệu và thêm thành phẩm vào `Inventory`.

```mermaid
sequenceDiagram
    autonumber
    actor User as Người chơi
    participant HUD as SandboxHudView<br/>(Presentation UI)
    participant CraftService as CraftingService<br/>(Application)
    participant Recipe as Recipe<br/>(Domain)
    participant Inv as Inventory<br/>(Domain)
    participant Registry as ItemRegistry<br/>(Domain)

    User->>HUD: Nhấn phím C mở Menu & Bấm nút "Chế tạo: WoodenPickaxe x1"
    HUD->>CraftService: TryCraft(recipe, player.Inventory)

    CraftService->>CraftService: CanCraft(recipe, inventory)
    CraftService->>Registry: Contains(recipe.OutputItemId)
    Registry-->>CraftService: true

    loop Duyệt từng RecipeIngredient trong recipe.Ingredients
        CraftService->>Inv: CountItem(ingredient.ItemId)
        Inv-->>CraftService: totalAvailableCount
    end

    alt Thiếu bất kỳ nguyên liệu nào (totalAvailableCount < RequiredCount)
        CraftService-->>HUD: false (Quy tắc R10: Giữ nguyên túi đồ, không trừ món nào)
    else Đủ toàn bộ nguyên liệu yêu cầu
        loop Duyệt từng RecipeIngredient trong recipe.Ingredients
            CraftService->>Inv: RemoveItem(ingredient.ItemId, ingredient.RequiredCount)
            Inv->>Inv: Trừ số lượng trên các ItemStack & xóa ô nếu Count == 0
            Inv--)HUD: Phát sự kiện InventoryChanged()
            Inv-->>CraftService: true
        end

        CraftService->>Registry: Get(recipe.OutputItemId)
        Registry-->>CraftService: outputItem (ToolItem: WoodenPickaxe, MaxStackSize = 1)
        CraftService->>Inv: AddItem(outputItem, recipe.OutputCount)
        Inv->>Inv: Đặt vào ô trống đầu tiên (Quy tắc R6: ToolItem stack = 1)
        Inv--)HUD: Phát sự kiện InventoryChanged()
        Inv-->>CraftService: remainder = 0

        CraftService-->>HUD: true (Chế tạo thành công!)
    end
```

---

## Sơ đồ 4 (Thành viên B — Phân hệ Survival): Luồng Sinh Quái Ban Đêm (R11), Chiến đấu, Chết Rơi Đồ & Hồi sinh (R9 — UC12)

**Kịch bản:** Khi `DayNightCycle` bước vào ban đêm (`IsNight == true`), `MobSpawnerService` kiểm tra điều kiện **Quy tắc R11** (ban đêm, số lượng $< 10$, khoảng cách tới người chơi $\ge 24$ block) và gọi `MobFactory.CreateMob` để sinh `Zombie`. Trong mỗi khung hình, `Zombie.Tick` cập nhật AI truy đuổi người chơi; khi tiếp cận trong bán kính tấn công, `MobCombatService` gây sát thương lên `Player.Health`. Khi máu giảm về `0`, sự kiện `Died` kích hoạt `DeathDropService.DropInventoryOnDeath` làm rơi toàn bộ đồ trong túi thành các `DroppedItemEntity` trên mặt đất và hồi sinh người chơi về `SpawnPoint` (**Quy tắc R9**).

```mermaid
sequenceDiagram
    autonumber
    participant Bootstrap as GameBootstrap<br/>(Presentation)
    participant Cycle as DayNightCycle<br/>(Domain)
    participant Spawner as MobSpawnerService<br/>(Application)
    participant Factory as MobFactory<br/>(IMobFactory)
    participant Zombie as Zombie<br/>(Mob : Entity)
    participant Combat as MobCombatService<br/>(Application)
    participant Player as Player<br/>(Domain)
    participant Health as Health<br/>(Domain)
    participant DropService as DeathDropService<br/>(Application)

    Bootstrap->>Cycle: Tick(deltaTime)
    Cycle--)Bootstrap: IsNight == true (NormalizedTime trong [0.55, 0.90))

    opt Ban đêm & ActiveMobs.Count < 10 & Khoảng cách >= 24 block (Quy tắc R11)
        Spawner->>Factory: CreateMob(MobType.Zombie, spawnPos)
        Factory->>Zombie: new Zombie(spawnPos)
        Factory-->>Spawner: Thực thể Zombie
        Spawner->>Spawner: _activeMobs.Add(zombie)
    end

    Bootstrap->>Spawner: Tick(deltaTime, player.Position, isTargetable)
    Spawner->>Zombie: UpdateAI(player.Position, isTargetable, deltaTime)
    Zombie->>Zombie: CurrentState = MobState.Chase -> MoveTo(về phía Player)

    Bootstrap->>Combat: ProcessHostileAttacks(Spawner.ActiveMobs, player)
    opt Zombie còn sống & Khoảng cách <= 2.0f & player.CurrentGameMode.CanTakeDamage
        Combat->>Player: Health.TakeDamage(zombie.AttackDamage = 3, player.CurrentGameMode)
        Player->>Health: TakeDamage(3, SurvivalMode)
        Health->>Health: _current = Math.Max(0, _current - 3)
        Health--)Bootstrap: Phát sự kiện HealthChanged(_current, _max)

        opt Khi _current == 0 (Người chơi tử vong - Quy tắc R9)
            Health--)Bootstrap: Phát sự kiện Died()
            Bootstrap->>DropService: DropInventoryOnDeath(player)
            opt player.CurrentGameMode.ConsumesItemsOnPlace == true (Survival)
                loop Duyệt 36 ô trong player.Inventory
                    DropService->>DropService: new DroppedItemEntity(scatterPos, slotStack)
                    DropService->>DropService: _activeDrops.Add(droppedEntity)
                end
                DropService->>Player: Inventory.Clear()
            end
            DropService->>Player: Respawn()
            Player->>Player: MoveTo(_spawnPoint) & Health.ResetToMax() & Hunger.ResetToMax()
            DropService-->>Bootstrap: Số ngăn xếp vật phẩm đã rơi
        end
    end
```
