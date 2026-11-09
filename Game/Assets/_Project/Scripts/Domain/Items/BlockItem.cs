using System;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.Items
{
    /// <summary>
    /// Vật phẩm đại diện cho các khối có thể cầm trên tay và đặt xuống thế giới Voxel.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Quy tắc nghiệp vụ (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Kế thừa &amp; Đa hình (Inheritance &amp; Polymorphism - Tiêu chí 14):</b> Kế thừa từ <see cref="Item"/> và ghi đè phương thức <see cref="Use"/>.</item>
    ///   <item><b>Quy tắc R2 (Tiêu chí 3):</b> Không cho phép đặt khối vào ô đã có khối hoặc ngoài biên chiều cao thế giới.</item>
    ///   <item><b>Quy tắc R6 (Tiêu chí 3):</b> Mọi khối đều có số lượng xếp chồng tối đa là 64 (<c>MaxStackSize = 64</c>).</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class BlockItem : Item
    {
        private readonly BlockId _placeBlockId;

        /// <summary>Mã loại khối sẽ được tạo ra trong thế giới khi đặt vật phẩm này.</summary>
        public BlockId PlaceBlockId => _placeBlockId;

        /// <summary>
        /// Khởi tạo vật phẩm khối.
        /// </summary>
        /// <param name="id">Mã vật phẩm.</param>
        /// <param name="name">Tên hiển thị.</param>
        /// <param name="placeBlockId">Mã khối tương ứng khi đặt.</param>
        public BlockItem(ItemId id, string name, BlockId placeBlockId)
            : base(id, name, maxStackSize: 64)
        {
            _placeBlockId = placeBlockId;
        }

        /// <inheritdoc />
        public override bool Use(ItemUseContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            BlockPos targetPos = context.AdjacentPlacePos;

            // Quy tắc R2 & R3: Kiểm tra trong biên thế giới và ô đặt phải là Air
            if (!context.WorldReader.IsInBounds(targetPos))
            {
                return false;
            }

            if (context.WorldReader.GetBlock(targetPos) != BlockId.Air)
            {
                return false;
            }

            // Đặt khối vào thế giới qua WorldWriter
            return context.WorldWriter.SetBlock(targetPos, _placeBlockId);
        }
    }
}
