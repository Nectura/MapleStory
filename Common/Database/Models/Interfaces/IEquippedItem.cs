using Common.Interfaces.Inventory;

namespace Common.Database.Models.Interfaces;

public interface IEquippedItem : IEquippableItem
{
    sbyte Slot { get; set; }
}