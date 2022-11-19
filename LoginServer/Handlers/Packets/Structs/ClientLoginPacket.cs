using Common.Networking.OperationCodes;
using Common.Networking.Packets.Attributes;
using Common.Networking.Packets.Interfaces;

namespace LoginServer.Handlers.Packets.Structs;

[PacketHandler(EClientOperationCode.ClientLogin)]
public record ClientLoginPacket : IPacketStructure
{
    [PacketProperty(0)] public string UserName { get; init; } = "";
    [PacketProperty(1)] public string Password { get; init; } = "";
}