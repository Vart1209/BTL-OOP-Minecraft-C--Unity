using System;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.Items
{
    /// <summary>
    /// Lớp cơ sở trừu tượng (Abstract Base Class) định nghĩa mọi loại vật phẩm trong hệ thống túi đồ và chế tạo.
    /// <para>
    /// <b>Nguyên lý OOP áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Trừu tượng hóa (Abstraction - Tiêu chí 13):</b> Định nghĩa khuôn mẫu chung cho mọi vật phẩm qua phương thức thuần ảo <see cref="Use(ItemUseContext)"/>.</item>
    ///   <item><b>Đóng gói (Encapsulation - Tiêu chí 13):</b> Các trường <c>_id</c>, <c>_name</c>, <c>_maxStackSize</c> là <c>private readonly</c>, có kiểm tra chặt chẽ điều kiện số lượng xếp chồng từ 1 tới 64 (Quy tắc R6).</item>
    ///   <item><b>Đa hình &amp; Kế thừa (Polymorphism &amp; Inheritance - Tiêu chí 14):</b> Các lớp con <see cref="BlockItem"/>, <see cref="ToolItem"/>, <see cref="FoodItem"/> kế thừa và ghi đè hành vi <see cref="Use"/> theo nghiệp vụ riêng.</item>
    /// </list>
    /// </para>
    /// </summary>
    public abstract class Item
    {
        private readonly ItemId _id;
        private readonly string _name;
        private readonly int _maxStackSize;

        /// <summary>Mã định danh duy nhất của vật phẩm.</summary>
        public ItemId Id => _id;

        /// <summary>Tên hiển thị của vật phẩm.</summary>
        public string Name => _name;

        /// <summary>Số lượng tối đa có thể xếp chồng trong một ô túi đồ (1..64 theo Quy tắc R6).</summary>
        public int MaxStackSize => _maxStackSize;

        /// <summary>
        /// Khởi tạo một định nghĩa vật phẩm.
        /// </summary>
        /// <param name="id">Mã định danh của vật phẩm.</param>
        /// <param name="name">Tên hiển thị.</param>
        /// <param name="maxStackSize">Số lượng xếp chồng tối đa (1..64).</param>
        /// <exception cref="ArgumentException">Ném ra nếu tên để trống.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Ném ra nếu maxStackSize không nằm trong [1, 64].</exception>
        protected Item(ItemId id, string name, int maxStackSize)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Tên vật phẩm không được để trống.", nameof(name));
            }

            if (maxStackSize < 1 || maxStackSize > 64)
            {
                throw new ArgumentOutOfRangeException(nameof(maxStackSize), "Số lượng xếp chồng (MaxStackSize) phải nằm trong khoảng [1, 64] (Quy tắc R6).");
            }

            _id = id;
            _name = name;
            _maxStackSize = maxStackSize;
        }

        /// <summary>
        /// Phương thức trừu tượng đa hình xử lý hành vi kích hoạt sử dụng vật phẩm.
        /// </summary>
        /// <param name="context">Ngữ cảnh khi dùng vật phẩm (vị trí nhắm, thế giới, gamemode).</param>
        /// <returns><c>true</c> nếu vật phẩm được dùng thành công (ví dụ đặt khối, ăn thức ăn); ngược lại <c>false</c>.</returns>
        public abstract bool Use(ItemUseContext context);
    }
}
