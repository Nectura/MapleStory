using Common.Networking.Packets.Attributes;
using Common.Networking.Packets.Interfaces;

namespace LoginServer.Packets.Models;

public record struct AccountLoginPacket : IPacketStructure
{
    [PacketField(0)] public string UserName;
    [PacketField(1)] public string Password;
}