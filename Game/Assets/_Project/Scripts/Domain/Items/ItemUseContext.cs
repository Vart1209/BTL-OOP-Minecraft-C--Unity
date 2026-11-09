using System;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.Items
{
    /// <summary>
    /// Gói ngữ cảnh truyền vào khi người chơi kích hoạt sử dụng một vật phẩm (Item Use Context).
    /// <para>
    /// <b>Nguyên lý OOP áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Đóng gói tham số (Parameter Object Pattern):</b> Gom các thông số về vị trí nhắm tới, vị trí kề và các giao diện điều phối thế giới/chế độ chơi vào một đối tượng duy nhất, tránh việc các phương thức <c>Use</c> phải nhận danh sách tham số quá dài.</item>
    ///   <item><b>Đóng gói &amp; Hợp lệ dữ liệu (Tiêu chí 13):</b> Kiểm tra tính hợp lệ không null của các tham chiếu <see cref="IWorldReader"/>, <see cref="IWorldWriter"/>, <see cref="IGameMode"/>.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class ItemUseContext
    {
        private readonly BlockPos _targetBlockPos;
        private readonly BlockPos _adjacentPlacePos;
        private readonly IWorldReader _worldReader;
        private readonly IWorldWriter _worldWriter;
        private readonly IGameMode _gameMode;

        /// <summary>Tọa độ của khối đang được ngắm trúng.</summary>
        public BlockPos TargetBlockPos => _targetBlockPos;

        /// <summary>Tọa độ ô kề mặt khối được ngắm (nơi sẽ đặt khối mới vào).</summary>
        public BlockPos AdjacentPlacePos => _adjacentPlacePos;

        /// <summary>Giao diện đọc trạng thái khối thế giới.</summary>
        public IWorldReader WorldReader => _worldReader;

        /// <summary>Giao diện cập nhật khối thế giới.</summary>
        public IWorldWriter WorldWriter => _worldWriter;

        /// <summary>Chế độ chơi hiện tại của người chơi (Survival / Creative).</summary>
        public IGameMode GameMode => _gameMode;

        /// <summary>
        /// Khởi tạo ngữ cảnh sử dụng vật phẩm.
        /// </summary>
        public ItemUseContext(
            BlockPos targetBlockPos,
            BlockPos adjacentPlacePos,
            IWorldReader worldReader,
            IWorldWriter worldWriter,
            IGameMode gameMode)
        {
            _targetBlockPos = targetBlockPos;
            _adjacentPlacePos = adjacentPlacePos;
            _worldReader = worldReader ?? throw new ArgumentNullException(nameof(worldReader));
            _worldWriter = worldWriter ?? throw new ArgumentNullException(nameof(worldWriter));
            _gameMode = gameMode ?? throw new ArgumentNullException(nameof(gameMode));
        }
    }
}
