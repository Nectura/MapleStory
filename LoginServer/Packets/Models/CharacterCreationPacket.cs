using Common.Networking.Packets.Attributes;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;

namespace LoginServer.Packets.Models;

[PacketHandler(EClientOperationCode.CreateNewCharacter)]
public record struct CharacterCreationPacket : IPacketStructure
{
    [PacketField(0)] public string Name;
    [PacketField(1)] public uint JobCategory;
    [PacketField(2)] public ushort SubJob;
    [PacketField(3)] public uint Face;
    [PacketField(4)] public uint HairStyle;
    [PacketField(5)] public uint HairColor;
    [PacketField(6)] public uint SkinColor;
    [PacketField(7)] public uint Top;
    [PacketField(8)] public uint Bottom;
    [PacketField(9)] public uint Shoes;
    [PacketField(10)] public uint Weapon;
}