using System;
using System.Numerics;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.Items;

namespace Minecraft.Domain.Entities
{
    /// <summary>
    /// Thực thể người chơi chính trong thế giới Voxel.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Kế thừa (Inheritance - Tiêu chí 6, 14):</b> <c>Player : Entity</c> — Kế thừa vị trí và hệ thống máu từ <see cref="Entity"/>.</item>
    ///   <item><b>Quan hệ Hợp thành (Composition - Tiêu chí 6):</b> <c>Player ◆ Inventory, Health, Hunger</c> — Người chơi sở hữu vòng đời của túi đồ, máu và thanh độ đói.</item>
    ///   <item><b>State Pattern (Tiêu chí 9, 10, 11):</b> Nắm giữ trạng thái chế độ chơi <see cref="CurrentGameMode"/> và cho phép đổi qua <see cref="SetGameMode"/>.</item>
    ///   <item><b>Quy tắc R9:</b> Hỗ trợ phương thức <see cref="Respawn"/> đưa người chơi về điểm hồi sinh <see cref="SpawnPoint"/>, hồi đầy máu và đói.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class Player : Entity
    {
        private readonly Inventory _inventory;
        private readonly Hunger _hunger;
        private readonly Vector3 _spawnPoint;
        private IGameMode _currentGameMode;

        /// <summary>Hệ thống túi đồ của người chơi.</summary>
        public Inventory Inventory => _inventory;

        /// <summary>Hệ thống độ đói của người chơi.</summary>
        public Hunger Hunger => _hunger;

        /// <summary>Tọa độ điểm hồi sinh cố định của người chơi.</summary>
        public Vector3 SpawnPoint => _spawnPoint;

        /// <summary>Chế độ chơi hiện tại của người chơi (Survival / Creative).</summary>
        public IGameMode CurrentGameMode => _currentGameMode;

        /// <summary>
        /// Khởi tạo người chơi mới với vị trí hồi sinh và chế độ chơi mặc định.
        /// </summary>
        public Player(
            Vector3 spawnPoint,
            IGameMode initialGameMode,
            int maxHealth = 20,
            int maxHunger = 20)
            : base(spawnPoint, maxHealth)
        {
            _spawnPoint = spawnPoint;
            _currentGameMode = initialGameMode ?? throw new ArgumentNullException(nameof(initialGameMode));
            _inventory = new Inventory();
            _hunger = new Hunger(maxHunger);
        }

        /// <summary>
        /// Thay đổi chế độ chơi hiện tại của người chơi (State Pattern).
        /// </summary>
        public void SetGameMode(IGameMode newMode)
        {
            _currentGameMode = newMode ?? throw new ArgumentNullException(nameof(newMode));
        }

        /// <summary>
        /// Hồi sinh người chơi sau khi tử vong (Quy tắc R9).
        /// </summary>
        public void Respawn()
        {
            MoveTo(_spawnPoint);
            Health.ResetToMax();
            Hunger.ResetToMax();
        }

        /// <inheritdoc />
        public override void Tick(float deltaTime)
        {
            _hunger.Tick(deltaTime, Health, _currentGameMode);
        }
    }
}

