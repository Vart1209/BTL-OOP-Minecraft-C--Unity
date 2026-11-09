using System;
using System.Collections.Generic;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.Crafting
{
    /// <summary>
    /// Sách công thức chế tạo lưu trữ và cung cấp toàn bộ các công thức hợp lệ trong trò chơi.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Quan hệ Kết tập (Aggregation - Tiêu chí 6):</b> <c>RecipeBook ◇ Recipe</c> — Sách công thức lưu trữ danh sách các công thức chế tạo, có thể thêm bớt công thức linh hoạt.</item>
    ///   <item><b>Dễ mở rộng (Open/Closed Principle - Tiêu chí 12):</b> Cho phép đăng ký thêm các công thức chế tạo mới (ví dụ: công cụ sắt, đuốc) mà không phải sửa logic xử lý túi đồ.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class RecipeBook
    {
        private readonly List<Recipe> _recipes = new List<Recipe>();

        /// <summary>Danh sách chỉ đọc các công thức hiện có trong sách.</summary>
        public IReadOnlyList<Recipe> Recipes => _recipes;

        /// <summary>
        /// Thêm một công thức chế tạo mới vào sách.
        /// </summary>
        public void AddRecipe(Recipe recipe)
        {
            if (recipe == null)
            {
                throw new ArgumentNullException(nameof(recipe));
            }

            _recipes.Add(recipe);
        }

        /// <summary>
        /// Khởi tạo sách công thức mặc định chứa 5 công thức cốt lõi của trò chơi.
        /// </summary>
        public static RecipeBook CreateDefault()
        {
            RecipeBook book = new RecipeBook();

            // 1. Chế tạo Ván gỗ: 1 Wood Log -> 4 Wood Planks
            book.AddRecipe(new Recipe(
                recipeId: "planks_from_wood",
                ingredients: new[] { new RecipeIngredient(ItemId.Wood, 1) },
                outputItemId: ItemId.Planks,
                outputCount: 4
            ));

            // 2. Chế tạo Bàn chế tạo: 4 Wood Planks -> 1 Crafting Table
            book.AddRecipe(new Recipe(
                recipeId: "crafting_table_from_planks",
                ingredients: new[] { new RecipeIngredient(ItemId.Planks, 4) },
                outputItemId: ItemId.CraftingTable,
                outputCount: 1
            ));

            // 3. Chế tạo Cúp gỗ: 3 Wood Planks + 2 Wood -> 1 Wooden Pickaxe
            book.AddRecipe(new Recipe(
                recipeId: "wooden_pickaxe",
                ingredients: new[]
                {
                    new RecipeIngredient(ItemId.Planks, 3),
                    new RecipeIngredient(ItemId.Wood, 2)
                },
                outputItemId: ItemId.WoodenPickaxe,
                outputCount: 1
            ));

            // 4. Chế tạo Kiếm gỗ: 2 Wood Planks + 1 Wood -> 1 Wooden Sword
            book.AddRecipe(new Recipe(
                recipeId: "wooden_sword",
                ingredients: new[]
                {
                    new RecipeIngredient(ItemId.Planks, 2),
                    new RecipeIngredient(ItemId.Wood, 1)
                },
                outputItemId: ItemId.WoodenSword,
                outputCount: 1
            ));

            // 5. Chế tạo Cúp đá: 3 Cobblestone + 2 Wood -> 1 Stone Pickaxe
            book.AddRecipe(new Recipe(
                recipeId: "stone_pickaxe",
                ingredients: new[]
                {
                    new RecipeIngredient(ItemId.Cobblestone, 3),
                    new RecipeIngredient(ItemId.Wood, 2)
                },
                outputItemId: ItemId.StonePickaxe,
                outputCount: 1
            ));

            return book;
        }
    }
}
