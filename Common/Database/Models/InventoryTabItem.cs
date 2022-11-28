using System.ComponentModel.DataAnnotations.Schema;
using Common.Database.Models.Interfaces;
using Common.Enums;

namespace Common.Database.Models;

public class InventoryTabItem : IInventoryTabItem
{
    public Guid Id { get; init; }
    public uint MapleId { get; init; }
    
    [ForeignKey(nameof(Inventory))]
    public Guid InventoryId { get; init; }
    public virtual Inventory? Inventory { get; set; }
    
    public byte Slot { get; set; }
    public bool IsNxItem { get; init; }
    public ushort Strength { get; set; }
    public ushort Dexterity { get; set; }
    public ushort Luck { get; set; }
    public ushort Intelligence { get; set; }
    public ushort AttackPower { get; set; }
    public ushort MagicalPower { get; set; }
    public ushort PhysicalDefense { get; set; }
    public ushort MagicalDefense { get; set; }
    public ushort HitPoints { get; set; }
    public ushort ManaPoints { get; set; }
    public ushort Speed { get; set; }
    public ushort Jump { get; set; }
    public ushort Accuracy { get; set; }
    public ushort Avoidability { get; set; }
    public ushort UpgradesAvailable { get; set; }
    public ushort UpgradesApplied { get; set; }
    public byte BonusUpgradeSlots { get; set; }
    public string? NameTag { get; set; }
    public DateTime? ExpirationTime { get; set; }
    public ushort Quantity { get; set; }
    public EInventoryTab InventoryTab { get; init; }
}