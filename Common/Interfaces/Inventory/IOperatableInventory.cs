using Common.Database.Models.Interfaces;
using Common.Enums;

namespace Common.Interfaces.Inventory;

public interface IInventoryService
{
    /// <summary>
    /// Loads the inventory
    /// </summary>
    /// <param name="inventoryId">The id of the inventory to load</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns></returns>
    Task LoadAsync(Guid inventoryId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Tries to get an inventory item
    /// </summary>
    /// <param name="inventoryTab">The inventory tab the item is located at</param>
    /// <param name="slot">The specific slot of the item</param>
    /// <param name="inventoryTabItem">The item that was found if found at all</param>
    /// <returns></returns>
    bool TryGetItem(EInventoryTab inventoryTab, byte slot, out IInventoryTabItem? inventoryTabItem);
    
    bool TryAddItem(uint itemId, ushort quantity, EInventoryTab inventoryTab, out IInventoryTabItem? inventoryTabItem);
    
    /// <summary>
    /// Tries to remove an item from the inventory
    /// </summary>
    /// <param name="inventoryTab">The inventory tab the item is located at</param>
    /// <param name="slot">The slot of the item</param>
    /// <param name="inventoryTabItem">The item that has been removed, if any</param>
    /// <returns>Whether the item has been successfully removed from the inventory</returns>
    bool TryRemoveItem(EInventoryTab inventoryTab, byte slot, out IInventoryTabItem? inventoryTabItem);
    
    /// <summary>
    /// Tries to move an inventory tab item from one slot to another
    /// </summary>
    /// <param name="itemToMove">The inventory tab item to move</param>
    /// <param name="toSlot">The slot the item should try to move to</param>
    /// <param name="inventoryTabItem">The updated inventory tab item after the move</param>
    /// <returns></returns>
    bool TryMoveItem(IInventoryTabItem itemToMove, byte toSlot, out IInventoryTabItem? inventoryTabItem);
}