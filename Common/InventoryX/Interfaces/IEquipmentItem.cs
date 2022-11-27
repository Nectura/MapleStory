namespace Common.InventoryX.Interfaces;

public interface IEquipmentItem : IItem
{
    public sbyte Slot { get; set; }
}