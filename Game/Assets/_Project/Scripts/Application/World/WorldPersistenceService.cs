using System;
using System.Collections.Generic;
using System.Numerics;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.Entities;
using Minecraft.Domain.GameModes;
using Minecraft.Domain.Items;
using Minecraft.Domain.Persistence;
using Minecraft.Domain.World;
using DomainWorld = Minecraft.Domain.World.World;

namespace Minecraft.Application.World
{
    /// <summary>
    /// Dịch vụ ứng dụng điều phối Use Case Lưu thế giới (UC05) và Tải thế giới (UC06).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Single Responsibility Principle (SRP - Tiêu chí 16):</b> Chỉ chịu trách nhiệm điều phối luồng dữ liệu giữa tầng Domain và Repository lưu trữ, không can thiệp trực tiếp vào thuật toán sinh hay hiển thị.</item>
    ///   <item><b>Dependency Injection (DIP - Tiêu chí 16):</b> Nhận <see cref="IWorldRepository"/> và <see cref="ItemRegistry"/> qua constructor, cho phép đổi cơ chế lưu từ JSON sang nhị phân mà không sửa đổi một dòng code nghiệp vụ.</item>
    ///   <item><b>State Pattern Synchronization:</b> Khôi phục chính xác trạng thái chế độ chơi (<see cref="IGameMode"/>) của người chơi khi nạp lại thế giới.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class WorldPersistenceService
    {
        private readonly IWorldRepository _repository;
        private readonly ItemRegistry _itemRegistry;

        /// <summary>
        /// Khởi tạo dịch vụ lưu/tải thế giới với các phụ thuộc được tiêm từ ngoài vào.
        /// </summary>
        public WorldPersistenceService(IWorldRepository repository, ItemRegistry itemRegistry)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _itemRegistry = itemRegistry ?? throw new ArgumentNullException(nameof(itemRegistry));
        }

        /// <summary>
        /// Thực hiện Use Case UC05: Lưu toàn bộ trạng thái thế giới và người chơi ra file.
        /// </summary>
        public bool SaveGame(
            string saveFilePath,
            int worldSeed,
            string saveName,
            DomainWorld world,
            Player player,
            IReadOnlyDictionary<BlockPos, BlockId> modifiedBlocks)
        {
            if (world == null) throw new ArgumentNullException(nameof(world));
            if (player == null) throw new ArgumentNullException(nameof(player));

            var dto = new WorldSaveDto
            {
                WorldSeed = worldSeed,
                SaveName = string.IsNullOrWhiteSpace(saveName) ? "World" : saveName,
                SavedAtUtc = DateTime.UtcNow.ToString("o"),
                Player = new PlayerSaveDto
                {
                    PositionX = player.Position.X,
                    PositionY = player.Position.Y,
                    PositionZ = player.Position.Z,
                    CurrentHealth = player.Health.CurrentHealth,
                    CurrentHunger = player.Hunger.CurrentHunger,
                    GameMode = player.CurrentGameMode?.ModeName ?? "Survival",
                    InventorySlots = new List<InventorySlotDto>()
                },
                ModifiedBlocks = new List<BlockChangeDto>()
            };

            // Lưu các ô trong túi đồ có vật phẩm
            for (int i = 0; i < player.Inventory.TotalSlots; i++)
            {
                var stack = player.Inventory.GetSlot(i);
                if (stack != null && !stack.IsEmpty && stack.Item != null)
                {
                    dto.Player.InventorySlots.Add(new InventorySlotDto
                    {
                        SlotIndex = i,
                        ItemId = stack.Item.Id.ToString(),
                        Count = stack.Count
                    });
                }
            }

            // Lưu danh sách các khối đã bị thay đổi so với địa hình gốc
            if (modifiedBlocks != null)
            {
                foreach (var kvp in modifiedBlocks)
                {
                    dto.ModifiedBlocks.Add(new BlockChangeDto
                    {
                        X = kvp.Key.X,
                        Y = kvp.Key.Y,
                        Z = kvp.Key.Z,
                        BlockId = kvp.Value.ToString()
                    });
                }
            }

            return _repository.SaveWorld(saveFilePath, dto);
        }

        /// <summary>
        /// Thực hiện Use Case UC06: Tải và khôi phục trạng thái thế giới, người chơi và khối từ file.
        /// </summary>
        public bool LoadGame(
            string saveFilePath,
            DomainWorld world,
            Player player,
            out Dictionary<BlockPos, BlockId> restoredModifiedBlocks,
            out IGameMode loadedMode)
        {
            restoredModifiedBlocks = new Dictionary<BlockPos, BlockId>();
            loadedMode = new SurvivalMode();

            if (world == null || player == null)
            {
                return false;
            }

            if (!_repository.TryLoadWorld(saveFilePath, out WorldSaveDto dto) || dto == null)
            {
                return false;
            }

            // Khôi phục GameMode (State Pattern)
            if (string.Equals(dto.Player?.GameMode, "Creative", StringComparison.OrdinalIgnoreCase))
            {
                loadedMode = new CreativeMode();
            }
            else if (string.Equals(dto.Player?.GameMode, "Adventure", StringComparison.OrdinalIgnoreCase))
            {
                loadedMode = new AdventureMode();
            }
            else
            {
                loadedMode = new SurvivalMode();
            }

            player.SetGameMode(loadedMode);

            // Khôi phục vị trí, máu và đói của người chơi
            if (dto.Player != null)
            {
                player.MoveTo(new Vector3(dto.Player.PositionX, dto.Player.PositionY, dto.Player.PositionZ));
                player.Health.SetCurrentHealth(dto.Player.CurrentHealth);
                player.Hunger.SetCurrentHunger(dto.Player.CurrentHunger);

                // Khôi phục túi đồ
                player.Inventory.Clear();
                if (dto.Player.InventorySlots != null)
                {
                    foreach (var slotDto in dto.Player.InventorySlots)
                    {
                        if (Enum.TryParse<ItemId>(slotDto.ItemId, out var itemId))
                        {
                            var item = _itemRegistry.GetItem(itemId);
                            if (item != null && slotDto.Count > 0)
                            {
                                player.Inventory.SetSlot(slotDto.SlotIndex, new ItemStack(item, slotDto.Count));
                            }
                        }
                    }
                }
            }

            // Khôi phục các khối bị chỉnh sửa vào thế giới
            if (dto.ModifiedBlocks != null)
            {
                foreach (var change in dto.ModifiedBlocks)
                {
                    if (Enum.TryParse<BlockId>(change.BlockId, out var blockId))
                    {
                        var pos = new BlockPos(change.X, change.Y, change.Z);
                        world.SetBlock(pos, blockId);
                        restoredModifiedBlocks[pos] = blockId;
                    }
                }
            }

            return true;
        }
    }
}
