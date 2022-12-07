using Common.Database.Models;
using Common.Database.Models.Interfaces;
using Common.Database.WorkUnits.Interfaces;
using Common.Enums;
using Common.Interfaces.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Services;

public sealed class InventoryService : IInventoryService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private Guid? _inventoryId;
    
    public SortedDictionary<EInventoryTab, SortedDictionary<short, IInventoryTabItem>> TabItems { get; init; } = new();
    public Dictionary<EInventoryTab, byte> TabCapacity { get; init; } = new();

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
        var workUnit = scope.ServiceProvider.GetRequiredService<IInventoryWorkUnit>();
        var existingTabItems = await workUnit.InventoryTabItems.Query(m => m.InventoryId == _inventoryId).ToListAsync(cancellationToken);
        
        foreach (var inventoryTab in TabItems.Keys)
        {
            // update existing items
            var updatedItems = TabItems[inventoryTab].Values.Where(m => m.Id != Guid.Empty).ToDictionary(m => m.Id, m => m);
            foreach (var existingItem in existingTabItems.Where(m => updatedItems.ContainsKey(m.Id)))
            {
                var updatedItem = updatedItems[existingItem.Id];
                existingItem.UpdateFromReference(updatedItem);
            }
            // delete non-existing items
            workUnit.InventoryTabItems.RemoveRange(existingTabItems.Where(m => !updatedItems.ContainsKey(m.Id)));
            // add new items
            workUnit.InventoryTabItems.AddRange(TabItems[inventoryTab].Values.Where(m => m.Id == Guid.Empty).Select(m =>
            {
                var tabItem = new InventoryTabItem();
                tabItem.UpdateFromReference(m);
                return tabItem;
            }));
        }
        
        await workUnit.CommitChangesAsync(cancellationToken);
    }
    
    public async Task LoadAsync(Guid inventoryId, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var workUnit = scope.ServiceProvider.GetRequiredService<IInventoryWorkUnit>();
        
        var inventory = await workUnit.Inventories
            .Query(m => m.Id == inventoryId)
            .Include(m => m.TabItems)
            .FirstOrDefaultAsync(cancellationToken);

        if (inventory == default)
            throw new ArgumentException($"Failed to find an inventory with id {inventoryId}");

        _inventoryId = inventory.Id;
        
        TabItems.Clear();
        
        foreach (var tab in Enum.GetValues<EInventoryTab>())
            TabItems.Add(tab, new SortedDictionary<short, IInventoryTabItem>());
        
        foreach (var item in inventory.TabItems!)
            TabItems[item.InventoryTab].Add(item.Slot, item);
        
        TabCapacity.Clear();
        TabCapacity.Add(EInventoryTab.Equipment, inventory.EquippableTabSlots);
        TabCapacity.Add(EInventoryTab.Consumables, inventory.ConsumableTabSlots);
        TabCapacity.Add(EInventoryTab.Setup, inventory.SetupTabSlots);
        TabCapacity.Add(EInventoryTab.Etcetera, inventory.EtceteraTabSlots);
        TabCapacity.Add(EInventoryTab.Cash, inventory.CashTabSlots);
    }

    public bool TryGetItem(EInventoryTab inventoryTab, short slot, out IInventoryTabItem? inventoryTabItem)
    {
        AssertLoaded();

        return TabItems[inventoryTab].TryGetValue(slot, out inventoryTabItem);
    }

    // TODO: handle item stacks
    public bool TryAddItem(EInventoryTab inventoryTab, uint itemId, ushort quantity, out IInventoryTabItem? inventoryTabItem, short? slot = default)
    {
        AssertLoaded();

        // TODO: figure out the type of the item by the mapleId or packet

        var currentItemCount = (byte) TabItems[inventoryTab].Count;
        var toSlot = slot ?? (short) (currentItemCount + 1);

        if (currentItemCount < TabCapacity[inventoryTab])
        {
            // TODO: create a new default or randomized instance of the item based on the WZ data
            inventoryTabItem = new InventoryTabItem
            {
                InventoryId = _inventoryId!.Value,
                MapleId = itemId,
                Quantity = quantity,
                InventoryTab = inventoryTab,
                Slot = toSlot
            };
            TabItems[inventoryTab].Add(toSlot, inventoryTabItem);
            return true;
        }

        inventoryTabItem = default;

        return false;
    }

    // TODO: handle item stacks
    public bool TryRemoveItem(EInventoryTab inventoryTab, short slot, out IInventoryTabItem? inventoryTabItem)
    {
        AssertLoaded();

        if (!TabItems[EInventoryTab.Equipment].TryGetValue(slot, out inventoryTabItem)) return false;
        
        TabItems[EInventoryTab.Equipment].Remove(slot);
        
        return true;
    }

    // TODO: handle item stacks
    public bool TryMoveItem(IInventoryTabItem itemToMove, short toSlot, out IInventoryTabItem? inventoryTabItem)
    {
        AssertLoaded();

        var inventoryTab = TabItems[itemToMove.InventoryTab];
        
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
        if (inventoryTab.Count >= TabCapacity[itemToMove.InventoryTab])
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
    
    public byte GetInventoryTabCapacity(EInventoryTab inventoryTab)
    {
        return TabCapacity[inventoryTab];
    }
    
    public SortedDictionary<short, IInventoryTabItem> GetInventoryTabItems(EInventoryTab inventoryTab)
    {
        return TabItems[inventoryTab];
    }

    public bool TryEquipItem(IInventoryTabItem item, out IInventoryTabItem? inventoryTabItem)
    {
        var targetSlot = item.MapleId switch
        {
            1040006 => EEquipSlot.Top,
            1060006 => EEquipSlot.Bottom,
            1072037 or 1072001 => EEquipSlot.Shoes,
            1322005 => EEquipSlot.Weapon,
            _ => throw new ArgumentException("Unsupported item id")
        };

        TryMoveItem(item, (short)targetSlot, out inventoryTabItem);
        
        return true;
    }
}