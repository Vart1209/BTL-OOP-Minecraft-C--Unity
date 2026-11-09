using System;
using Minecraft.Domain.Contracts;

namespace Minecraft.Domain.Items
{
    /// <summary>
    /// Hệ thống túi đồ của người chơi, gồm 36 ô (9 ô Hotbar + 27 ô túi đồ chính) và cơ chế lựa chọn Hotbar.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc áp dụng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Quan hệ Hợp thành (Composition - Tiêu chí 6):</b> <c>Inventory ◆ ItemStack</c> — Quản lý mảng các ô <see cref="ItemStack"/>.</item>
    ///   <item><b>Observer Pattern (Tiêu chí 9, 10, 11):</b> Phát sự kiện <see cref="InventoryChanged"/> mỗi khi thêm, bớt hoặc đổi ô Hotbar để UI Presentation tự động cập nhật.</item>
    ///   <item><b>Quy tắc R10 (Tiêu chí 3):</b> Cơ chế trừ vật phẩm "đủ hết hoặc không làm gì" (Atomic Transaction) phục vụ chế tạo công thức mà không bị mất mát tài nguyên dở dang.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class Inventory
    {
        /// <summary>Tổng số ô trong túi đồ (9 Hotbar + 27 túi đồ = 36 ô).</summary>
        public const int SlotCount = 36;

        /// <summary>Số ô thuộc thanh Hotbar thao tác nhanh.</summary>
        public const int HotbarSlotCount = 9;

        private readonly ItemStack[] _slots = new ItemStack[SlotCount];
        private int _selectedHotbarIndex;

        /// <summary>Chỉ số ô Hotbar đang được chọn cầm trên tay (0..8).</summary>
        public int SelectedHotbarIndex => _selectedHotbarIndex;

        /// <summary>Tổng số lượng ô trong túi đồ.</summary>
        public int TotalSlots => SlotCount;

        /// <summary>Sự kiện phát ra khi nội dung túi đồ hoặc ô chọn Hotbar thay đổi (Observer Pattern).</summary>
        public event Action InventoryChanged;

        /// <summary>
        /// Khởi tạo túi đồ với toàn bộ các ô ban đầu là rỗng.
        /// </summary>
        public Inventory()
        {
            for (int i = 0; i < SlotCount; i++)
            {
                _slots[i] = ItemStack.Empty;
            }

            _selectedHotbarIndex = 0;
        }

        /// <summary>
        /// Lấy thông tin ngăn xếp vật phẩm tại một ô chỉ định.
        /// </summary>
        public ItemStack GetSlot(int index)
        {
            ValidateSlotIndex(index);
            return _slots[index] ?? ItemStack.Empty;
        }

        /// <summary>
        /// Gán thủ công một ngăn xếp vật phẩm vào một ô chỉ định.
        /// </summary>
        public void SetSlot(int index, ItemStack stack)
        {
            ValidateSlotIndex(index);
            _slots[index] = stack ?? ItemStack.Empty;
            InventoryChanged?.Invoke();
        }

        /// <summary>
        /// Lựa chọn ô Hotbar đang cầm trên tay (0..8).
        /// </summary>
        public void SelectHotbarSlot(int index)
        {
            if (index < 0 || index >= HotbarSlotCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Chỉ số Hotbar phải nằm trong khoảng [0, {HotbarSlotCount - 1}].");
            }

            if (_selectedHotbarIndex == index)
            {
                return;
            }

            _selectedHotbarIndex = index;
            InventoryChanged?.Invoke();
        }

        /// <summary>
        /// Lấy ngăn xếp vật phẩm của ô Hotbar đang được chọn.
        /// </summary>
        public ItemStack GetSelectedSlot()
        {
            return GetSlot(_selectedHotbarIndex);
        }

        /// <summary>
        /// Thêm một số lượng vật phẩm vào túi đồ.
        /// Tự động ưu tiên dồn vào các ô cùng loại chưa đầy trước khi chiếm ô trống mới.
        /// </summary>
        /// <param name="item">Loại vật phẩm cần thêm.</param>
        /// <param name="count">Số lượng cần thêm.</param>
        /// <returns>Số lượng còn dư không thể thêm do đầy túi (0 nếu thêm thành công toàn bộ).</returns>
        public int AddItem(Item item, int count)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Số lượng thêm vào túi phải > 0.");
            }

            int remaining = count;

            // Bước 1: Ưu tiên cộng dồn vào các ô cùng loại chưa đầy
            for (int i = 0; i < SlotCount && remaining > 0; i++)
            {
                ItemStack stack = _slots[i];
                if (!stack.IsEmpty && stack.Definition.Id == item.Id && stack.Count < item.MaxStackSize)
                {
                    remaining = stack.Add(remaining);
                }
            }

            // Bước 2: Điền vào các ô trống còn lại
            for (int i = 0; i < SlotCount && remaining > 0; i++)
            {
                if (_slots[i].IsEmpty)
                {
                    int toPut = Math.Min(item.MaxStackSize, remaining);
                    _slots[i] = new ItemStack(item, toPut);
                    remaining -= toPut;
                }
            }

            if (remaining < count)
            {
                InventoryChanged?.Invoke();
            }

            return remaining;
        }

        /// <summary>
        /// Đếm tổng số lượng của một loại vật phẩm trong toàn bộ túi đồ.
        /// </summary>
        public int CountItem(ItemId itemId)
        {
            int total = 0;
            for (int i = 0; i < SlotCount; i++)
            {
                ItemStack stack = _slots[i];
                if (!stack.IsEmpty && stack.Definition.Id == itemId)
                {
                    total += stack.Count;
                }
            }

            return total;
        }

        /// <summary>
        /// Trừ bớt một số lượng vật phẩm khỏi túi đồ theo Quy tắc R10 ("đủ hết hoặc không làm gì").
        /// </summary>
        /// <param name="itemId">Mã vật phẩm cần trừ.</param>
        /// <param name="count">Số lượng cần trừ.</param>
        /// <returns><c>true</c> nếu đủ số lượng và đã trừ thành công; <c>false</c> nếu không đủ vật phẩm (không trừ cái nào).</returns>
        public bool RemoveItem(ItemId itemId, int count)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Số lượng trừ phải > 0.");
            }

            // Kiểm tra đủ trước khi trừ (Atomic check)
            if (CountItem(itemId) < count)
            {
                return false;
            }

            int toRemove = count;
            for (int i = 0; i < SlotCount && toRemove > 0; i++)
            {
                ItemStack stack = _slots[i];
                if (!stack.IsEmpty && stack.Definition.Id == itemId)
                {
                    int removed = stack.Remove(toRemove);
                    toRemove -= removed;
                    if (stack.Count == 0)
                    {
                        _slots[i] = ItemStack.Empty;
                    }
                }
            }

            InventoryChanged?.Invoke();
            return true;
        }

        private static void ValidateSlotIndex(int index)
        {
            if (index < 0 || index >= SlotCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Chỉ số ô ({index}) phải nằm trong khoảng [0, {SlotCount - 1}].");
            }
        }
    }
}

