using Common.InventoryX.Interfaces;

namespace Common.Database.Models;

public class EtceteraItem : IEtceteraItem
{
    public Guid Id { get; init; }
    public uint MapleId { get; init; }

    public Guid InventoryId { get; init; }
    public virtual Inventory? Inventory { get; set; }
    
    public ushort Quantity { get; init; }
    public byte Slot { get; init; }   
    public bool IsNxItem { get; init; }
}