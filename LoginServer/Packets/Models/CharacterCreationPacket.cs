using Common.Networking.Packets.Attributes;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;

namespace LoginServer.Packets.Models;

[PacketHandler(EClientOperationCode.CreateNewCharacter)]
public record CharacterCreationPacket : IPacketStructure
{
    [PacketProperty(0)] public string Name { get; init; } = "";
    [PacketProperty(1)] public uint JobCategory { get; init; }
    [PacketProperty(2)] public ushort SubJob { get; init; }
    [PacketProperty(3)] public uint Face { get; init; }
    [PacketProperty(4)] public uint HairStyle { get; init; }
    [PacketProperty(5)] public uint HairColor { get; init; }
    [PacketProperty(6)] public uint SkinColor { get; init; }
    [PacketProperty(7)] public uint Top { get; init; }
    [PacketProperty(8)] public uint Bottom { get; init; }
    [PacketProperty(9)] public uint Shoes { get; init; }
    [PacketProperty(10)] public uint Weapon { get; init; }
}