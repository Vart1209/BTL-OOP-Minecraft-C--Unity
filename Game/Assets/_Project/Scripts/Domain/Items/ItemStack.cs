using System;

namespace Minecraft.Domain.Items
{
    /// <summary>
    /// Đối tượng thể hiện một ngăn xếp các vật phẩm cùng loại kèm theo số lượng.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Quan hệ Kết tập (Aggregation - Tiêu chí 6):</b> <c>ItemStack ◇ Item</c> — <see cref="ItemStack"/> giữ tham chiếu tới định nghĩa <see cref="Item"/> và số lượng hiện có mà không sở hữu vòng đời của đối tượng <see cref="Item"/>.</item>
    ///   <item><b>Đóng gói (Encapsulation - Tiêu chí 13):</b> Số lượng <c>_count</c> là <c>private</c>, kiểm soát qua <see cref="Add"/> và <see cref="Remove"/> để không bao giờ vượt quá <see cref="Item.MaxStackSize"/> hoặc bị âm.</item>
    ///   <item><b>Quy tắc R6 (Tiêu chí 3):</b> Giới hạn số lượng xếp chồng không vượt quá giới hạn tối đa của vật phẩm.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class ItemStack
    {
        private readonly Item _definition;
        private int _count;

        /// <summary>Định nghĩa thuộc tính vật phẩm được chứa trong ngăn xếp.</summary>
        public Item Definition => _definition;

        /// <summary>Số lượng vật phẩm hiện có trong ngăn xếp.</summary>
        public int Count => _count;

        /// <summary>Ngăn xếp có đang trống (rỗng) hay không.</summary>
        public bool IsEmpty => _count <= 0 || _definition == null;

        /// <summary>Đối tượng ngăn xếp rỗng dùng chung.</summary>
        public static ItemStack Empty => new ItemStack(null, 0);

        /// <summary>
        /// Khởi tạo một ngăn xếp vật phẩm.
        /// </summary>
        /// <param name="definition">Định nghĩa loại vật phẩm.</param>
        /// <param name="count">Số lượng ban đầu.</param>
        public ItemStack(Item definition, int count)
        {
            if (definition == null || count <= 0)
            {
                _definition = null;
                _count = 0;
                return;
            }

            if (count > definition.MaxStackSize)
            {
                throw new ArgumentOutOfRangeException(nameof(count), $"Số lượng ({count}) vượt quá MaxStackSize ({definition.MaxStackSize}) của vật phẩm '{definition.Name}'.");
            }

            _definition = definition;
            _count = count;
        }

        /// <summary>
        /// Thêm số lượng vật phẩm vào ngăn xếp hiện tại.
        /// </summary>
        /// <param name="amount">Số lượng cần thêm vào (> 0).</param>
        /// <returns>Số lượng dư thừa không thể chứa (nếu chạm mốc MaxStackSize).</returns>
        public int Add(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Số lượng thêm vào phải > 0.");
            }

            if (IsEmpty)
            {
                return amount;
            }

            int spaceLeft = _definition.MaxStackSize - _count;
            int added = Math.Min(spaceLeft, amount);
            _count += added;

            return amount - added;
        }

        /// <summary>
        /// Lấy bớt một số lượng vật phẩm ra khỏi ngăn xếp.
        /// </summary>
        /// <param name="amount">Số lượng cần lấy ra (> 0).</param>
        /// <returns>Số lượng thực tế đã lấy ra.</returns>
        public int Remove(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Số lượng lấy ra phải > 0.");
            }

            int removed = Math.Min(_count, amount);
            _count -= removed;

            return removed;
        }
    }
}
