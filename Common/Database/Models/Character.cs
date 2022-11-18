using Common.Database.Models.Interfaces;
using Common.Enums;

namespace Common.Database.Models;

public class Character : ICharacter
{
    public int Id { get; set; }

    public int AccountId { get; set; }
    public virtual Account? Account { get; set; }

    public EWorld WorldId { get; set; }
    public string Name { get; set; } = string.Empty;
    public byte Level { get; set; }
    public int Experience { get; set; }
    public EJob Job { get; set; }
    public short Fame { get; set; }
    public int GachaponExperience { get; set; }
    public short Strength { get; set; }
    public short Dexterity { get; set; }
    public short Luck { get; set; }
    public short Intelligence { get; set; }
    public short MaxHitPoints { get; set; }
    public short MaxManaPoints { get; set; }
    public short HitPoints { get; set; }
    public short ManaPoints { get; set; }
    public int Mesos { get; set; }
    public EGender Gender { get; set; }
    public int HairStyle { get; set; }
    public int HairColor { get; set; }
    public byte Skin { get; set; }
    public int Face { get; set; }
    public short AbilityPoints { get; set; }
    public short SkillPoints { get; set; }
    public int MapId { get; set; }
    public byte SpawnPoint { get; set; }
    public byte BuddyLimit { get; set; }
    public byte EquipmentSlots { get; set; }
    public byte UsableSlots { get; set; }
    public byte SetupSlots { get; set; }
    public byte EtceteraSlots { get; set; }
    public byte CashSlots { get; set; }
    public short X { get; set; }
    public short Y { get; set; }
    public byte Stance { get; set; }
    public short Foothold { get; set; }
    public bool ExperienceLocked { get; set; }
    public bool LevelLocked { get; set; }
}