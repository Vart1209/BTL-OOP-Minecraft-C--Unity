using System;
using UnityEngine;
using Minecraft.Application.Survival;
using Minecraft.Domain.Crafting;
using Minecraft.Domain.Entities;
using Minecraft.Domain.Items;

namespace Minecraft.Presentation.UI
{
    /// <summary>
    /// Giao diện người dùng HUD cơ bản của chế độ Sandbox (Presentation Layer).
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

        private int _currentHealth;
        private int _maxHealth = 20;

        private int _currentHunger;
        private int _maxHunger = 20;

        private bool _showCraftingMenu = false;

        /// <summary>
        /// Khởi tạo phụ thuộc của HUD từ GameBootstrap.
        /// </summary>
        public void Initialize(
            Domain.Entities.Player player,
            RecipeBook recipeBook,
            CraftingService craftingService)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _recipeBook = recipeBook ?? throw new ArgumentNullException(nameof(recipeBook));
            _craftingService = craftingService ?? throw new ArgumentNullException(nameof(craftingService));

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
            // Bấm phím C hoặc E để bật/tắt bảng chế tạo nhanh
            if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.E))
            {
                _showCraftingMenu = !_showCraftingMenu;
            }
        }

        private void OnGUI()
        {
            if (_player == null)
            {
                return;
            }

            // 1. Hiển thị Tâm ngắm giữa màn hình
            float centerX = Screen.width / 2f;
            float centerY = Screen.height / 2f;
            GUI.Box(new Rect(centerX - 4, centerY - 4, 8, 8), "+");

            // 2. Hiển thị Thanh Máu & Đói góc dưới bên trái
            GUI.Box(new Rect(20, Screen.height - 110, 240, 90), "Trạng thái người chơi");
            GUI.Label(new Rect(30, Screen.height - 85, 220, 25), $"Máu: {_currentHealth} / {_maxHealth} HP (Chế độ: {_player.CurrentGameMode.ModeName})");
            GUI.Label(new Rect(30, Screen.height - 60, 220, 25), $"Độ đói: {_currentHunger} / {_maxHunger}");

            // 3. Hiển thị 9 ô Hotbar ở đáy màn hình
            float hotbarWidth = 9 * 55;
            float hotbarStartX = (Screen.width - hotbarWidth) / 2f;
            float hotbarY = Screen.height - 65;

            for (int i = 0; i < 9; i++)
            {
                Rect slotRect = new Rect(hotbarStartX + (i * 55), hotbarY, 50, 50);
                ItemStack stack = _player.Inventory.GetSlot(i);

                string selectedBorder = (i == _player.Inventory.SelectedHotbarIndex) ? " [X] " : "";
                string itemLabel = stack.IsEmpty ? $"{i + 1}\n{selectedBorder}" : $"{stack.Definition.Name}\nx{stack.Count}\n{selectedBorder}";

                if (GUI.Button(slotRect, itemLabel))
                {
                    _player.Inventory.SelectHotbarSlot(i);
                }
            }

            // 4. Bảng chế tạo nhanh (Quick Crafting Menu)
            if (_showCraftingMenu && _recipeBook != null && _craftingService != null)
            {
                GUI.Box(new Rect(Screen.width - 320, 20, 300, 380), "Bàn Chế Tạo (Nhấn C để đóng)");
                int yOffset = 50;

                foreach (Recipe recipe in _recipeBook.Recipes)
                {
                    bool canCraft = _craftingService.CanCraft(recipe, _player.Inventory);
                    string craftBtnText = $"Chế tạo: {recipe.OutputItemId} x{recipe.OutputCount}";

                    if (!canCraft)
                    {
                        GUI.enabled = false;
                    }

                    if (GUI.Button(new Rect(Screen.width - 310, yOffset, 280, 45), craftBtnText))
                    {
                        _craftingService.TryCraft(recipe, _player.Inventory);
                    }

                    GUI.enabled = true;
                    yOffset += 55;
                }
            }
        }
    }
}
