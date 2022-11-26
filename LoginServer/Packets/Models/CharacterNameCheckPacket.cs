using Common.Networking.Packets.Attributes;
using Common.Networking.Packets.Interfaces;

namespace LoginServer.Packets.Models;

public record struct CharacterNameCheckPacket : IPacketStructure
{
    [PacketField(0)] public string Name;
}