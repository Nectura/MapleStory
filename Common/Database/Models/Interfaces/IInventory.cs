namespace Common.Database.Models.Interfaces;

public interface IInventory
{
    Guid Id { get; set; }
    uint CharacterId { get; set; }
    byte EquippableTabSlots { get; set; }
    byte ConsumableTabSlots { get; set; }
    byte SetupTabSlots { get; set; }
    byte EtceteraTabSlots { get; set; }
    byte CashTabSlots { get; set; }
}