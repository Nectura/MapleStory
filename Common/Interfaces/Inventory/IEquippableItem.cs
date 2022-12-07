using Common.Enums;

namespace Common.Interfaces.Inventory;

public interface IEquippableItem : IItem
{
    ushort Strength { get; set; }
    ushort Dexterity { get; set; }
    ushort Luck { get; set; }
    ushort Intelligence { get; set; }
    ushort AttackPower { get; set; }
    ushort MagicalPower { get; set; }
    ushort PhysicalDefense { get; set; }
    ushort MagicalDefense { get; set; }
    ushort HitPoints { get; set; }
    ushort ManaPoints { get; set; }
    ushort Speed { get; set; }
    ushort Jump { get; set; }
    ushort Accuracy { get; set; }
    ushort Avoidability { get; set; }
    byte UpgradesAvailable { get; set; }
    byte UpgradesApplied { get; set; }
    uint Vicious { get; set; } // wtf is this even
    bool CanGrow { get; set; }
    byte GrowthLevel { get; set; }
    uint GrowthExperience { get; set; }
    uint Durability { get; set; }
    EItemPotential Potential { get; set; }
    byte Enchantments { get; set; }
    ushort FirstPotential { get; set; }
    ushort SecondPotential { get; set; }
    ushort ThirdPotential { get; set; }
    ushort FirstSocket { get; set; }
    ushort SecondSocket { get; set; }
    DateTime? ExpirationTime { get; set; }
}