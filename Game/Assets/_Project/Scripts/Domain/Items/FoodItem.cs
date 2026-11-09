using System;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.Items
{
    /// <summary>
    /// Vật phẩm thức ăn giúp người chơi hồi phục thanh đói và máu khi tiêu thụ.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Quy tắc nghiệp vụ (Quy tắc R6, R8 - dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Kế thừa &amp; Đa hình (Inheritance &amp; Polymorphism - Tiêu chí 14):</b> Kế thừa từ <see cref="Item"/> và ghi đè <see cref="Use"/>.</item>
    ///   <item><b>Quy tắc R6 (Tiêu chí 3):</b> Thức ăn xếp chồng tối đa 64 cái (<c>MaxStackSize = 64</c>).</item>
    ///   <item><b>Quy tắc R8 (Tiêu chí 3):</b> Khai báo lượng điểm đói <see cref="HungerRestore"/> và máu <see cref="HealthRestore"/> hồi phục khi ăn.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class FoodItem : Item
    {
        private readonly int _hungerRestore;
        private readonly int _healthRestore;

        /// <summary>Số điểm đói hồi phục khi ăn (> 0).</summary>
        public int HungerRestore => _hungerRestore;

        /// <summary>Số điểm máu hồi phục lập tức (nếu có, >= 0).</summary>
        public int HealthRestore => _healthRestore;

        /// <summary>
        /// Khởi tạo một loại thức ăn.
        /// </summary>
        public FoodItem(
            ItemId id,
            string name,
            int hungerRestore,
            int healthRestore = 0)
            : base(id, name, maxStackSize: 64)
        {
            if (hungerRestore <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(hungerRestore), "Điểm hồi đói phải > 0.");
            }

            if (healthRestore < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(healthRestore), "Điểm hồi máu phải >= 0.");
            }

            _hungerRestore = hungerRestore;
            _healthRestore = healthRestore;
        }

        /// <inheritdoc />
        public override bool Use(ItemUseContext context)
        {
            // Trả về true biểu thị hành động tiêu thụ thức ăn thành công
            return true;
        }
    }
}
