namespace Common.Interfaces.Inventory;

public interface IStackableItem : IItem
{
    public ushort Quantity { get; set; }
}