using Common.Networking.Packets.Attributes;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;

namespace LoginServer.Packets.Models;

[PacketHandler(EClientOperationCode.ClientLogin)]
public record AccountLoginPacket : IPacketStructure
{
    [PacketProperty(0)] public string UserName { get; init; } = "";
    [PacketProperty(1)] public string Password { get; init; } = "";
}