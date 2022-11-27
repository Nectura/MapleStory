namespace Common.InventoryX.Interfaces;

public interface IInventoryTabItem : IItem
{
    public byte Slot { get; init; }
    public Guid InventoryId { get; init; }
    public bool IsNxItem { get; init; }
}