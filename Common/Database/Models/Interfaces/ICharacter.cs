using Common.Enums;

namespace Common.Database.Models.Interfaces;

public interface ICharacter
{
    uint Id { get; set; }
    uint AccountId { get; set; }
    Guid? InventoryId { get; set; }
    EWorld WorldId { get; set; }
    string Name { get; set; }
    byte Level { get; set; }
    uint Experience { get; set; }
    EJob Job { get; set; }
    ushort SubJob { get; set; }
    ushort Fame { get; set; }
    uint GachaponExperience { get; set; }
    ushort Strength { get; set; }
    ushort Dexterity { get; set; }
    ushort Luck { get; set; }
    ushort Intelligence { get; set; }
    ushort MaxHitPoints { get; set; }
    ushort MaxManaPoints { get; set; }
    ushort HitPoints { get; set; }
    ushort ManaPoints { get; set; }
    uint Mesos { get; set; }
    EGender Gender { get; set; }
    uint HairStyle { get; set; }
    byte HairColor { get; set; }
    byte SkinColor { get; set; }
    uint Face { get; set; }
    ushort AbilityPoints { get; set; }
    ushort SkillPoints { get; set; }
    uint MapId { get; set; }
    byte SpawnPoint { get; set; }
    byte BuddyLimit { get; set; }
    ushort X { get; set; }
    ushort Y { get; set; }
    byte Stance { get; set; }
    ushort Foothold { get; set; }
    bool ExperienceLocked { get; set; }
    bool LevelLocked { get; set; }
}