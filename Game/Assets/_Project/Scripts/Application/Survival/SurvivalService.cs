using System;
using Minecraft.Domain.Entities;
using Minecraft.Domain.Items;

namespace Minecraft.Application.Survival
{
    /// <summary>
    /// Dịch vụ ứng dụng điều phối Use Case UC10 (Ăn thức ăn) và nhịp sinh tồn của người chơi.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc phân tầng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Application Service (Tiêu chí 4, 8):</b> Điều phối tương tác giữa hành động của người chơi và các thực thể Domain (<see cref="Player"/>, <see cref="Hunger"/>, <see cref="Health"/>, <see cref="FoodItem"/>).</item>
    ///   <item><b>Quy tắc R8:</b> Cập nhật nhịp sinh học đói và tự hồi phục sinh lực theo thời gian thực.</item>
    ///   <item><b>Quy tắc R12:</b> Ở chế độ Survival trừ 1 thức ăn khi ăn; ở Creative giữ nguyên.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class SurvivalService
    {
        /// <summary>
        /// Xử lý hành động ăn thức ăn đang cầm trên tay của người chơi (Use Case UC10).
        /// </summary>
        /// <param name="player">Thực thể người chơi.</param>
        /// <returns><c>true</c> nếu ăn thành công; <c>false</c> nếu không cầm thức ăn hoặc người chơi đã chết.</returns>
        public bool TryEatSelectedFood(Player player)
        {
            if (player == null || !player.IsAlive)
            {
                return false;
            }

            ItemStack selectedSlot = player.Inventory.GetSelectedSlot();
            if (selectedSlot == null || selectedSlot.IsEmpty)
            {
                return false;
            }

            // Kiểm tra vật phẩm đang cầm có phải là FoodItem hay không
            if (selectedSlot.Definition is FoodItem foodItem)
            {
                // Chỉ ăn khi đói chưa đầy hoặc máu chưa đầy
                if (player.Hunger.Current < player.Hunger.Max || player.Health.Current < player.Health.Max)
                {
                    player.Hunger.Eat(foodItem.HungerRestore);
                    if (foodItem.HealthRestore > 0)
                    {
                        player.Health.Heal(foodItem.HealthRestore);
                    }

                    // Quy tắc R12: Survival trừ 1 thức ăn
                    if (player.CurrentGameMode.ConsumesItemsOnPlace)
                    {
                        selectedSlot.Remove(1);
                        if (selectedSlot.Count == 0)
                        {
                            player.Inventory.SetSlot(player.Inventory.SelectedHotbarIndex, ItemStack.Empty);
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Cập nhật chu kỳ nhịp sinh học đói và máu của người chơi.
        /// </summary>
        /// <param name="player">Thực thể người chơi cần cập nhật.</param>
        /// <param name="deltaTime">Thời gian trôi qua giữa các khung hình (giây).</param>
        public void TickPlayerSurvival(Player player, float deltaTime)
        {
            if (player != null && player.IsAlive)
            {
                player.Tick(deltaTime);
            }
        }
    }
}
