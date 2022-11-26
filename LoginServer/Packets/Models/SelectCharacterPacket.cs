using Common.Networking.Packets.Attributes;
using Common.Networking.Packets.Interfaces;

namespace LoginServer.Packets.Models;

public record struct SelectCharacterPacket : IPacketStructure
{
    [PacketField(0)] public uint CharacterId;
}