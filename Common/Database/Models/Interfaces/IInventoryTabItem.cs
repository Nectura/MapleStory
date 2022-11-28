using Common.Enums;
using Common.Interfaces.Inventory;

namespace Common.Database.Models.Interfaces;

public interface IInventoryTabItem : IEquippableItem, IConsumableItem, ISetupItem, IEtceteraItem
{
    EInventoryTab InventoryTab { get; init; }
    byte Slot { get; set; }
    
    Guid InventoryId { get; init; }
    Inventory? Inventory { get; set; }
    
    bool IsNxItem { get; init; }
}