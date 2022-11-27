using Common.Database.Models;
using Common.Database.Repositories.Interfaces;
using Common.InventoryX.Enums;
using Common.InventoryX.Interfaces;
using Microsoft.EntityFrameworkCore;

// TODO: handle cash items
public sealed class InventoryService : IBagInventory
{
    public SortedDictionary<byte, IEquippableItem> EquippableItems { get; init; } = new();
    public SortedDictionary<byte, IConsumableItem> ConsumableItems { get; init; } = new();
    public SortedDictionary<byte, ISetupItem> SetupItems { get; init; } = new();
    public SortedDictionary<byte, IEtceteraItem> EtceteraItems { get; init; } = new();
    public Dictionary<EInventoryTab, byte> TabCapacity { get; init; } = new();

    private readonly IInventoryRepository _inventoryRepository;

    public InventoryService(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    private void AssertLoaded()
    {
        if (!EquippableItems.Any() || !ConsumableItems.Any() || !SetupItems.Any() || !EtceteraItems.Any() || !TabCapacity.Any())
            throw new Exception("The inventory hasn't been loaded before use!");
    }

    public async Task LoadAsync(Guid inventoryId, CancellationToken cancellationToken = default)
    {
        var inventory = await _inventoryRepository
            .Query(m => m.Id == inventoryId)
            .Include(m => m.EquippableItems)
            .Include(m => m.ConsumableItems)
            .Include(m => m.SetupItems)
            .Include(m => m.EtceteraItems)
            .FirstOrDefaultAsync(cancellationToken);

        if (inventory == default)
            throw new ArgumentException($"Failed to find an inventory with id {inventoryId}");

        EquippableItems.Clear();
        foreach (var item in inventory.EquippableItems!)
            EquippableItems.Add(item.Slot, item);
        
        ConsumableItems.Clear();
        foreach (var item in inventory.ConsumableItems!)
            ConsumableItems.Add(item.Slot, item);
        
        SetupItems.Clear();
        foreach (var item in inventory.SetupItems!)
            SetupItems.Add(item.Slot, item);
        
        EtceteraItems.Clear();
        foreach (var item in inventory.EtceteraItems!)
            EtceteraItems.Add(item.Slot, item);
        
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

        switch (inventoryTab)
        {
            case EInventoryTab.Equipment when EquippableItems.ContainsKey(slot):
                inventoryTabItem = EquippableItems[slot];
                return true;
            
            case EInventoryTab.Consumables when ConsumableItems.ContainsKey(slot):
                inventoryTabItem = ConsumableItems[slot];
                return true;
            
            case EInventoryTab.Setup when SetupItems.ContainsKey(slot):
                inventoryTabItem = SetupItems[slot];
                return true;
            
            case EInventoryTab.Etcetera when EtceteraItems.ContainsKey(slot):
                inventoryTabItem = EtceteraItems[slot];
                return true;
            
            case EInventoryTab.Cash:
            default:
                throw new NotImplementedException();
        }
        
        return false;
    }

    // TODO: handle item stacks
    public bool TryAddItem(IItem item, out IInventoryTabItem? inventoryTabItem)
    {
        AssertLoaded();

        // TODO: figure out the type of the item by the mapleId or packet
        var a = EInventoryTab.Equipment;
        switch (a)
        {
            case EInventoryTab.Equipment when EquippableItems.Count < TabCapacity[EInventoryTab.Equipment]:
                inventoryTabItem = new EquippableItem();
                EquippableItems.Add((byte)EquippableItems.Count, (EquippableItem)inventoryTabItem);
                return true;
            
            case EInventoryTab.Consumables when ConsumableItems.Count < TabCapacity[EInventoryTab.Consumables]:
                inventoryTabItem = new ConsumableItem();
                ConsumableItems.Add((byte)ConsumableItems.Count, (ConsumableItem)inventoryTabItem);
                return true;
            
            case EInventoryTab.Setup when SetupItems.Count < TabCapacity[EInventoryTab.Setup]:
                inventoryTabItem = new SetupItem();
                SetupItems.Add((byte)SetupItems.Count, (SetupItem)inventoryTabItem);
                return true;
            
            case EInventoryTab.Etcetera when EtceteraItems.Count < TabCapacity[EInventoryTab.Etcetera]:
                inventoryTabItem = new EtceteraItem();
                EtceteraItems.Add((byte)EtceteraItems.Count, (EtceteraItem)inventoryTabItem);
                return true;
            
            case EInventoryTab.Cash:
            default:
                throw new NotImplementedException();
        }
    }

    public bool TryRemoveItem(EInventoryTab inventoryTab, byte slot, out IInventoryTabItem? inventoryTabItem)
    {
        AssertLoaded();
        
        switch (inventoryTab)
        {
            case EInventoryTab.Equipment when EquippableItems.ContainsKey(slot):
                inventoryTabItem = EquippableItems[slot];
                EquippableItems.Remove(slot);
                return true;
            
            case EInventoryTab.Consumables when ConsumableItems.ContainsKey(slot):
                inventoryTabItem = ConsumableItems[slot];
                ConsumableItems.Remove(slot);
                return true;
            
            case EInventoryTab.Setup when SetupItems.ContainsKey(slot):
                inventoryTabItem = SetupItems[slot];
                SetupItems.Remove(slot);
                return true;
            
            case EInventoryTab.Etcetera when EtceteraItems.ContainsKey(slot):
                inventoryTabItem = EtceteraItems[slot];
                EtceteraItems.Remove(slot);
                return true;
            
            case EInventoryTab.Cash:
            default:
                throw new NotImplementedException();
        }
    }

    // TODO: move to an unoccupied slot by adding a new dict entry or replace the values between two entries
    public bool TryMoveItem(IInventoryTabItem itemToMove, byte toSlot, out IInventoryTabItem? inventoryTabItem)
    {
        AssertLoaded();

        throw new NotImplementedException();
    }
}