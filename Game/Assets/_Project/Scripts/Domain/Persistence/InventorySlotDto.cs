namespace Minecraft.Domain.Persistence
{
    /// <summary>
    /// Đối tượng DTO lưu thông tin một ô trong túi đồ người chơi.
    /// </summary>
    public sealed class InventorySlotDto
    {
        public int SlotIndex { get; set; }
        public string ItemId { get; set; }
        public int Count { get; set; }
    }
}
