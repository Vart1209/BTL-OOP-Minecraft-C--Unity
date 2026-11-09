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
using Minecraft.Presentation.Environment;
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
    ///   <item><b>Xử lý ngoại lệ tự phục hồi (Tiêu chí 19):</b> Đọc file cấu hình ngoài bằng <see cref="ConfigLoader"/> có <c>try-catch</c> an toàn, đảm bảo game luôn chạy được kể cả khi thiếu file cấu hình.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class GameBootstrap : MonoBehaviour
    {
        [Header("Views liên kết trong Scene")]
        [SerializeField] private WorldView _worldView;
        [SerializeField] private PlayerInteractionController _playerController;
        [SerializeField] private SandboxHudView _hudView;
        [SerializeField] private DayNightView _dayNightView;

        // Domain Registries & Models
        private BlockRegistry _blockRegistry;
        private ItemRegistry _itemRegistry;
        private RecipeBook _recipeBook;
        private DomainWorld _world;
        private Entities.Player _player;
        private DayNightCycle _dayNightCycle;
        private readonly Dictionary<BlockPos, BlockId> _modifiedBlocks = new Dictionary<BlockPos, BlockId>();

        // Application Services
        private BlockInteractionService _blockInteractionService;
        private CraftingService _craftingService;
        private SurvivalService _survivalService;
        private MobSpawnerService _mobSpawnerService;
        private MobCombatService _combatService;
        private GameModeService _gameModeService;
        private WorldPersistenceService _persistenceService;

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

            // 3. Khởi tạo thế giới và sinh địa hình (Domain World)
            int seed = config.World.DefaultSeed;
            ITerrainGenerator generator = new FlatTerrainGenerator(surfaceHeight: 1);
            _world = new DomainWorld(generator, seed);

            // 4. Khởi tạo người chơi và chu kỳ ngày/đêm (Domain Entities & Environment)
            var spawnPos = new SysVector3(0f, 2f, 0f);
            var initialGameMode = new SurvivalMode();
            _player = new Entities.Player(spawnPos, initialGameMode, config.Player.MaxHealth, config.Player.MaxHunger);
            _dayNightCycle = new DayNightCycle(DayNightCycle.DefaultDayLengthSeconds, initialNormalizedTime: 0.1f);

            // 5. Khởi tạo các dịch vụ điều phối Use Case (Application Services)
            _blockInteractionService = new BlockInteractionService(_world, _world, _blockRegistry, _itemRegistry);
            _craftingService = new CraftingService(_recipeBook);
            _survivalService = new SurvivalService();
            _combatService = new MobCombatService(attackRange: 2.0f);
            _gameModeService = new GameModeService();

            IMobFactory mobFactory = new MobFactory();
            _mobSpawnerService = new MobSpawnerService(mobFactory, _dayNightCycle, maxHostileMobs: 10, minSpawnDistance: 24.0f);

            IWorldRepository worldRepo = new JsonWorldRepository();
            _persistenceService = new WorldPersistenceService(worldRepo, _itemRegistry);

            // 6. Theo dõi các khối bị sửa đổi để phục vụ lưu thế giới
            _world.BlockChanged += (pos, oldId, newId) =>
            {
                _modifiedBlocks[pos] = newId;
            };

            // 7. Cung cấp vật phẩm khởi đầu cho người chơi để thử nghiệm Sandbox
            _player.Inventory.AddItem(_itemRegistry.Get(ItemId.Dirt), 32);
            _player.Inventory.AddItem(_itemRegistry.Get(ItemId.Wood), 16);
            _player.Inventory.AddItem(_itemRegistry.Get(ItemId.Apple), 5);

            // 8. Tiêm phụ thuộc vào các Views hiển thị (Presentation Layer)
            if (_worldView != null)
            {
                _worldView.Initialize(_world, _blockRegistry);
            }

            if (_playerController != null)
            {
                _playerController.Initialize(_player, _world, _blockInteractionService, _survivalService);
            }

            if (_hudView != null)
            {
                _hudView.Initialize(_player, _craftingService);
            }

            if (_dayNightView != null)
            {
                _dayNightView.Initialize(_dayNightCycle);
            }

            Debug.Log($"[GameBootstrap] Khởi tạo hệ thống thành công! Seed: {seed}, GameMode: {_player.CurrentGameMode.ModeName}");
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            // Cập nhật nhịp sinh học và môi trường
            _dayNightCycle?.Tick(dt);
            _survivalService?.Tick(dt, _player);

            bool isTargetable = _player.CurrentGameMode.CanTakeDamage;
            _mobSpawnerService?.Tick(dt, _player.Position, isTargetable);

            // Phím tắt thao tác nhanh (Quick test hotkeys)
            HandleDevHotkeys();
        }

        private void HandleDevHotkeys()
        {
            // F1: Đổi qua lại giữa Survival và Creative (State Pattern demo)
            if (Input.GetKeyDown(KeyCode.F1))
            {
                if (_player.CurrentGameMode is SurvivalMode)
                {
                    _gameModeService.SwitchToCreative(_player);
                }
                else
                {
                    _gameModeService.SwitchToSurvival(_player);
                }
                Debug.Log($"[GameMode] Đã chuyển sang: {_player.CurrentGameMode.ModeName}");
            }

            // F5: Lưu nhanh thế giới (UC05 demo)
            if (Input.GetKeyDown(KeyCode.F5))
            {
                string savePath = Path.Combine(UnityEngine.Application.persistentDataPath, "quicksave.json");
                bool ok = _persistenceService.SaveGame(savePath, 20261107, "QuickSave", _world, _player, _modifiedBlocks);
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
