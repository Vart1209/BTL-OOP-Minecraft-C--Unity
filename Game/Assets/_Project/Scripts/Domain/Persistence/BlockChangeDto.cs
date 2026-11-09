namespace Minecraft.Domain.Persistence
{
    /// <summary>
    /// Đối tượng DTO lưu thông tin một khối bị người chơi thay đổi (đặt/phá).
    /// </summary>
    public sealed class BlockChangeDto
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public string BlockId { get; set; }
    }
}
