namespace Common.Database.Models;

public interface IInventory
{
    Guid Id { get; set; }
    uint CharacterId { get; set; }
    Character? Character { get; set; }
    byte EquippableTabSlots { get; set; }
    byte ConsumableTabSlots { get; set; }
    byte SetupTabSlots { get; set; }
    byte EtceteraTabSlots { get; set; }
    byte CashTabSlots { get; set; }
    ICollection<EquippableItem>? EquippableItems { get; set; }
    ICollection<ConsumableItem>? ConsumableItems { get; set; }
    ICollection<SetupItem>? SetupItems { get; set; }
    ICollection<EtceteraItem>? EtceteraItems { get; set; }
}