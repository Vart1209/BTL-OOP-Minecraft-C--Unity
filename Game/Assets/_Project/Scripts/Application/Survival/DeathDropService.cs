using System;
using System.Collections.Generic;
using System.Numerics;
using Minecraft.Domain.Entities;
using Minecraft.Domain.Items;

namespace Minecraft.Application.Survival
{
    /// <summary>
    /// Dịch vụ điều phối cơ chế Rơi đồ khi chết, Hồi sinh và Tự động thu nhặt vật phẩm trên mặt đất (Mở rộng Quy tắc R9 &amp; R12 — Tuần T4).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Quy tắc nghiệp vụ (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Phân quyền tác nhân qua State Pattern (Tiêu chí 4, 9, 10, 11):</b> Khi người chơi tử vong, dịch vụ kiểm tra <c>player.CurrentGameMode.ConsumesItemsOnPlace</c>. Ở chế độ Survival (<c>true</c>), toàn bộ 36 ô túi đồ rơi ra thành các <see cref="DroppedItemEntity"/> tại vị trí chết rồi xóa sạch túi đồ trước khi hồi sinh; ở chế độ Creative (<c>false</c>), người chơi giữ nguyên toàn bộ túi đồ.</item>
    ///   <item><b>Đa hình thực thể (Polymorphism - Tiêu chí 14):</b> Cập nhật nhịp thời gian cho danh sách <see cref="DroppedItemEntity"/> thông qua phương thức đa hình <see cref="Entity.Tick"/>.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class DeathDropService
    {
        /// <summary>Bán kính tự động thu nhặt vật phẩm mặc định quanh người chơi (2.0 mét).</summary>
        public const float DefaultPickupRadius = 2.0f;

        private readonly List<DroppedItemEntity> _activeDrops = new List<DroppedItemEntity>();

        /// <summary>Danh sách các thực thể vật phẩm đang nằm trên mặt đất trong thế giới.</summary>
        public IReadOnlyList<DroppedItemEntity> ActiveDrops => _activeDrops;

        /// <summary>
        /// Tạo trực tiếp một thực thể vật phẩm rơi tại vị trí chỉ định trong thế giới.
        /// </summary>
        public DroppedItemEntity SpawnDrop(
            Vector3 position,
            ItemStack stack,
            float despawnTimeSeconds = DroppedItemEntity.DefaultDespawnTimeSeconds)
        {
            if (stack == null || stack.IsEmpty)
            {
                throw new ArgumentException("Không thể tạo vật phẩm rơi từ ngăn xếp rỗng.", nameof(stack));
            }

            var drop = new DroppedItemEntity(position, stack, despawnTimeSeconds);
            _activeDrops.Add(drop);
            return drop;
        }

        /// <summary>
        /// Xử lý rơi toàn bộ túi đồ khi người chơi chết ở chế độ Survival và hồi sinh người chơi về điểm xuất phát (Quy tắc R9 &amp; R12).
        /// </summary>
        /// <param name="player">Thực thể người chơi vừa tử vong.</param>
        /// <returns>Số lượng ngăn xếp vật phẩm (<see cref="DroppedItemEntity"/>) đã văng ra mặt đất.</returns>
        public int DropInventoryOnDeath(Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            int droppedCount = 0;
            Vector3 deathPosition = player.Position;

            // Kiểm tra quyền theo chế độ chơi: Survival (ConsumesItemsOnPlace == true) rơi sạch đồ; Creative giữ nguyên túi đồ
            if (player.CurrentGameMode != null && player.CurrentGameMode.ConsumesItemsOnPlace)
            {
                for (int i = 0; i < Inventory.SlotCount; i++)
                {
                    ItemStack slotStack = player.Inventory.GetSlot(i);
                    if (slotStack != null && !slotStack.IsEmpty)
                    {
                        var dropEntity = new DroppedItemEntity(deathPosition, slotStack);
                        _activeDrops.Add(dropEntity);
                        droppedCount++;
                    }
                }

                player.Inventory.Clear();
            }

            player.Respawn();
            return droppedCount;
        }

        /// <summary>
        /// Kiểm tra các vật phẩm đang rơi trong bán kính thu nhặt quanh vị trí người chơi và tự động đưa vào túi đồ.
        /// </summary>
        /// <param name="player">Thực thể người chơi.</param>
        /// <param name="pickupRadius">Bán kính thu nhặt (mặc định 2.0f).</param>
        /// <returns>Số lượng ngăn xếp vật phẩm đã nhặt hoàn toàn vào túi đồ.</returns>
        public int CollectNearbyDrops(Player player, float pickupRadius = DefaultPickupRadius)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            if (pickupRadius <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(pickupRadius), "Bán kính thu nhặt phải lớn hơn 0.");
            }

            int collectedStacksCount = 0;
            float radiusSquared = pickupRadius * pickupRadius;

            for (int i = _activeDrops.Count - 1; i >= 0; i--)
            {
                DroppedItemEntity drop = _activeDrops[i];
                if (!drop.IsAlive || drop.IsExpired || drop.Payload.IsEmpty)
                {
                    _activeDrops.RemoveAt(i);
                    continue;
                }

                float distSq = Vector3.DistanceSquared(player.Position, drop.Position);
                if (distSq <= radiusSquared)
                {
                    Item itemDef = drop.Payload.Definition;
                    int amountToPick = drop.Payload.Count;
                    int remainder = player.Inventory.AddItem(itemDef, amountToPick);

                    if (remainder == 0)
                    {
                        drop.MarkCollected();
                        _activeDrops.RemoveAt(i);
                        collectedStacksCount++;
                    }
                    else if (remainder < amountToPick)
                    {
                        int picked = amountToPick - remainder;
                        drop.Payload.Remove(picked);
                    }
                }
            }

            return collectedStacksCount;
        }

        /// <summary>
        /// Cập nhật bộ đếm thời gian cho toàn bộ vật phẩm đang rơi trên mặt đất và dọn dẹp các vật phẩm đã hết hạn.
        /// </summary>
        public void Tick(float deltaTime)
        {
            if (deltaTime <= 0f)
            {
                return;
            }

            for (int i = _activeDrops.Count - 1; i >= 0; i--)
            {
                DroppedItemEntity drop = _activeDrops[i];
                drop.Tick(deltaTime);
                if (!drop.IsAlive || drop.IsExpired)
                {
                    _activeDrops.RemoveAt(i);
                }
            }
        }
    }
}
