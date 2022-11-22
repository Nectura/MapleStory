using System.Reflection;
using Common.Database.Models;
using Common.Networking.Packets.Attributes;

namespace Common.Networking.Extensions;

public static class GameMessageBufferExtensions
{
    private struct PacketProperty
    {
        public PropertyInfo PropertyInfo { get; init; }
        public int? ReadLength { get; init; }
    }
    
    public static T ParsePacketInstance<T>(this GameMessageBuffer buffer) where T : class
    {
        var packetInstance = Activator.CreateInstance<T>();
        var dict = new SortedDictionary<uint, PacketProperty>();
        
        // get the order of each property and append them in a sorted manner to a dictionary
        foreach (var property in packetInstance.GetType().GetProperties())
        {
            var packetOrderAttr = property.GetCustomAttribute<PacketPropertyAttribute>();
            
            if (packetOrderAttr == default) 
                continue;
            
            dict.Add(packetOrderAttr.Order, new PacketProperty
            {
                PropertyInfo = property,
                ReadLength = packetOrderAttr.HasReadLength ? packetOrderAttr.ReadLength : default
            });
        }

        // iterate over the properties in the dictionary and try to read them with our GameMessageBuffer
        foreach (var packetProperty in dict.Values)
        {
            if (!buffer.TryRead(packetProperty.PropertyInfo.PropertyType, packetProperty.ReadLength, out var val)) continue;
            packetProperty.PropertyInfo.SetValue(packetInstance, val);
        }

        return packetInstance;
    } 
    
    public static GameMessageBuffer WriteCharacterInfo(this GameMessageBuffer buffer, Character character)
    {
        buffer.WriteCharacterStats(character);
        buffer.WriteCharacterAppearance(character);
        buffer.WriteByte(); // VAC
        buffer.WriteByte(); // Ranking
        return buffer;
    }
    
    public static GameMessageBuffer WriteCharacterStats(this GameMessageBuffer buffer, Character character)
    {
        buffer.WriteInt(character.Id);
        buffer.WriteFixedString(character.Name, 13);
        buffer.WriteByte((byte)character.Gender);
        buffer.WriteByte(character.SkinColor);
        buffer.WriteUInt(character.Face);
        buffer.WriteUInt(character.HairStyle + character.HairColor);
        buffer.WriteByte(character.Level);
        buffer.WriteUShort((ushort)character.Job);
        buffer.WriteUShort(character.Strength);
        buffer.WriteUShort(character.Dexterity);
        buffer.WriteUShort(character.Intelligence);
        buffer.WriteUShort(character.Luck);
        buffer.WriteUInt(character.HitPoints);
        buffer.WriteUInt(character.MaxHitPoints);
        buffer.WriteUInt(character.ManaPoints);
        buffer.WriteUInt(character.MaxManaPoints);
        buffer.WriteUShort(character.AbilityPoints);
        buffer.WriteUShort(character.SkillPoints);
        buffer.WriteUInt(character.Experience);
        buffer.WriteUShort(character.Fame);
        buffer.WriteUInt(character.GachaponExperience);
        buffer.WriteULong();
        buffer.WriteUInt(character.MapId);
        buffer.WriteByte(character.SpawnPoint);
        buffer.WriteUShort(character.SubJob);
        return buffer;
    }
    
    public static GameMessageBuffer WriteCharacterAppearance(this GameMessageBuffer buffer, Character character)
    {
        buffer.WriteByte((byte)character.Gender);
        buffer.WriteByte(character.SkinColor);
        buffer.WriteUInt(character.Face);
        buffer.WriteBool(false); //megaphone usage
        buffer.WriteUInt(character.HairStyle + character.HairColor);
        buffer.WriteCharacterEquipmentData(character);
        return buffer;
    }
    
    public static GameMessageBuffer WriteCharacterEquipmentData(this GameMessageBuffer buffer, Character character)
    {
        buffer.WriteSByte(-1);
        buffer.WriteSByte(-1);
        buffer.WriteUInt(); //cash weapon
        for (var i = 0; i < 3; i++)
            buffer.WriteUInt(); // pets
        return buffer;
    }
}