using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Minecraft.Application.Survival;
using Minecraft.Application.World;
using Minecraft.Domain.Blocks;
using Minecraft.Domain.Configuration;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.Crafting;
using Minecraft.Domain.Entities;
using Minecraft.Domain.Entities.Mobs;
using Minecraft.Domain.Environment;
using Minecraft.Domain.GameModes;
using Minecraft.Domain.Items;
using Minecraft.Domain.World;
using Minecraft.Infrastructure.Config;
using Minecraft.Infrastructure.Persistence;
using Minecraft.Infrastructure.ResourcePacks;
using Minecraft.Presentation.Environment;
using Minecraft.Presentation.Mobs;
using Minecraft.Presentation.Player;
using Minecraft.Presentation.UI;
using Minecraft.Presentation.World;
using DomainWorld = Minecraft.Domain.World.World;
using SysVector3 = System.Numerics.Vector3;

namespace Minecraft.Presentation.Bootstrap
{
    /// <summary>
    /// Điểm khởi động tập trung và nối ghép phụ thuộc của toàn bộ trò chơi (Composition Root / Dependency Injection Root).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Nói KHÔNG với Singleton (Barem [S2] Tiêu chí 4 &amp; 16):</b> Dự án tuyệt đối không dùng <c>Instance</c> tĩnh cho logic nghiệp vụ. Toàn bộ các dịch vụ (Service), kho lưu trữ (Repository), nhà máy (Factory) và thực thể (Entity) được khởi tạo và tiêm thủ công (Pure DI) tại đây.</item>
    ///   <item><b>Liên kết 4 tầng kiến trúc:</b> Tầng Presentation nắm quyền điều phối vòng đời của các tầng dưới (Domain, Application, Infrastructure).</item>
    ///   <item><b>Xử lý ngoại lệ tự phục hồi (Tiêu chí 19):</b> Đọc file cấu hình ngoài bằng <see cref="ConfigLoader"/> và nạp Resource Pack bằng <see cref="FolderResourcePackProvider"/> có <c>try-catch</c> an toàn kèm fallback <see cref="BuiltInAtlasProvider"/>, đảm bảo game luôn chạy được kể cả khi thiếu file cấu hình hoặc thư mục tài nguyên.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class GameBootstrap : MonoBehaviour
    {
        [Header("Views liên kết trong Scene")]
        [SerializeField] private WorldView _worldView;
        [SerializeField] private ResourcePackView _resourcePackView;
        [SerializeField] private FirstPersonPlayerController _playerController;
        [SerializeField] private PlayerInteractionController _interactionController;
        [SerializeField] private SandboxHudView _hudView;
        [SerializeField] private DayNightView _dayNightView;
        [SerializeField] private AtmosphereFogView _atmosphereFogView;

        // Domain Registries & Models
        private BlockRegistry _blockRegistry;
        private ItemRegistry _itemRegistry;
        private RecipeBook _recipeBook;
        private DomainWorld _world;
        private VoxelRaycaster _raycaster;
        private Domain.Entities.Player _player;
        private DayNightCycle _dayNightCycle;
        private ChunkCoord _lastPlayerChunkCoord;
        private bool _hasInitialChunkLoad;
        private readonly Dictionary<BlockPos, BlockId> _modifiedBlocks = new Dictionary<BlockPos, BlockId>();
        private readonly Dictionary<Mob, MobView> _spawnedMobViews = new Dictionary<Mob, MobView>();
        private float _nightZombieSpawnTimer;

        // Application Services
        private ChunkLoadService _chunkLoadService;
        private BlockInteractionService _blockInteractionService;
        private CraftingService _craftingService;
        private SurvivalService _survivalService;
        private DeathDropService _deathDropService;
        private MobSpawnerService _mobSpawnerService;
        private MobCombatService _combatService;
        private GameModeService _gameModeService;
        private WorldPersistenceService _persistenceService;
        private ResourcePackService _resourcePackService;

        private void Awake()
        {
            InitializeApplication();
        }

        private void InitializeApplication()
        {
            // 1. Đọc cấu hình an toàn từ StreamingAssets (Infrastructure)
            string configPath = Path.Combine(UnityEngine.Application.streamingAssetsPath, "config.json");
            IConfigLoader configLoader = new ConfigLoader();
            GameConfigDto config = configLoader.LoadConfig(configPath);

            // 2. Khởi tạo danh mục dữ liệu gốc (Domain Registries)
            _blockRegistry = BlockRegistry.CreateDefault();
            _itemRegistry = ItemRegistry.CreateDefault(_blockRegistry);
            _recipeBook = RecipeBook.CreateDefault();

            // 3. Khởi tạo thế giới và sinh địa hình tự nhiên Perlin Noise (Domain World - Strategy Pattern)
            int seed = config.World.DefaultSeed;
            ITerrainGenerator generator = new PerlinTerrainGenerator(minHeight: 16, maxHeight: 38, scale: 0.05f);
            _world = new DomainWorld(generator, seed);
            _raycaster = new VoxelRaycaster(_world, _blockRegistry);

            // 4. Khởi tạo người chơi và chu kỳ ngày/đêm (Domain Entities & Environment)
            var spawnPos = new SysVector3(0f, 42f, 0f);
            var initialGameMode = new SurvivalMode();
            _player = new Domain.Entities.Player(spawnPos, initialGameMode, config.Player.MaxHealth, config.Player.MaxHunger);
            _dayNightCycle = new DayNightCycle(DayNightCycle.DefaultDayLengthSeconds, initialNormalizedTime: 0.1f);

            // 5. Khởi tạo các dịch vụ điều phối Use Case (Application Services)
            int renderDistance = Mathf.Clamp(config.World.RenderDistanceInChunks, 2, 6);
            _chunkLoadService = new ChunkLoadService(_world, renderDistance);
            _blockInteractionService = new BlockInteractionService(_world, _world, _blockRegistry, _itemRegistry);
            _craftingService = new CraftingService(_itemRegistry);
            _survivalService = new SurvivalService();
            _deathDropService = new DeathDropService();
            _combatService = new MobCombatService(attackRange: 2.0f);
            _gameModeService = new GameModeService();

            IMobFactory mobFactory = new MobFactory();
            _mobSpawnerService = new MobSpawnerService(mobFactory, _dayNightCycle, maxHostileMobs: 10, minSpawnDistance: 24.0f);

            IWorldRepository worldRepo = new JsonWorldRepository();
            _persistenceService = new WorldPersistenceService(worldRepo, _itemRegistry);

            // Khởi tạo hệ thống Resource Pack (Strategy Pattern + Fallback an toàn)
            string resourcePacksRoot = Path.Combine(UnityEngine.Application.streamingAssetsPath, "ResourcePacks");
            ITextureAtlasProvider fallbackProvider = new BuiltInAtlasProvider();
            ITextureAtlasProvider folderProvider = new FolderResourcePackProvider(resourcePacksRoot, fallbackProvider);
            string initialPackId = config.ResourcePack != null && !string.IsNullOrWhiteSpace(config.ResourcePack.ActivePack)
                ? config.ResourcePack.ActivePack
                : "OptimumRealism_64x";
            _resourcePackService = new ResourcePackService(folderProvider, initialPackId);

            // 6. Đăng ký lắng nghe sự kiện tử vong để rơi đồ ở chế độ Survival và hồi sinh (Quy tắc R9)
            _player.Health.Died += () =>
            {
                int droppedStacks = _deathDropService.DropInventoryOnDeath(_player);
                Debug.Log($"[DeathDrop] Người chơi tử vong! Đã làm rơi {droppedStacks} ngăn xếp vật phẩm và hồi sinh về SpawnPoint.");
            };

            // 7. Theo dõi các khối bị sửa đổi để phục vụ lưu thế giới
            _world.BlockChanged += args =>
            {
                _modifiedBlocks[args.Position] = args.NewBlock;
            };

            // 8. Cung cấp vật phẩm khởi đầu phong phú cho người chơi để thử nghiệm Sandbox & Xây dựng
            _player.Inventory.AddItem(_itemRegistry.Get(ItemId.Dirt), 32);
            _player.Inventory.AddItem(_itemRegistry.Get(ItemId.Wood), 16);
            _player.Inventory.AddItem(_itemRegistry.Get(ItemId.Bricks), 32);
            _player.Inventory.AddItem(_itemRegistry.Get(ItemId.Glass), 32);
            _player.Inventory.AddItem(_itemRegistry.Get(ItemId.Glowstone), 16);
            _player.Inventory.AddItem(_itemRegistry.Get(ItemId.Bookshelf), 16);
            _player.Inventory.AddItem(_itemRegistry.Get(ItemId.WoodenSword), 1);
            _player.Inventory.AddItem(_itemRegistry.Get(ItemId.Apple), 8);

            // 9. Tiêm phụ thuộc vào các Views hiển thị (Presentation Layer)
            if (_worldView != null)
            {
                _worldView.Initialize(_world, _blockRegistry);
            }

            if (_resourcePackView != null)
            {
                _resourcePackView.Initialize(_resourcePackService, _worldView);
            }

            if (_playerController != null)
            {
                _playerController.Initialize(_player.CurrentGameMode, _player.Inventory);
            }

            if (_interactionController != null)
            {
                _interactionController.Initialize(_player, _raycaster, _blockInteractionService, _survivalService, _combatService, _itemRegistry);
            }

            if (_hudView != null)
            {
                _hudView.Initialize(_player, _recipeBook, _craftingService, _blockRegistry, _itemRegistry, _dayNightCycle, _interactionController, _playerController);
            }

            if (_dayNightView != null)
            {
                _dayNightView.Initialize(_dayNightCycle);
            }

            if (_atmosphereFogView != null)
            {
                _atmosphereFogView.Initialize(_dayNightCycle);
            }

            // 10. Sinh và nạp lưới Chunk ban đầu quanh vị trí xuất phát của người chơi (UC02)
            UpdateStreamingChunks(forceUpdate: true);

            // 11. Sinh đàn Bò hiền lành ban đầu quanh đồng cỏ (UC12)
            SpawnInitialPassiveMobs();

            Debug.Log($"[GameBootstrap] Khởi tạo hệ thống thành công! Seed: {seed}, GameMode: {_player.CurrentGameMode.ModeName}, ResourcePack: {_resourcePackService.CurrentPack?.PackId}");
        }

        private void SpawnInitialPassiveMobs()
        {
            if (_mobSpawnerService == null)
            {
                return;
            }

            Vector2[] offsets =
            {
                new Vector2(6f, 8f),
                new Vector2(-7f, 9f),
                new Vector2(10f, -6f),
                new Vector2(-8f, -8f)
            };

            foreach (Vector2 off in offsets)
            {
                float surfaceY = FindSurfaceHeightAt(Mathf.RoundToInt(off.x), Mathf.RoundToInt(off.y));
                var spawnPos = new SysVector3(off.x, surfaceY, off.y);
                if (_mobSpawnerService.TrySpawnPassiveMob(spawnPos, out Mob cow))
                {
                    EnsureMobViewExists(cow);
                }
            }
        }

        private float FindSurfaceHeightAt(int worldX, int worldZ)
        {
            for (int y = Chunk.Height - 1; y >= 1; y--)
            {
                BlockId b = _world.GetBlock(new BlockPos(worldX, y, worldZ));
                if (b != BlockId.Air && b != BlockId.Leaves)
                {
                    return y + 1f;
                }
            }

            return 28f;
        }

        private void EnsureMobViewExists(Mob mob)
        {
            if (mob == null || _spawnedMobViews.ContainsKey(mob))
            {
                return;
            }

            GameObject mobGo = new GameObject($"Mob_{mob.GetType().Name}");
            MobView view = mobGo.AddComponent<MobView>();
            view.Initialize(mob);
            _spawnedMobViews.Add(mob, view);
        }

        private void SyncActiveMobViewsAndSurfaceSnap()
        {
            if (_mobSpawnerService == null)
            {
                return;
            }

            var active = _mobSpawnerService.ActiveMobs;
            for (int i = 0; i < active.Count; i++)
            {
                Mob mob = active[i];
                if (mob == null || !mob.IsAlive)
                {
                    continue;
                }

                float groundY = FindSurfaceHeightAt(Mathf.FloorToInt(mob.Position.X), Mathf.FloorToInt(mob.Position.Z));
                if (Mathf.Abs(mob.Position.Y - groundY) > 0.05f)
                {
                    mob.MoveTo(new SysVector3(mob.Position.X, groundY, mob.Position.Z));
                }

                EnsureMobViewExists(mob);
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            // Cập nhật nhịp sinh học, thu nhặt vật phẩm rơi và môi trường
            _dayNightCycle?.Tick(dt);
            _survivalService?.TickPlayerSurvival(_player, dt);
            _deathDropService?.Tick(dt);
            _deathDropService?.CollectNearbyDrops(_player, DeathDropService.DefaultPickupRadius);

            bool isTargetable = _player.CurrentGameMode.CanTakeDamage;
            _mobSpawnerService?.Tick(dt, _player.Position, isTargetable);

            // Tự động sinh Zombie vào ban đêm cách người chơi >= 25 block (Quy tắc R11)
            if (_dayNightCycle != null && _dayNightCycle.IsNight && _mobSpawnerService != null)
            {
                _nightZombieSpawnTimer += dt;
                if (_nightZombieSpawnTimer >= 4.0f)
                {
                    _nightZombieSpawnTimer = 0f;
                    float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
                    float dist = 26f;
                    float zx = _player.Position.X + Mathf.Cos(angle) * dist;
                    float zz = _player.Position.Z + Mathf.Sin(angle) * dist;
                    float zy = FindSurfaceHeightAt(Mathf.RoundToInt(zx), Mathf.RoundToInt(zz));
                    if (_mobSpawnerService.TrySpawnZombie(_player.Position, new SysVector3(zx, zy, zz), out Mob zombie))
                    {
                        EnsureMobViewExists(zombie);
                    }
                }
            }

            SyncActiveMobViewsAndSurfaceSnap();

            if (_mobSpawnerService != null && _combatService != null)
            {
                _combatService.ProcessHostileAttacks(_mobSpawnerService.ActiveMobs, _player);
            }

            // Tự động tải/gỡ Chunk khi người chơi di chuyển sang Chunk mới (UC02)
            UpdateStreamingChunks(forceUpdate: false);

            // Phím tắt thao tác nhanh (Quick test hotkeys)
            HandleDevHotkeys();
        }

        private void UpdateStreamingChunks(bool forceUpdate)
        {
            if (_chunkLoadService == null || _player == null)
            {
                return;
            }

            BlockPos playerBlockPos = new BlockPos(
                Mathf.FloorToInt(_player.Position.X),
                Mathf.FloorToInt(_player.Position.Y),
                Mathf.FloorToInt(_player.Position.Z));
            ChunkCoord currentChunk = playerBlockPos.ToChunkCoord(Chunk.Width);

            if (!forceUpdate && _hasInitialChunkLoad && currentChunk.Equals(_lastPlayerChunkCoord))
            {
                return;
            }

            _lastPlayerChunkCoord = currentChunk;
            _hasInitialChunkLoad = true;

            ChunkLoadResult result = _chunkLoadService.UpdateActiveChunks(currentChunk);
            if (_worldView != null)
            {
                foreach (ChunkCoord loaded in result.NewlyLoaded)
                {
                    _worldView.LoadChunkView(loaded);
                }

                foreach (ChunkCoord unloaded in result.Unloaded)
                {
                    _worldView.UnloadChunkView(unloaded);
                }
            }
        }

        private void HandleDevHotkeys()
        {
            // F1: Đổi vòng tròn giữa Survival -> Creative -> Adventure (State Pattern demo)
            if (Input.GetKeyDown(KeyCode.F1))
            {
                if (_player.CurrentGameMode is SurvivalMode)
                {
                    _gameModeService.SwitchToCreative(_player);
                }
                else if (_player.CurrentGameMode is CreativeMode)
                {
                    _gameModeService.SwitchToAdventure(_player);
                }
                else
                {
                    _gameModeService.SwitchToSurvival(_player);
                }

                if (_playerController != null)
                {
                    _playerController.Initialize(_player.CurrentGameMode, _player.Inventory);
                }

                Debug.Log($"[GameMode] Đã chuyển sang: {_player.CurrentGameMode.ModeName}");
            }

            // F2: Đổi nhanh Resource Pack giữa default (16x) và OptimumRealism_64x (Strategy + Observer Pattern demo)
            if (Input.GetKeyDown(KeyCode.F2) && _resourcePackService != null)
            {
                string currentId = _resourcePackService.CurrentPack != null
                    ? _resourcePackService.CurrentPack.PackId
                    : BuiltInAtlasProvider.DefaultPackId;

                string nextPackId = string.Equals(currentId, "OptimumRealism_64x", System.StringComparison.OrdinalIgnoreCase)
                    ? BuiltInAtlasProvider.DefaultPackId
                    : "OptimumRealism_64x";

                bool switched = _resourcePackService.SwitchPack(nextPackId);
                Debug.Log(switched
                    ? $"[ResourcePack] Đã chuyển sang gói: {_resourcePackService.CurrentPack.PackId} ({_resourcePackService.CurrentPack.TileResolution}x)"
                    : $"[ResourcePack] Chuyển gói '{nextPackId}' thất bại!");
            }

            // F5: Lưu nhanh thế giới (UC05 demo)
            if (Input.GetKeyDown(KeyCode.F5))
            {
                string savePath = Path.Combine(UnityEngine.Application.persistentDataPath, "quicksave.json");
                bool ok = _persistenceService.SaveGame(savePath, _world.Seed, "QuickSave", _world, _player, _modifiedBlocks);
                Debug.Log(ok ? $"[Save] Đã lưu thế giới vào: {savePath}" : "[Save] Lưu thế giới thất bại!");
            }

            // F9: Tải nhanh thế giới (UC06 demo)
            if (Input.GetKeyDown(KeyCode.F9))
            {
                string savePath = Path.Combine(UnityEngine.Application.persistentDataPath, "quicksave.json");
                bool ok = _persistenceService.LoadGame(savePath, _world, _player, out var restoredBlocks, out var loadedMode);
                if (ok)
                {
                    _modifiedBlocks.Clear();
                    foreach (var kvp in restoredBlocks)
                    {
                        _modifiedBlocks[kvp.Key] = kvp.Value;
                    }

                    if (_playerController != null)
                    {
                        _playerController.Initialize(_player.CurrentGameMode, _player.Inventory);
                    }

                    Debug.Log($"[Load] Đã nạp thế giới thành công! Chế độ: {loadedMode.ModeName}, Số khối khôi phục: {restoredBlocks.Count}");
                }
                else
                {
                    Debug.LogWarning("[Load] Nạp thế giới thất bại hoặc không tìm thấy file lưu!");
                }
            }
        }
    }
}
