using System.Reflection;
using Common.Database.Models.Interfaces;
using Common.Enums;
using Common.Interfaces.Inventory;
using Common.Models.Structs;
using Common.Networking.Packets.Attributes;

namespace Common.Networking.Extensions;

public static class GameMessageBufferExtensions
{
    private struct PacketField
    {
        public FieldInfo MemberInfo { get; init; }
        public int? ReadLength { get; init; }
    }

    public static T ParsePacketInstance<T>(this GameMessageBuffer buffer) where T : struct
    {
        var packetInstance = Activator.CreateInstance<T>();
        var dict = new SortedDictionary<uint, PacketField>();

        // get the order of each property and append them in a sorted manner to a dictionary
        foreach (var member in packetInstance.GetType().GetFields())
        {
            var packetOrderAttr = member.GetCustomAttribute<PacketFieldAttribute>();

            if (packetOrderAttr == default)
                continue;

            dict.Add(packetOrderAttr.Order, new PacketField
            {
                MemberInfo = member,
                ReadLength = packetOrderAttr.HasReadLength ? packetOrderAttr.ReadLength : default
            });
        }

        // iterate over the properties in the dictionary and try to read them with our GameMessageBuffer
        foreach (var packetMember in dict.Values)
        {
            if (!buffer.TryRead(packetMember.MemberInfo.FieldType, packetMember.ReadLength, out var val)) continue;
            packetMember.MemberInfo.SetValueDirect(__makeref(packetInstance), val!);
        }

        return packetInstance;
    }

    public static GameMessageBuffer WriteCharacterInfo(this GameMessageBuffer buffer, ICharacter character,
        IInventoryService inventoryService)
    {
        buffer.WriteCharacterStats(character);
        buffer.WriteCharacterAppearance(character, inventoryService);
        buffer.WriteByte(); // VAC
        buffer.WriteByte(); // Ranking
        return buffer;
    }

    public static GameMessageBuffer WriteCharacterStats(this GameMessageBuffer buffer, ICharacter character)
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

    public static GameMessageBuffer WriteCharacterAppearance(this GameMessageBuffer buffer, ICharacter character,
        IInventoryService inventoryService)
    {
        buffer.WriteByte((byte)character.Gender);
        buffer.WriteByte(character.SkinColor);
        buffer.WriteUInt(character.Face);
        buffer.WriteBool(false); //megaphone usage
        buffer.WriteUInt(character.HairStyle + character.HairColor);
        buffer.WriteCharacterEquipmentData(character, inventoryService);
        return buffer;
    }

    public static GameMessageBuffer WriteCharacterEquipmentData(this GameMessageBuffer buffer, ICharacter character,
        IInventoryService inventoryService)
    {
        var regularClothes = new Dictionary<sbyte, uint>();
        var cashClothes = new Dictionary<sbyte, uint>();

        foreach (var equippedItem in inventoryService.GetInventoryTabItems(EInventoryTab.Equipment))
        {
            var slot = Math.Abs(equippedItem.Key);
            if (slot > 100 && slot != 111)
                regularClothes.Add((sbyte)(slot - 100), equippedItem.Value.MapleId);
            else
                regularClothes.Add((sbyte)slot, equippedItem.Value.MapleId);
        }

        foreach (var item in regularClothes)
        {
            buffer.WriteSByte(item.Key);
            buffer.WriteUInt(item.Value);
        }

        buffer.WriteSByte(-1);

        foreach (var item in cashClothes)
        {
            buffer.WriteSByte(item.Key);
            buffer.WriteUInt(item.Value);
        }

        buffer.WriteSByte(-1);

        buffer.WriteUInt(); //cash weapon
        for (var i = 0; i < 3; i++)
            buffer.WriteUInt(); // pets

        return buffer;
    }

    public static GameMessageBuffer WriteCharacterData(this GameMessageBuffer buffer, GameClient client,
        ECharacterDataType dataType = ECharacterDataType.All)
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
            if (client.Character.Inventory == null)
                throw new NullReferenceException("Expected the character's inventory to be already loaded at this point!");

            if (dataType.HasFlag(ECharacterDataType.ItemSlotEquip))
                buffer.WriteByte(client.Character.Inventory.EquippableTabSlots);

            if (dataType.HasFlag(ECharacterDataType.ItemSlotConsume))
                buffer.WriteByte(client.Character.Inventory.ConsumableTabSlots);

            if (dataType.HasFlag(ECharacterDataType.ItemSlotInstall))
                buffer.WriteByte(client.Character.Inventory.SetupTabSlots);

            if (dataType.HasFlag(ECharacterDataType.ItemSlotEtc))
                buffer.WriteByte(client.Character.Inventory.EtceteraTabSlots);

            if (dataType.HasFlag(ECharacterDataType.ItemSlotCash))
                buffer.WriteByte(client.Character.Inventory.CashTabSlots);
        }

        if (dataType.HasFlag(ECharacterDataType.AdminShopCount))
        {
            buffer
                .WriteUInt()
                .WriteUInt();
        }

        if (dataType.HasFlag(ECharacterDataType.ItemSlotEquip))
        {
            foreach (var slotType in Enum.GetValues<EEquipSlotType>())
            {
                foreach (var (slot, item) in client.InventoryServices[client.Character.Id]
                             .GetInventoryTabItems(EInventoryTab.Equipment)
                             .Where(m => m.Value.GetEquipSlotType() == slotType))
                {
                    buffer.WriteShort((short)(Math.Abs(slot) % 100));
                    buffer.WriteEquipInfo(item);
                }
                buffer.WriteUShort();
            }
        }

        if (dataType.HasFlag(ECharacterDataType.ItemSlotConsume))
        {
            foreach (var item in client.InventoryServices[client.Character.Id]
                         .GetInventoryTabItems(EInventoryTab.Consumables))
            {
                buffer
                    .WriteByte((byte)item.Key)
                    .WriteItemBundleInfo(item.Value);
            }
            buffer.WriteByte();
        }

        if (dataType.HasFlag(ECharacterDataType.ItemSlotInstall))
        {
            foreach (var item in client.InventoryServices[client.Character.Id]
                         .GetInventoryTabItems(EInventoryTab.Setup))
            {
                buffer
                    .WriteByte((byte)item.Key)
                    .WriteItemBundleInfo(item.Value);
            }
            buffer.WriteByte();
        }

        if (dataType.HasFlag(ECharacterDataType.ItemSlotEtc))
        {
            foreach (var item in client.InventoryServices[client.Character.Id].GetInventoryTabItems(EInventoryTab.Etcetera))
            {
                buffer
                    .WriteByte((byte)item.Key)
                    .WriteItemBundleInfo(item.Value);
            }
            buffer.WriteByte();
        }

        if (dataType.HasFlag(ECharacterDataType.ItemSlotCash))
        {
            foreach (var item in client.InventoryServices[client.Character.Id]
                         .GetInventoryTabItems(EInventoryTab.Cash))
            {
                buffer
                    .WriteByte((byte)item.Key)
                    .WriteItemBundleInfo(item.Value);
            }
            buffer.WriteByte();
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

    private static GameMessageBuffer WriteEquipInfo(this GameMessageBuffer buffer, IEquippableItem item)
    {
        buffer
            .WriteByte((byte)EItemType.Equip)
            .WriteItemInfo(item)
            .WriteByte(item.UpgradesAvailable)
            .WriteByte(item.UpgradesApplied)
            .WriteUShort(item.Strength)
            .WriteUShort(item.Dexterity)
            .WriteUShort(item.Intelligence)
            .WriteUShort(item.Luck)
            .WriteUShort(item.HitPoints)
            .WriteUShort(item.ManaPoints)
            .WriteUShort(item.AttackPower)
            .WriteUShort(item.MagicalPower)
            .WriteUShort(item.PhysicalDefense)
            .WriteUShort(item.MagicalDefense)
            .WriteUShort(item.Accuracy)
            .WriteUShort(item.Avoidability)
            .WriteUShort() // craft?
            .WriteUShort(item.Speed)
            .WriteUShort(item.Jump)
            .WriteString(item.NameTag ?? string.Empty)
            .WriteUShort() // attribute?
            .WriteBool(item.CanGrow) // is level-up type?
            .WriteByte(item.GrowthLevel)
            .WriteUInt(item.GrowthExperience)
            .WriteUInt(item.Durability)
            .WriteUInt(item.Vicious)
            .WriteByte((byte)item.Potential)
            .WriteByte(item.Enchantments)
            .WriteUShort(item.FirstPotential)
            .WriteUShort(item.SecondPotential)
            .WriteUShort(item.ThirdPotential)
            .WriteUShort(item.FirstSocket)
            .WriteUShort(item.SecondSocket);

        if (!item.IsNxItem)
            buffer.WriteULong();

        buffer
            .WriteULong()
            .WriteUInt();

        return buffer;
    }

    private static GameMessageBuffer WriteItemInfo(this GameMessageBuffer buffer, IItem item)
    {
        buffer
            .WriteUInt(item.MapleId)
            .WriteBool(item.IsNxItem);

        if (item.IsNxItem)
            buffer.WriteULong(); // cash item SN

        buffer
            .WritePermanentDateTime() // expiration date or perma
            .WriteInt(-1); // bag id

        return buffer;
    }
    
    private static GameMessageBuffer WriteItemBundleInfo(this GameMessageBuffer buffer, IStackableItem item)
    {
        buffer
            .WriteByte((byte)EItemType.Bundle)
            .WriteItemInfo(item)
            .WriteUShort(item.Quantity)
            .WriteString(item.NameTag ?? string.Empty)
            .WriteUShort(item.Attribute);

        if (item.IsRechargable())
            buffer.WriteULong();
        
        return buffer;
    }
}