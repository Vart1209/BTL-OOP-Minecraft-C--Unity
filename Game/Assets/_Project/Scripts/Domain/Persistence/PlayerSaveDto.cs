using System.Collections.Generic;

namespace Minecraft.Domain.Persistence
{
    /// <summary>
    /// Đối tượng DTO lưu trạng thái người chơi (vị trí, máu, đói, chế độ chơi, túi đồ).
    /// </summary>
    public sealed class PlayerSaveDto
    {
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public int CurrentHealth { get; set; } = 20;
        public int CurrentHunger { get; set; } = 20;
        public string GameMode { get; set; } = "Survival";
        public List<InventorySlotDto> InventorySlots { get; set; } = new List<InventorySlotDto>();
    }
}
