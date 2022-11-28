namespace Common.Interfaces.Inventory;

public interface IEquipmentItem : IItem
{
    public sbyte Slot { get; set; }
}