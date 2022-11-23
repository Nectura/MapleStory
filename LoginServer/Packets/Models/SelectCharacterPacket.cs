using Common.Networking.Packets.Attributes;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;

namespace LoginServer.Packets.Models;

[PacketHandler(EClientOperationCode.SelectCharacter)]
public record SelectCharacterPacket : IPacketStructure
{
    [PacketProperty(0)] public uint CharacterId { get; init; }
}