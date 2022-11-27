namespace Common.Database.Models;

public class Inventory : IInventory
{
    public Guid Id { get; set; }
    
    public uint CharacterId { get; set; }
    public virtual Character? Character { get; set; }
    
    public byte EquippableTabSlots { get; set; }
    public byte ConsumableTabSlots { get; set; }
    public byte SetupTabSlots { get; set; }
    public byte EtceteraTabSlots { get; set; }
    public byte CashTabSlots { get; set; }
    
    public virtual ICollection<EquippableItem>? EquippableItems { get; set; }
    public virtual ICollection<ConsumableItem>? ConsumableItems { get; set; }
    public virtual ICollection<SetupItem>? SetupItems { get; set; }
    public virtual ICollection<EtceteraItem>? EtceteraItems { get; set; }
}