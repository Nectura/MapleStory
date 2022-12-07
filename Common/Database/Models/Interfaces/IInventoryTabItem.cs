using Common.Enums;
using Common.Interfaces.Inventory;

namespace Common.Database.Models.Interfaces;

public interface IInventoryTabItem : IEquippableItem, IConsumableItem, ISetupItem, IEtceteraItem
{
    EInventoryTab InventoryTab { get; set; }
    short Slot { get; set; }
    Guid InventoryId { get; set; }
    void UpdateFromReference(IInventoryTabItem item);
    EEquipSlotType GetEquipSlotType();
}