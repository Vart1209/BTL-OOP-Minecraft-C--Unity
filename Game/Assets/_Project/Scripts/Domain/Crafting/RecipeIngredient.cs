using System;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.Crafting
{
    /// <summary>
    /// Cấu trúc dữ liệu bất biến (Immutable Value Object) đại diện cho một loại nguyên liệu và số lượng cần thiết trong công thức chế tạo.
    /// <para>
    /// <b>Nguyên lý OOP áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Đóng gói &amp; Bất biến (Encapsulation &amp; Immutability - Tiêu chí 13):</b> Dùng <c>readonly struct</c> với các trường <c>private readonly</c>, bảo vệ tính toàn vẹn của dữ liệu công thức.</item>
    ///   <item><b>Kiểm tra hợp lệ dữ liệu (Tiêu chí 13):</b> Số lượng nguyên liệu yêu cầu phải &gt; 0.</item>
    /// </list>
    /// </para>
    /// </summary>
    public readonly struct RecipeIngredient : IEquatable<RecipeIngredient>
    {
        private readonly ItemId _itemId;
        private readonly int _requiredCount;

        /// <summary>Mã định danh vật phẩm nguyên liệu yêu cầu.</summary>
        public ItemId ItemId => _itemId;

        /// <summary>Số lượng nguyên liệu yêu cầu (> 0).</summary>
        public int RequiredCount => _requiredCount;

        /// <summary>
        /// Khởi tạo một thành phần nguyên liệu cho công thức chế tạo.
        /// </summary>
        public RecipeIngredient(ItemId itemId, int requiredCount = 1)
        {
            if (requiredCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(requiredCount), "Số lượng nguyên liệu yêu cầu phải > 0.");
            }

            _itemId = itemId;
            _requiredCount = requiredCount;
        }

        /// <inheritdoc />
        public bool Equals(RecipeIngredient other)
        {
            return _itemId == other._itemId && _requiredCount == other._requiredCount;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is RecipeIngredient other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(_itemId, _requiredCount);
        }

        public static bool operator ==(RecipeIngredient left, RecipeIngredient right) => left.Equals(right);

        public static bool operator !=(RecipeIngredient left, RecipeIngredient right) => !left.Equals(right);
    }
}
