using Common.InventoryX.Interfaces;

namespace Common.Database.Models;

public class ConsumableItem : IConsumableItem
{
    public Guid Id { get; init; }
    public uint MapleId { get; init; }
    
    public Guid InventoryId { get; init; }
    public virtual Inventory? Inventory { get; set; }
    
    public byte Slot { get; init; }
    public ushort Quantity { get; init; }
    public bool IsNxItem { get; init; }
}