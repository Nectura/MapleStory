using System.Reflection;
using Common.Database.Models;
using Common.Enums;
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
        buffer.WriteUInt(character.Id);
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
    
    public static GameMessageBuffer WriteCharacterData(this GameMessageBuffer buffer, GameClient client, ECharacterDataType dataType = ECharacterDataType.All)
    {
        buffer
            .WriteULong((ulong)dataType)
            .WriteByte() // combat orders
            .WriteByte();
        if (dataType.HasFlag(ECharacterDataType.Character))
        {
            buffer
                .WriteCharacterStats(client.Character)
                .WriteByte(client.Character.BuddyLimit)
                .WriteBool(false)
                .WriteBool(false)
                .WriteBool(false);
        }
        if (dataType.HasFlag(ECharacterDataType.Money))
            buffer.WriteUInt(client.Character.Mesos);
        if (dataType.HasFlag(ECharacterDataType.InventorySize))
        {
            if (dataType.HasFlag(ECharacterDataType.ItemSlotEquip))
                buffer.WriteByte(client.Character.EquipmentSlots);
            
            if (dataType.HasFlag(ECharacterDataType.ItemSlotConsume))
                buffer.WriteByte(client.Character.UsableSlots);
            
            if (dataType.HasFlag(ECharacterDataType.ItemSlotInstall))
                buffer.WriteByte(client.Character.SetupSlots);
            
            if (dataType.HasFlag(ECharacterDataType.ItemSlotEtc))
                buffer.WriteByte(client.Character.EtceteraSlots);
            
            if (dataType.HasFlag(ECharacterDataType.ItemSlotCash))
                buffer.WriteByte(client.Character.CashSlots);
        }
        if (dataType.HasFlag(ECharacterDataType.AdminShopCount))
        {
            buffer
                .WriteUInt()
                .WriteUInt();
        }

        if (dataType.HasFlag(ECharacterDataType.ItemSlotEquip))
        {
            buffer
                .WriteUShort()
                .WriteUShort()
                .WriteUShort()
                .WriteUShort()
                .WriteUShort();
        }
        if (dataType.HasFlag(ECharacterDataType.ItemSlotConsume))
        {
            buffer
                .WriteByte();
        }
        if (dataType.HasFlag(ECharacterDataType.ItemSlotInstall))
        {
            buffer
                .WriteByte();
        }
        if (dataType.HasFlag(ECharacterDataType.ItemSlotEtc))
        {
            buffer
                .WriteByte();
        }
        if (dataType.HasFlag(ECharacterDataType.ItemSlotCash))
        {
            buffer
                .WriteByte();
        }
        buffer.WriteInt(-1);
        if (dataType.HasFlag(ECharacterDataType.SkillRecord))
        {
            buffer.WriteUShort(); //skills
        }
        if (dataType.HasFlag(ECharacterDataType.SkillCooltime))
        {
            buffer.WriteUShort(); //cooldowns
        }
        if (dataType.HasFlag(ECharacterDataType.QuestRecord))
        {
            buffer.WriteUShort(); //ongoing quests
        }
        if (dataType.HasFlag(ECharacterDataType.QuestComplete))
        {
            buffer.WriteUShort(); //completed quests
        }
        if (dataType.HasFlag(ECharacterDataType.MinigameRecord))
        {
            buffer.WriteUShort(); //minigames
        }
        if (dataType.HasFlag(ECharacterDataType.CoupleRecord))
        {
            buffer
                .WriteUShort() //couple
                .WriteUShort() //friend
                .WriteUShort(); //marriage
        }
        if (dataType.HasFlag(ECharacterDataType.MapTransfer))
        {
            //teleport rocks
            for (var i = 0; i < 5; i++)
                buffer.WriteUInt();
            for (var i = 0; i < 10; i++)
                buffer.WriteUInt();
            for (var i = 0; i < 13; i++)
                buffer.WriteUInt();
            for (var i = 0; i < 13; i++)
                buffer.WriteUInt();
        }
        if (dataType.HasFlag(ECharacterDataType.QuestRecordEx))
        {
            buffer.WriteUShort(); //extended quests
        }
        if (dataType.HasFlag(ECharacterDataType.WildHunterInfo))
        {
            /* if (SkillProvider.IsWildHunter(character.Job))
            {
                message.Write1(0);
                for (int i = 0; i < 5; i++) //jaguars
                    message.Write4(0);
            } */
        }
        if (dataType.HasFlag(ECharacterDataType.QuestCompleteOld))
        {
            buffer.WriteUShort();
        }
        if (dataType.HasFlag(ECharacterDataType.VisitorLog))
        {
            buffer.WriteUShort();
        }
        if (dataType.HasFlag(ECharacterDataType.VisitorLog1))
        {
            buffer.WriteUShort();
        }
        buffer.WriteUShort();
        return buffer;
    }
}