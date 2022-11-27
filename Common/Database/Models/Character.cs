using System.ComponentModel.DataAnnotations.Schema;
using Common.Database.Models.Interfaces;
using Common.Enums;
namespace Common.Database.Models;

public class Character : ICharacter
{
    public uint Id { get; set; }

    [ForeignKey(nameof(Account))]
    public uint AccountId { get; set; }
    public virtual Account? Account { get; set; }
    
    [ForeignKey(nameof(Inventory))]
    public Guid InventoryId { get; set; }
    public virtual Inventory? Inventory { get; set; }

    public EWorld WorldId { get; set; }
    public string Name { get; set; } = string.Empty;
    public byte Level { get; set; } = 1;
    public uint Experience { get; set; }
    public EJob Job { get; set; }
    public ushort SubJob { get; set; }
    public ushort Fame { get; set; }
    public uint GachaponExperience { get; set; }
    public ushort Strength { get; set; }
    public ushort Dexterity { get; set; }
    public ushort Luck { get; set; }
    public ushort Intelligence { get; set; }
    public ushort MaxHitPoints { get; set; }
    public ushort MaxManaPoints { get; set; }
    public ushort HitPoints { get; set; }
    public ushort ManaPoints { get; set; }
    public uint Mesos { get; set; }
    public EGender Gender { get; set; }
    public uint HairStyle { get; set; }
    public byte HairColor { get; set; }
    public byte SkinColor { get; set; }
    public uint Face { get; set; }
    public ushort AbilityPoints { get; set; }
    public ushort SkillPoints { get; set; }
    public uint MapId { get; set; }
    public byte SpawnPoint { get; set; }
    public byte BuddyLimit { get; set; }
    public ushort X { get; set; }
    public ushort Y { get; set; }
    public byte Stance { get; set; }
    public ushort Foothold { get; set; }
    public bool ExperienceLocked { get; set; }
    public bool LevelLocked { get; set; }
}