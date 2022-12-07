namespace LoginServer.Configuration;

public sealed class InventoryConfig
{
    public byte EquippableTabSlots { get; set; }
    public byte ConsumableTabSlots { get; set; }
    public byte SetupTabSlots { get; set; }
    public byte EtceteraTabSlots { get; set; }
    public byte CashTabSlots { get; set; }
}