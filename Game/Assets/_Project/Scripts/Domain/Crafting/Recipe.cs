using System;
using System.Collections.Generic;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.Crafting
{
    /// <summary>
    /// Thực thể định nghĩa một công thức chế tạo (Recipe) từ danh sách nguyên liệu đầu vào ra sản phẩm đầu ra.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Đóng gói (Encapsulation - Tiêu chí 13):</b> Toàn bộ trường dữ liệu đều là <c>private readonly</c>, bảo vệ danh sách nguyên liệu và kết quả đầu ra.</item>
    ///   <item><b>Kiểm tra hợp lệ dữ liệu (Tiêu chí 13):</b> Kiểm tra mã công thức không rỗng, danh sách nguyên liệu không rỗng và số lượng sản phẩm đầu ra &gt; 0.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class Recipe
    {
        private readonly string _recipeId;
        private readonly IReadOnlyList<RecipeIngredient> _ingredients;
        private readonly ItemId _outputItemId;
        private readonly int _outputCount;

        /// <summary>Mã định danh duy nhất của công thức chế tạo.</summary>
        public string RecipeId => _recipeId;

        /// <summary>Danh sách các nguyên liệu yêu cầu để chế tạo.</summary>
        public IReadOnlyList<RecipeIngredient> Ingredients => _ingredients;

        /// <summary>Mã vật phẩm thành phẩm tạo ra.</summary>
        public ItemId OutputItemId => _outputItemId;

        /// <summary>Số lượng thành phẩm tạo ra (> 0).</summary>
        public int OutputCount => _outputCount;

        /// <summary>
        /// Khởi tạo một công thức chế tạo mới.
        /// </summary>
        public Recipe(
            string recipeId,
            IReadOnlyList<RecipeIngredient> ingredients,
            ItemId outputItemId,
            int outputCount = 1)
        {
            if (string.IsNullOrWhiteSpace(recipeId))
            {
                throw new ArgumentException("Mã công thức chế tạo không được để trống.", nameof(recipeId));
            }

            if (ingredients == null || ingredients.Count == 0)
            {
                throw new ArgumentException("Công thức chế tạo phải có ít nhất một nguyên liệu.", nameof(ingredients));
            }

            if (outputCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(outputCount), "Số lượng sản phẩm đầu ra phải > 0.");
            }

            _recipeId = recipeId;
            _ingredients = ingredients;
            _outputItemId = outputItemId;
            _outputCount = outputCount;
        }
    }
}
