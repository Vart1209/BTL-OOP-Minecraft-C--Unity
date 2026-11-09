using System;
using Minecraft.Domain.Blocks;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.Items
{
    /// <summary>
    /// Vật phẩm công cụ hoặc vũ khí hỗ trợ khai thác tài nguyên và chiến đấu.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Quy tắc nghiệp vụ (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Kế thừa &amp; Đa hình (Inheritance &amp; Polymorphism - Tiêu chí 14):</b> Kế thừa từ <see cref="Item"/>.</item>
    ///   <item><b>Quy tắc R6 (Tiêu chí 3):</b> Mọi công cụ đều có số lượng xếp chồng tối đa là 1 (<c>MaxStackSize = 1</c>).</item>
    ///   <item><b>Quy tắc R4 (Tiêu chí 3):</b> Thời gian đào một khối được tính bằng công thức: <c>Thời gian đào = Độ cứng / Hệ số công cụ</c>. Khối không hợp công cụ đào với tốc độ cơ bản; ở chế độ Creative đào tức thì (0 giây); Bedrock không thể đào (vô cực).</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class ToolItem : Item
    {
        private readonly ToolType _toolCategory;
        private readonly float _miningSpeedMultiplier;
        private readonly int _attackDamage;

        /// <summary>Chủng loại công cụ.</summary>
        public ToolType ToolCategory => _toolCategory;

        /// <summary>Hệ số nhân tốc độ khai thác khối tương thích (> 0).</summary>
        public float MiningSpeedMultiplier => _miningSpeedMultiplier;

        /// <summary>Sát thương vật lý khi tấn công quái vật / thực thể.</summary>
        public int AttackDamage => _attackDamage;

        /// <summary>
        /// Khởi tạo một công cụ.
        /// </summary>
        public ToolItem(
            ItemId id,
            string name,
            ToolType toolCategory,
            float miningSpeedMultiplier,
            int attackDamage)
            : base(id, name, maxStackSize: 1)
        {
            if (miningSpeedMultiplier <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(miningSpeedMultiplier), "Hệ số tốc độ đào phải > 0.");
            }

            if (attackDamage < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(attackDamage), "Sát thương tấn công phải >= 1.");
            }

            _toolCategory = toolCategory;
            _miningSpeedMultiplier = miningSpeedMultiplier;
            _attackDamage = attackDamage;
        }

        /// <summary>
        /// Tính toán thời gian cần thiết để đào vỡ một khối dựa trên độ cứng và chế độ chơi (Quy tắc R4).
        /// </summary>
        /// <param name="block">Khối mục tiêu cần đào.</param>
        /// <param name="mode">Chế độ chơi hiện tại của người chơi.</param>
        /// <returns>Thời gian đào tính bằng giây (0f nếu tức thì, vô cực nếu không thể phá hủy).</returns>
        public float GetMiningDuration(BlockType block, IGameMode mode)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            if (mode != null && mode.InstantBreak)
            {
                return 0f;
            }

            // Quy tắc R1: Bedrock không thể phá trong Survival (Hardness = -1)
            if (block.Hardness < 0f)
            {
                return float.PositiveInfinity;
            }

            bool isEffective = IsEffectiveAgainst(block.Id);
            float multiplier = isEffective ? _miningSpeedMultiplier : 1.0f;

            return block.Hardness / multiplier;
        }

        private bool IsEffectiveAgainst(BlockId blockId)
        {
            switch (_toolCategory)
            {
                case ToolType.Pickaxe:
                    return blockId == BlockId.Stone || blockId == BlockId.Cobblestone;
                case ToolType.Axe:
                    return blockId == BlockId.Wood || blockId == BlockId.Planks || blockId == BlockId.CraftingTable;
                case ToolType.Shovel:
                    return blockId == BlockId.Dirt || blockId == BlockId.Grass || blockId == BlockId.Sand;
                default:
                    return false;
            }
        }

        /// <inheritdoc />
        public override bool Use(ItemUseContext context)
        {
            // Sử dụng công cụ (tấn công hoặc vung tay đào khối)
            return true;
        }
    }
}

