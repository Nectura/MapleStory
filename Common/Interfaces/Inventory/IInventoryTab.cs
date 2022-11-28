using Common.Database.Models.Interfaces;

namespace Common.Interfaces.Inventory;

public interface IInventoryTab
{
    SortedDictionary<byte, IInventoryTabItem> TabItems { get; init; }
    byte Capacity { get; set; }
}