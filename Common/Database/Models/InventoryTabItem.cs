using System.ComponentModel.DataAnnotations.Schema;
using Common.Database.Models.Interfaces;
using Common.Enums;

namespace Common.Database.Models;

public class InventoryTabItem : IInventoryTabItem
{
    public Guid Id { get; set; }
    public uint MapleId { get; set; }
    
    [ForeignKey(nameof(Inventory))]
    public Guid InventoryId { get; set; }
    public virtual Inventory? Inventory { get; set; }
    
    public short Slot { get; set; }
    public bool IsNxItem { get; set; }
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
    public uint Vicious { get; set; }
    public byte UpgradesAvailable { get; set; }
    public byte UpgradesApplied { get; set; }
    public string? NameTag { get; set; }
    public bool CanGrow { get; set; }
    public byte GrowthLevel { get; set; }
    public uint GrowthExperience { get; set; }
    public uint Durability { get; set; }
    public EItemPotential Potential { get; set; }
    public byte Enchantments { get; set; }
    public ushort FirstPotential { get; set; }
    public ushort SecondPotential { get; set; }
    public ushort ThirdPotential { get; set; }
    public ushort FirstSocket { get; set; }
    public ushort SecondSocket { get; set; }
    public DateTime? ExpirationTime { get; set; }
    public ushort Quantity { get; set; }
    public ushort Attribute { get; set; } // dunno wtf this is
    public EInventoryTab InventoryTab { get; set; }

    [NotMapped] 
    public bool IsEquipped => Slot < 0;

    public EEquipSlotType GetEquipSlotType()
    {
        return Slot switch
        {
            >= 0 => EEquipSlotType.Equip,
            >= -100 => EEquipSlotType.Equipped,
            >= -1000 => EEquipSlotType.Masked,
            >= -1100 => EEquipSlotType.Dragon,
            >= -1200 => EEquipSlotType.Mechanic,
            _ => throw new IndexOutOfRangeException()
        };
    }
    
    public bool IsRechargable()
    {
        var type = MapleId / 10000;
        return type is 207 or 233;
    }
    
    public void UpdateFromReference(IInventoryTabItem item)
    {
        var properties = item.GetType().GetProperties().Where(m => m.CanWrite);
        foreach (var property in properties)
        {
            var value = property.GetValue(item);
            property.SetValue(this, value);
        }
    }
}