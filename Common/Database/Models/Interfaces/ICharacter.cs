using Common.Enums;

namespace Common.Database.Models.Interfaces;

public interface ICharacter
{
    int Id { get; set; }
    int AccountId { get; set; }
    EWorld WorldId { get; set; }
    string Name { get; set; }
    byte Level { get; set; }
    int Experience { get; set; }
    EJob Job { get; set; }
    short Fame { get; set; }
    int GachaponExperience { get; set; }
    short Strength { get; set; }
    short Dexterity { get; set; }
    short Luck { get; set; }
    short Intelligence { get; set; }
    short MaxHitPoints { get; set; }
    short MaxManaPoints { get; set; }
    short HitPoints { get; set; }
    short ManaPoints { get; set; }
    int Mesos { get; set; }
    EGender Gender { get; set; }
    int HairStyle { get; set; }
    int HairColor { get; set; }
    byte Skin { get; set; }
    int Face { get; set; }
    short AbilityPoints { get; set; }
    short SkillPoints { get; set; }
    int MapId { get; set; }
    byte SpawnPoint { get; set; }
    byte BuddyLimit { get; set; }
    byte EquipmentSlots { get; set; }
    byte UsableSlots { get; set; }
    byte SetupSlots { get; set; }
    byte EtceteraSlots { get; set; }
    byte CashSlots { get; set; }
    short X { get; set; }
    short Y { get; set; }
    byte Stance { get; set; }
    short Foothold { get; set; }
    bool ExperienceLocked { get; set; }
    bool LevelLocked { get; set; }
}