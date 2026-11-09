using System;
using System.Collections.Generic;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.Blocks
{
    /// <summary>
    /// Sổ đăng ký và quản lý các loại khối (<see cref="BlockType"/>) trong thế giới.
    /// <para>
    /// <b>Nguyên lý OOP áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Đóng gói (Encapsulation - Tiêu chí 13):</b> Bảng tra cứu <c>_definitions</c> được khai báo <c>private readonly</c>, bảo vệ danh mục khối khỏi việc sửa đổi bất hợp pháp ngoài luồng.</item>
    ///   <item><b>Thực thi quy tắc nghiệp vụ (Quy tắc R5 - Tiêu chí 3):</b> Khởi tạo bảng ánh xạ khối sang vật phẩm rơi khi bị phá hủy.</item>
    ///   <item><b>Dễ mở rộng (Open/Closed Principle - Tiêu chí 12):</b> Cho phép đăng ký thêm các khối mới (ví dụ: khối Quặng, Obsidian) mà không cần sửa đổi các hàm điều phối của World.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class BlockRegistry
    {
        private readonly Dictionary<BlockId, BlockType> _definitions = new Dictionary<BlockId, BlockType>();

        /// <summary>
        /// Đăng ký một kiểu khối mới vào sổ quản lý.
        /// </summary>
        /// <param name="blockType">Đối tượng kiểu khối cần đăng ký.</param>
        /// <exception cref="ArgumentNullException">Ném ra nếu <paramref name="blockType"/> null.</exception>
        /// <exception cref="ArgumentException">Ném ra nếu <see cref="BlockType.Id"/> đã được đăng ký trước đó.</exception>
        public void Register(BlockType blockType)
        {
            if (blockType == null)
            {
                throw new ArgumentNullException(nameof(blockType));
            }

            if (_definitions.ContainsKey(blockType.Id))
            {
                throw new ArgumentException($"Khối có ID '{blockType.Id}' đã tồn tại trong Registry.", nameof(blockType));
            }

            _definitions.Add(blockType.Id, blockType);
        }

        /// <summary>
        /// Lấy thông tin kiểu khối theo mã định danh.
        /// </summary>
        /// <exception cref="KeyNotFoundException">Ném ra nếu ID chưa được đăng ký.</exception>
        public BlockType Get(BlockId id)
        {
            if (_definitions.TryGetValue(id, out BlockType type))
            {
                return type;
            }

            throw new KeyNotFoundException($"Không tìm thấy BlockType cho mã '{id}'.");
        }

        /// <summary>
        /// Thử lấy thông tin kiểu khối theo mã định danh.
        /// </summary>
        public bool TryGet(BlockId id, out BlockType blockType)
        {
            return _definitions.TryGetValue(id, out blockType);
        }

        /// <summary>
        /// Kiểm tra mã khối đã được đăng ký hay chưa.
        /// </summary>
        public bool Contains(BlockId id)
        {
            return _definitions.ContainsKey(id);
        }

        /// <summary>
        /// Khởi tạo Registry mặc định chứa toàn bộ các khối cốt lõi của trò chơi.
        /// Đáp ứng Quy tắc R5: Đá vỡ rơi Cobblestone, Cỏ rơi Đất, Lá rụng rơi Không khí, v.v.
        /// </summary>
        public static BlockRegistry CreateDefault()
        {
            BlockRegistry registry = new BlockRegistry();

            // Air
            registry.Register(new BlockType(
                id: BlockId.Air,
                name: "Air",
                isSolid: false,
                isTransparent: true,
                hardness: 0.0f,
                dropItemId: ItemId.None,
                topTextureIndex: 0,
                bottomTextureIndex: 0,
                sideTextureIndex: 0
            ));

            // Bedrock (Quy tắc R1: Hardness = -1 không thể phá trong Survival)
            registry.Register(new BlockType(
                id: BlockId.Bedrock,
                name: "Bedrock",
                isSolid: true,
                isTransparent: false,
                hardness: -1.0f,
                dropItemId: ItemId.None,
                topTextureIndex: 1,
                bottomTextureIndex: 1,
                sideTextureIndex: 1
            ));

            // Stone (Quy tắc R5: phá Stone rơi Cobblestone)
            registry.Register(new BlockType(
                id: BlockId.Stone,
                name: "Stone",
                isSolid: true,
                isTransparent: false,
                hardness: 1.5f,
                dropItemId: ItemId.Cobblestone,
                topTextureIndex: 2,
                bottomTextureIndex: 2,
                sideTextureIndex: 2
            ));

            // Dirt
            registry.Register(new BlockType(
                id: BlockId.Dirt,
                name: "Dirt",
                isSolid: true,
                isTransparent: false,
                hardness: 0.5f,
                dropItemId: ItemId.Dirt,
                topTextureIndex: 3,
                bottomTextureIndex: 3,
                sideTextureIndex: 3
            ));

            // Grass (Quy tắc R5: phá Grass rơi Dirt)
            registry.Register(new BlockType(
                id: BlockId.Grass,
                name: "Grass Block",
                isSolid: true,
                isTransparent: false,
                hardness: 0.6f,
                dropItemId: ItemId.Dirt,
                topTextureIndex: 4,
                bottomTextureIndex: 3,
                sideTextureIndex: 5
            ));

            // Wood (Thân gỗ)
            registry.Register(new BlockType(
                id: BlockId.Wood,
                name: "Wood Log",
                isSolid: true,
                isTransparent: false,
                hardness: 2.0f,
                dropItemId: ItemId.Wood,
                topTextureIndex: 7,
                bottomTextureIndex: 7,
                sideTextureIndex: 6
            ));

            // Leaves (Tán lá - trong suốt, phá rơi None trừ khi dùng kéo)
            registry.Register(new BlockType(
                id: BlockId.Leaves,
                name: "Leaves",
                isSolid: true,
                isTransparent: true,
                hardness: 0.2f,
                dropItemId: ItemId.None,
                topTextureIndex: 8,
                bottomTextureIndex: 8,
                sideTextureIndex: 8
            ));

            // Sand
            registry.Register(new BlockType(
                id: BlockId.Sand,
                name: "Sand",
                isSolid: true,
                isTransparent: false,
                hardness: 0.5f,
                dropItemId: ItemId.Sand,
                topTextureIndex: 9,
                bottomTextureIndex: 9,
                sideTextureIndex: 9
            ));

            // Planks (Ván gỗ)
            registry.Register(new BlockType(
                id: BlockId.Planks,
                name: "Wood Planks",
                isSolid: true,
                isTransparent: false,
                hardness: 2.0f,
                dropItemId: ItemId.Planks,
                topTextureIndex: 10,
                bottomTextureIndex: 10,
                sideTextureIndex: 10
            ));

            // Crafting Table
            registry.Register(new BlockType(
                id: BlockId.CraftingTable,
                name: "Crafting Table",
                isSolid: true,
                isTransparent: false,
                hardness: 2.5f,
                dropItemId: ItemId.CraftingTable,
                topTextureIndex: 11,
                bottomTextureIndex: 10,
                sideTextureIndex: 12
            ));

            // Cobblestone (Đá cuội)
            registry.Register(new BlockType(
                id: BlockId.Cobblestone,
                name: "Cobblestone",
                isSolid: true,
                isTransparent: false,
                hardness: 2.0f,
                dropItemId: ItemId.Cobblestone,
                topTextureIndex: 13,
                bottomTextureIndex: 13,
                sideTextureIndex: 13
            ));

            return registry;
        }
    }
}
