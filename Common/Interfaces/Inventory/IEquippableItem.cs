namespace Common.Interfaces.Inventory;

public interface IEquippableItem : IItem
{
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
}