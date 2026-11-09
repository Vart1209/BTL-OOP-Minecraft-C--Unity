namespace Minecraft.Domain.Items
{
    /// <summary>
    /// Định danh các chủng loại công cụ lao động và vũ khí trong trò chơi.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Quy tắc nghiệp vụ (Quy tắc R4, R6 - dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item>Dùng để đối sánh loại công cụ thích hợp khi khai thác khối (ví dụ: <see cref="Pickaxe"/> đào đá nhanh hơn, <see cref="Axe"/> chặt gỗ nhanh hơn).</item>
    /// </list>
    /// </para>
    /// </summary>
    public enum ToolType : byte
    {
        /// <summary>Không phải công cụ chuyên dụng.</summary>
        None = 0,

        /// <summary>Cúp đào đá và khoáng sản.</summary>
        Pickaxe = 1,

        /// <summary>Rìu chặt cây và các khối gỗ.</summary>
        Axe = 2,

        /// <summary>Xẻng xúc đất, cát, sỏi.</summary>
        Shovel = 3,

        /// <summary>Kiếm dùng chiến đấu gây sát thương lớn lên Mob.</summary>
        Sword = 4
    }
}
