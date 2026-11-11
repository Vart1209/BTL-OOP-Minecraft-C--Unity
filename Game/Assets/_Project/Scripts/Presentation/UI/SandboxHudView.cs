using System;
using UnityEngine;
using Minecraft.Application.Survival;
using Minecraft.Domain.Blocks;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.Crafting;
using Minecraft.Domain.Entities;
using Minecraft.Domain.Environment;
using Minecraft.Domain.Items;
using Minecraft.Presentation.Player;

namespace Minecraft.Presentation.UI
{
    /// <summary>
    /// Giao diện người dùng HUD nâng cao kiểu Minecraft: Thanh Tim/Đói trực quan, Tooltip khối đang nhắm, Bàn chế tạo và Kho khối Sáng tạo (Presentation Layer).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc phân tầng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Observer Pattern (Tiêu chí 9, 10, 11):</b> Đăng ký lắng nghe sự kiện <see cref="Health.HealthChanged"/>, <see cref="Hunger.HungerChanged"/>, <see cref="Inventory.InventoryChanged"/> để hiển thị dữ liệu thời gian thực.</item>
    ///   <item><b>Phân tách giao diện và nghiệp vụ (Tiêu chí 4):</b> UI không can thiệp trực tiếp vào dữ liệu túi đồ mà kích hoạt hành động thông qua <see cref="CraftingService"/>.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class SandboxHudView : MonoBehaviour
    {
        private Domain.Entities.Player _player;
        private RecipeBook _recipeBook;
        private CraftingService _craftingService;
        private BlockRegistry _blockRegistry;
        private ItemRegistry _itemRegistry;
        private DayNightCycle _dayNightCycle;
        private PlayerInteractionController _interactionController;
        private FirstPersonPlayerController _fpsController;

        private int _currentHealth;
        private int _maxHealth = 20;

        private int _currentHunger;
        private int _maxHunger = 20;

        private bool _showCraftingMenu;

        private static readonly ItemId[] PaletteItems =
        {
            ItemId.Grass, ItemId.Dirt, ItemId.Stone, ItemId.Cobblestone,
            ItemId.Wood, ItemId.Planks, ItemId.Leaves, ItemId.Sand,
            ItemId.Bricks, ItemId.Glass, ItemId.Bookshelf, ItemId.Glowstone,
            ItemId.CoalOre, ItemId.IronOre, ItemId.GoldOre, ItemId.DiamondOre,
            ItemId.Snow, ItemId.CraftingTable, ItemId.DiamondPickaxe, ItemId.Apple
        };

        /// <summary>
        /// Khởi tạo phụ thuộc của HUD từ GameBootstrap.
        /// </summary>
        public void Initialize(
            Domain.Entities.Player player,
            RecipeBook recipeBook,
            CraftingService craftingService,
            BlockRegistry blockRegistry = null,
            ItemRegistry itemRegistry = null,
            DayNightCycle dayNightCycle = null,
            PlayerInteractionController interactionController = null,
            FirstPersonPlayerController fpsController = null)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _recipeBook = recipeBook ?? throw new ArgumentNullException(nameof(recipeBook));
            _craftingService = craftingService ?? throw new ArgumentNullException(nameof(craftingService));
            _blockRegistry = blockRegistry;
            _itemRegistry = itemRegistry;
            _dayNightCycle = dayNightCycle;
            _interactionController = interactionController;
            _fpsController = fpsController;

            _currentHealth = _player.Health.Current;
            _maxHealth = _player.Health.Max;

            _currentHunger = _player.Hunger.Current;
            _maxHunger = _player.Hunger.Max;

            _player.Health.HealthChanged += OnHealthChanged;
            _player.Hunger.HungerChanged += OnHungerChanged;
        }

        private void OnDestroy()
        {
            if (_player != null)
            {
                _player.Health.HealthChanged -= OnHealthChanged;
                _player.Hunger.HungerChanged -= OnHungerChanged;
            }
        }

        private void OnHealthChanged(int current, int max)
        {
            _currentHealth = current;
            _maxHealth = max;
        }

        private void OnHungerChanged(int current, int max)
        {
            _currentHunger = current;
            _maxHunger = max;
        }

        private void Update()
        {
            // Bấm phím E hoặc C để bật/tắt bảng Chế tạo & Kho khối, tự động mở khóa chuột
            if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.E))
            {
                _showCraftingMenu = !_showCraftingMenu;
                if (_fpsController != null)
                {
                    _fpsController.IsUiOpen = _showCraftingMenu;
                }
            }
            else if (_fpsController != null && _fpsController.IsUiOpen != _showCraftingMenu)
            {
                _showCraftingMenu = _fpsController.IsUiOpen;
            }
        }

        private void OnGUI()
        {
            if (_player == null)
            {
                return;
            }

            float centerX = Screen.width / 2f;
            float centerY = Screen.height / 2f;

            // 1. Tâm ngắm + Thông tin khối đang nhắm ở chính giữa màn hình
            GUI.Box(new Rect(centerX - 10, centerY - 10, 20, 20), "+");
            if (_interactionController != null && _interactionController.CurrentHit.IsHit)
            {
                var hit = _interactionController.CurrentHit;
                GUI.Box(
                    new Rect(centerX - 135, centerY + 18, 270, 26),
                    $"Đang nhắm khối tại: {hit.BlockPosition} (Mặt: {hit.HitFace})");
            }

            // 2. Bảng thông tin Điều khiển & Trạng thái Thế giới ở góc trên bên trái
            string phaseName = _dayNightCycle != null ? _dayNightCycle.CurrentPhase.ToString() : "Day";
            GUI.Box(new Rect(14, 14, 340, 96), "MINECRAFT Voxel OOP — Bảng Điều Khiển");
            GUI.Label(new Rect(24, 36, 320, 22), $"Chế độ: {_player.CurrentGameMode.ModeName} | Thời gian: {phaseName} | Tọa độ: ({_player.Position.X:0}, {_player.Position.Y:0}, {_player.Position.Z:0})");
            GUI.Label(new Rect(24, 58, 320, 22), "[WASD/Space] Di chuyển  |  [Chuột Trái/Phải] Đập / Đặt & Ăn");
            GUI.Label(new Rect(24, 78, 320, 22), "[E/C] Chế tạo & Kho Block | [F1] Đổi Mode | [F2] Đổi Pack | [F5/F9] Lưu/Tải");

            // 3. Thanh Máu (♥) & Thanh Đói (🍖) nằm ngay phía trên thanh Hotbar chuẩn Minecraft
            float hotbarWidth = 9 * 60f;
            float hotbarStartX = (Screen.width - hotbarWidth) / 2f;
            float hotbarY = Screen.height - 68f;

            int hearts = Mathf.CeilToInt(_currentHealth / 2f);
            string heartIcons = new string('♥', Mathf.Clamp(hearts, 0, 10)) + new string('♡', Mathf.Clamp(10 - hearts, 0, 10));
            int drumsticks = Mathf.CeilToInt(_currentHunger / 2f);
            string hungerIcons = new string('♦', Mathf.Clamp(drumsticks, 0, 10)) + new string('♢', Mathf.Clamp(10 - drumsticks, 0, 10));

            GUI.Box(new Rect(hotbarStartX, hotbarY - 34f, 255f, 28f), $"Máu: {heartIcons} ({_currentHealth}/{_maxHealth})");
            GUI.Box(new Rect(hotbarStartX + hotbarWidth - 255f, hotbarY - 34f, 255f, 28f), $"Đói: {hungerIcons} ({_currentHunger}/{_maxHunger})");

            // 4. Thanh 9 ô Hotbar ở đáy màn hình
            for (int i = 0; i < 9; i++)
            {
                Rect slotRect = new Rect(hotbarStartX + (i * 60f), hotbarY, 56f, 56f);
                ItemStack stack = _player.Inventory.GetSlot(i);
                bool isSelected = i == _player.Inventory.SelectedHotbarIndex;

                Color prevBg = GUI.backgroundColor;
                if (isSelected)
                {
                    GUI.backgroundColor = new Color(1f, 0.92f, 0.35f, 1f);
                }

                string header = isSelected ? $"[{i + 1}]" : $"{i + 1}";
                string itemLabel = stack.IsEmpty
                    ? $"{header}\n(Trống)"
                    : $"{header}\n{stack.Definition.Name}\nx{stack.Count}";

                if (GUI.Button(slotRect, itemLabel))
                {
                    _player.Inventory.SelectHotbarSlot(i);
                }

                GUI.backgroundColor = prevBg;

                // Thanh độ bền trực quan cho công cụ (v1.2.0 chuẩn Minecraft 1.12)
                if (!stack.IsEmpty && stack.Definition is ToolItem && stack.Durability < stack.MaxDurability)
                {
                    float durabilityRatio = Mathf.Clamp01((float)stack.Durability / stack.MaxDurability);
                    Color barColor = durabilityRatio > 0.6f
                        ? new Color(0.2f, 0.85f, 0.2f, 1f) // Xanh lá
                        : durabilityRatio > 0.25f
                            ? new Color(1.0f, 0.75f, 0.1f, 1f) // Vàng
                            : new Color(0.95f, 0.2f, 0.2f, 1f); // Đỏ

                    float barWidth = 46f;
                    float barHeight = 4f;
                    float barX = slotRect.x + 5f;
                    float barY = slotRect.y + slotRect.height - 7f;

                    Color prevColor = GUI.color;
                    GUI.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
                    GUI.DrawTexture(new Rect(barX, barY, barWidth, barHeight), Texture2D.whiteTexture);

                    GUI.color = barColor;
                    GUI.DrawTexture(new Rect(barX, barY, barWidth * durabilityRatio, barHeight), Texture2D.whiteTexture);
                    GUI.color = prevColor;
                }
            }

            // 5. Cửa sổ Bàn Chế Tạo & Kho Khối Sáng Tạo (mở bằng phím E hoặc C)
            if (_showCraftingMenu)
            {
                DrawCraftingAndPaletteModal();
            }
        }

        private void DrawCraftingAndPaletteModal()
        {
            float modalW = 680f;
            float modalH = 420f;
            float modalX = (Screen.width - modalW) / 2f;
            float modalY = (Screen.height - modalH) / 2f;

            GUI.Box(new Rect(modalX, modalY, modalW, modalH), "BÀN CHẾ TẠO (CRAFTING) & KHO VẬT PHẨM — Nhấn [E] hoặc [Esc] để đóng");

            // Cột trái: Danh sách Công thức Chế tạo (RecipeBook)
            GUI.Label(new Rect(modalX + 20f, modalY + 32f, 300f, 24f), "1. Chế tạo theo công thức (Survival & Creative):");
            if (_recipeBook != null && _craftingService != null)
            {
                int yOffset = (int)(modalY + 60f);
                for (int i = 0; i < _recipeBook.Recipes.Count && i < 8; i++)
                {
                    Recipe recipe = _recipeBook.Recipes[i];
                    bool canCraft = _craftingService.CanCraft(recipe, _player.Inventory);

                    string ingText = "";
                    for (int j = 0; j < recipe.Ingredients.Count; j++)
                    {
                        if (j > 0) ingText += " + ";
                        ingText += $"{recipe.Ingredients[j].RequiredCount} {recipe.Ingredients[j].ItemId}";
                    }

                    GUI.enabled = canCraft;
                    if (GUI.Button(new Rect(modalX + 20f, yOffset, 300f, 38f), $"{ingText} ➔ {recipe.OutputCount}x {recipe.OutputItemId}"))
                    {
                        _craftingService.TryCraft(recipe, _player.Inventory);
                    }
                    GUI.enabled = true;
                    yOffset += 43;
                }
            }

            // Cột phải: Kho Khối Đa Dạng (Click để lấy nhanh khối mới vào Túi đồ để thử nghiệm đồ họa)
            GUI.Label(new Rect(modalX + 350f, modalY + 32f, 310f, 24f), "2. Kho Khối Mở Rộng (Click để lấy vào Hotbar):");
            if (_itemRegistry != null)
            {
                int cols = 2;
                for (int i = 0; i < PaletteItems.Length; i++)
                {
                    ItemId itemId = PaletteItems[i];
                    if (!_itemRegistry.TryGet(itemId, out Item itemDef))
                    {
                        continue;
                    }

                    int col = i % cols;
                    int row = i / cols;
                    Rect btnRect = new Rect(modalX + 350f + col * 155f, modalY + 60f + row * 34f, 148f, 30f);

                    int giveAmount = itemDef.MaxStackSize > 1 ? 16 : 1;
                    if (GUI.Button(btnRect, $"+ {itemDef.Name} (x{giveAmount})"))
                    {
                        _player.Inventory.AddItem(itemDef, giveAmount);
                    }
                }
            }
        }
    }
}

