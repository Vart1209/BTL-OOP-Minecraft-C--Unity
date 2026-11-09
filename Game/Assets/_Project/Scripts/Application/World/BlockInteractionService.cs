using System;
using Minecraft.Domain.Blocks;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.Items;

namespace Minecraft.Application.World
{
    /// <summary>
    /// Dịch vụ ứng dụng điều phối các Use Case tương tác khối: UC03 (Phá block) và UC04 (Đặt block).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc phân tầng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Dependency Injection (Tiêu chí 2):</b> Tiêm phụ thuộc <see cref="IWorldReader"/>, <see cref="IWorldWriter"/>, <see cref="BlockRegistry"/>, <see cref="ItemRegistry"/> qua constructor.</item>
    ///   <item><b>Thực thi quy tắc nghiệp vụ (Tiêu chí 3):</b>
    ///     <list type="bullet">
    ///       <item><b>Quy tắc R1 &amp; R12:</b> Kiểm tra quyền <see cref="IGameMode.CanBreakBlock"/> (Survival không phá được Bedrock, Creative phá được).</item>
    ///       <item><b>Quy tắc R2:</b> Không đặt block vào ô đã có block hoặc trùng vị trí chân/đầu người chơi.</item>
    ///       <item><b>Quy tắc R3:</b> Từ chối mọi thao tác ngoài biên chiều cao thế giới.</item>
    ///       <item><b>Quy tắc R5:</b> Khi phá khối thành công ở chế độ Survival, tự động thêm vật phẩm rơi tương ứng vào túi đồ.</item>
    ///       <item><b>Quy tắc R12:</b> Survival tiêu hao vật phẩm khi đặt; Creative không tiêu hao.</item>
    ///     </list>
    ///   </item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class BlockInteractionService
    {
        private readonly IWorldReader _worldReader;
        private readonly IWorldWriter _worldWriter;
        private readonly BlockRegistry _blockRegistry;
        private readonly ItemRegistry _itemRegistry;

        /// <summary>
        /// Khởi tạo dịch vụ tương tác khối.
        /// </summary>
        public BlockInteractionService(
            IWorldReader worldReader,
            IWorldWriter worldWriter,
            BlockRegistry blockRegistry,
            ItemRegistry itemRegistry)
        {
            _worldReader = worldReader ?? throw new ArgumentNullException(nameof(worldReader));
            _worldWriter = worldWriter ?? throw new ArgumentNullException(nameof(worldWriter));
            _blockRegistry = blockRegistry ?? throw new ArgumentNullException(nameof(blockRegistry));
            _itemRegistry = itemRegistry ?? throw new ArgumentNullException(nameof(itemRegistry));
        }

        /// <summary>
        /// Xử lý hành vi phá hủy một khối tại tọa độ chỉ định (Use Case UC03).
        /// </summary>
        /// <param name="targetPos">Tọa độ khối mục tiêu cần phá.</param>
        /// <param name="inventory">Túi đồ của người chơi (dùng để nhận vật phẩm rơi theo Quy tắc R5).</param>
        /// <param name="gameMode">Chế độ chơi hiện tại (phân quyền tác nhân theo Quy tắc R1 và R12).</param>
        /// <returns><c>true</c> nếu phá khối thành công; ngược lại <c>false</c>.</returns>
        public bool TryBreakBlock(BlockPos targetPos, Inventory inventory, IGameMode gameMode)
        {
            if (inventory == null)
            {
                throw new ArgumentNullException(nameof(inventory));
            }

            if (gameMode == null)
            {
                throw new ArgumentNullException(nameof(gameMode));
            }

            // Quy tắc R3: Kiểm tra biên độ cao thế giới
            if (!_worldReader.IsInBounds(targetPos))
            {
                return false;
            }

            BlockId currentBlock = _worldReader.GetBlock(targetPos);
            if (currentBlock == BlockId.Air)
            {
                return false;
            }

            // Quy tắc R1 & R12: Phân quyền phá khối theo GameMode (Bedrock không thể phá trong Survival)
            if (!gameMode.CanBreakBlock(currentBlock))
            {
                return false;
            }

            // Thực hiện đặt ô thành Air để phá khối
            bool removed = _worldWriter.SetBlock(targetPos, BlockId.Air);
            if (!removed)
            {
                return false;
            }

            // Quy tắc R5: Rơi vật phẩm vào túi đồ ở chế độ Survival
            if (gameMode.ConsumesItemsOnPlace && _blockRegistry.TryGet(currentBlock, out BlockType blockType))
            {
                if (blockType.DropItemId != ItemId.None && _itemRegistry.TryGet(blockType.DropItemId, out Item dropItem))
                {
                    inventory.AddItem(dropItem, 1);
                }
            }

            return true;
        }

        /// <summary>
        /// Xử lý hành vi đặt một khối vào thế giới từ vật phẩm đang chọn trên Hotbar (Use Case UC04).
        /// </summary>
        /// <param name="placePos">Tọa độ ô cần đặt khối mới vào.</param>
        /// <param name="playerFootPos">Tọa độ chân người chơi (kiểm tra va chạm Quy tắc R2).</param>
        /// <param name="playerHeadPos">Tọa độ đầu người chơi (kiểm tra va chạm Quy tắc R2).</param>
        /// <param name="inventory">Túi đồ của người chơi để lấy vật phẩm đang cầm.</param>
        /// <param name="gameMode">Chế độ chơi hiện tại (Survival tiêu hao item, Creative không tiêu hao).</param>
        /// <returns><c>true</c> nếu đặt khối thành công; ngược lại <c>false</c>.</returns>
        public bool TryPlaceBlock(
            BlockPos placePos,
            BlockPos playerFootPos,
            BlockPos playerHeadPos,
            Inventory inventory,
            IGameMode gameMode)
        {
            if (inventory == null)
            {
                throw new ArgumentNullException(nameof(inventory));
            }

            if (gameMode == null)
            {
                throw new ArgumentNullException(nameof(gameMode));
            }

            // Quy tắc R3: Kiểm tra biên độ cao thế giới
            if (!_worldReader.IsInBounds(placePos))
            {
                return false;
            }

            // Quy tắc R2: Không đặt block vào ô đã có block khác Air
            if (_worldReader.GetBlock(placePos) != BlockId.Air)
            {
                return false;
            }

            // Quy tắc R2: Không đặt block vào vị trí đang bị chiếm bởi chân hoặc đầu của người chơi
            if (placePos == playerFootPos || placePos == playerHeadPos)
            {
                return false;
            }

            ItemStack selectedSlot = inventory.GetSelectedSlot();
            if (selectedSlot == null || selectedSlot.IsEmpty)
            {
                return false;
            }

            // Chỉ cho phép đặt nếu vật phẩm đang cầm là BlockItem
            if (selectedSlot.Definition is BlockItem blockItem)
            {
                bool placed = _worldWriter.SetBlock(placePos, blockItem.PlaceBlockId);
                if (!placed)
                {
                    return false;
                }

                // Quy tắc R12: Survival trừ 1 vật phẩm, Creative không trừ
                if (gameMode.ConsumesItemsOnPlace)
                {
                    selectedSlot.Remove(1);
                    if (selectedSlot.Count == 0)
                    {
                        inventory.SetSlot(inventory.SelectedHotbarIndex, ItemStack.Empty);
                    }
                }

                return true;
            }

            return false;
        }
    }
}

