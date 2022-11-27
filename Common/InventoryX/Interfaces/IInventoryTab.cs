namespace Common.InventoryX.Interfaces;

public interface IInventoryTab
{
    SortedDictionary<byte, IInventoryTabItem> TabItems { get; init; }
    byte Capacity { get; set; }
}