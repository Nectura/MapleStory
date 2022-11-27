namespace Common.InventoryX.Interfaces;

public interface IEquippableItem : IInventoryTabItem
{
    public ushort Strength { get; init; }
    public ushort Dexterity { get; init; }
    public ushort Luck { get; init; }
    public ushort Intelligence { get; init; }
    public ushort AttackPower { get; init; }
    public ushort MagicalPower { get; init; }
    public ushort PhysicalDefense { get; init; }
    public ushort MagicalDefense { get; init; }
    public ushort HitPoints { get; init; }
    public ushort ManaPoints { get; init; }
    public ushort Speed { get; init; }
    public ushort Jump { get; init; }
    public ushort Accuracy { get; init; }
    public ushort Avoidability { get; init; }
    public ushort UpgradesAvailable { get; init; }
    public ushort UpgradesApplied { get; init; }
    public byte BonusUpgradeSlots { get; init; }
    public string? NameTag { get; init; }
    public DateTime? ExpirationTime { get; init; }
}