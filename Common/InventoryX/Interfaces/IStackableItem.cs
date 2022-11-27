namespace Common.InventoryX.Interfaces;

public interface IStackableItem : IItem
{
    public ushort Quantity { get; init; }
}