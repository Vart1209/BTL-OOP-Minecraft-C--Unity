using System;
using Minecraft.Domain.Crafting;
using Minecraft.Domain.Items;

namespace Minecraft.Application.Survival
{
    /// <summary>
    /// Dịch vụ ứng dụng điều phối Use Case UC09 (Chế tạo vật phẩm theo công thức).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc phân tầng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Application Service (Tiêu chí 4, 8):</b> Điều phối luồng kiểm tra và trừ nguyên liệu túi đồ, thêm thành phẩm mà không phụ thuộc vào UI Presentation.</item>
    ///   <item><b>Quy tắc R10 (Tiêu chí 3):</b> Cơ chế giao dịch nguyên tử ("đủ hết hoặc không làm gì" - Atomic Transaction): Nếu thiếu bất kỳ nguyên liệu nào trong công thức, phương thức <see cref="TryCraft"/> lập tức trả về <c>false</c> mà không trừ bất kỳ món nào.</item>
    ///   <item><b>Dependency Injection:</b> Nhận <see cref="ItemRegistry"/> qua constructor để tạo ra đúng thực thể thành phẩm tương ứng.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class CraftingService
    {
        private readonly ItemRegistry _itemRegistry;

        /// <summary>
        /// Khởi tạo dịch vụ chế tạo.
        /// </summary>
        public CraftingService(ItemRegistry itemRegistry)
        {
            _itemRegistry = itemRegistry ?? throw new ArgumentNullException(nameof(itemRegistry));
        }

        /// <summary>
        /// Kiểm tra xem túi đồ của người chơi có đủ toàn bộ nguyên liệu để chế tạo công thức hay không.
        /// </summary>
        public bool CanCraft(Recipe recipe, Inventory inventory)
        {
            if (recipe == null || inventory == null)
            {
                return false;
            }

            if (!_itemRegistry.Contains(recipe.OutputItemId))
            {
                return false;
            }

            // Kiểm tra từng nguyên liệu có đủ số lượng yêu cầu trong túi đồ
            foreach (RecipeIngredient ingredient in recipe.Ingredients)
            {
                if (inventory.CountItem(ingredient.ItemId) < ingredient.RequiredCount)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Thực hiện chế tạo vật phẩm theo Quy tắc R10 (giao dịch nguyên tử).
        /// </summary>
        /// <param name="recipe">Công thức chế tạo được chọn.</param>
        /// <param name="inventory">Túi đồ của người chơi.</param>
        /// <returns><c>true</c> nếu chế tạo thành công; <c>false</c> nếu không đủ nguyên liệu (không trừ món nào).</returns>
        public bool TryCraft(Recipe recipe, Inventory inventory)
        {
            if (!CanCraft(recipe, inventory))
            {
                return false;
            }

            // Quy tắc R10: Đảm bảo trừ đủ tất cả nguyên liệu
            foreach (RecipeIngredient ingredient in recipe.Ingredients)
            {
                inventory.RemoveItem(ingredient.ItemId, ingredient.RequiredCount);
            }

            // Thêm thành phẩm vào túi đồ
            Item outputItem = _itemRegistry.Get(recipe.OutputItemId);
            inventory.AddItem(outputItem, recipe.OutputCount);

            return true;
        }
    }
}
