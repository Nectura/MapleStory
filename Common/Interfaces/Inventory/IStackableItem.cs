namespace Common.Interfaces.Inventory;

public interface IStackableItem : IItem
{
    ushort Attribute { get; set; }
    ushort Quantity { get; set; }
    bool IsRechargable();
}
