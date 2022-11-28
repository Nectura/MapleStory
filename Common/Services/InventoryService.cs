using Common.Database.Models;
using Common.Database.Models.Interfaces;
using Common.Database.Repositories.Interfaces;
using Common.Enums;
using Common.Interfaces.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// TODO: handle cash items
namespace Common.Services;

public sealed class InventoryService : IInventoryService
{
    public SortedDictionary<EInventoryTab, SortedDictionary<byte, IInventoryTabItem>> TabItems { get; init; } = new();
    public Dictionary<EInventoryTab, byte> TabCapacity { get; init; } = new();

    private readonly IServiceScopeFactory _scopeFactory;
    private Guid? _inventoryId;

    public InventoryService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    private void AssertLoaded()
    {
        if (!TabItems.Any() || !TabCapacity.Any() || _inventoryId == default)
            throw new Exception("The inventory hasn't been loaded before use!");
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (!_inventoryId.HasValue)
            throw new Exception("The inventory hasn't been loaded before use!");
        
        await using var scope = _scopeFactory.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IInventoryTabItemRepository>();
        var inventoryItems = await repository.Query(m => m.InventoryId == _inventoryId.Value).ToListAsync(cancellationToken);

        foreach (var item in inventoryItems)
        {
            var itemSet = TabItems[item.InventoryTab];
            if (itemSet.TryGetValue(item.Slot, out var updatedItem))
            {
                item.Slot = updatedItem.Slot;
                item.Quantity = updatedItem.Quantity;
                continue;
            }
            repository.Remove(item);
        }
        
        

        await repository.SaveChangesAsync(cancellationToken);
    }
    
    public async Task LoadAsync(Guid inventoryId, CancellationToken cancellationToken = default)
    {
        var inventory = await _inventoryRepository
            .Query(m => m.Id == inventoryId)
            .Include(m => m.TabItems)
            .FirstOrDefaultAsync(cancellationToken);

        if (inventory == default)
            throw new ArgumentException($"Failed to find an inventory with id {inventoryId}");

        TabItems.Clear();
        
        foreach (var tab in Enum.GetValues<EInventoryTab>())
            TabItems.Add(tab, new SortedDictionary<byte, IInventoryTabItem>());
        
        foreach (var item in inventory.TabItems!)
            TabItems[item.InventoryTab].Add(item.Slot, item);
        
        TabCapacity.Clear();
        TabCapacity.Add(EInventoryTab.Equipment, inventory.EquippableTabSlots);
        TabCapacity.Add(EInventoryTab.Consumables, inventory.ConsumableTabSlots);
        TabCapacity.Add(EInventoryTab.Setup, inventory.SetupTabSlots);
        TabCapacity.Add(EInventoryTab.Etcetera, inventory.EtceteraTabSlots);
        TabCapacity.Add(EInventoryTab.Cash, inventory.CashTabSlots);
    }

    public bool TryGetItem(EInventoryTab inventoryTab, byte slot, out IInventoryTabItem? inventoryTabItem)
    {
        AssertLoaded();

        return TabItems[inventoryTab].TryGetValue(slot, out inventoryTabItem);
    }

    // TODO: handle item stacks
    public bool TryAddItem(uint itemId, ushort quantity, EInventoryTab inventoryTab, out IInventoryTabItem? inventoryTabItem)
    {
        AssertLoaded();

        // TODO: figure out the type of the item by the mapleId or packet

        var currentItemCount = (byte) TabItems[inventoryTab].Count;

        if (currentItemCount < TabCapacity[inventoryTab])
        {
            // TODO: create a new default or randomized instance of the item based on the WZ data
            inventoryTabItem = new InventoryTabItem
            {
                MapleId = itemId,
                Quantity = quantity
            };
            TabItems[inventoryTab].Add(currentItemCount, inventoryTabItem);
            return true;
        }

        inventoryTabItem = default;

        return false;
    }

    // TODO: handle item stacks
    public bool TryRemoveItem(EInventoryTab inventoryTab, byte slot, out IInventoryTabItem? inventoryTabItem)
    {
        AssertLoaded();

        if (!TabItems[EInventoryTab.Equipment].TryGetValue(slot, out inventoryTabItem)) return false;
        
        TabItems[EInventoryTab.Equipment].Remove(slot);
        
        return true;
    }

    // TODO: handle item stacks
    public bool TryMoveItem(IInventoryTabItem itemToMove, byte toSlot, out IInventoryTabItem? inventoryTabItem)
    {
        AssertLoaded();

        var inventoryTab = TabItems[EInventoryTab.Equipment];
        
        // if an item already exists in that spot the swap their places
        if (inventoryTab.TryGetValue(toSlot, out var existingItem))
        {
            existingItem.Slot = itemToMove.Slot;
            itemToMove.Slot = toSlot;
            inventoryTab[existingItem.Slot] = existingItem;
            inventoryTab[itemToMove.Slot] = itemToMove;
            inventoryTabItem = itemToMove;
            return true;
        }

        // if there's no more space in the inventory
        if (inventoryTab.Count >= TabCapacity[EInventoryTab.Equipment])
        {
            inventoryTabItem = default;
            return false;
        }
        
        // otherwise just put it in that spot and remove the slot occupation
        inventoryTab.Remove(itemToMove.Slot);
        itemToMove.Slot = toSlot;
        inventoryTab.Add(toSlot, itemToMove);
        inventoryTabItem = itemToMove;

        return true;
    }
}