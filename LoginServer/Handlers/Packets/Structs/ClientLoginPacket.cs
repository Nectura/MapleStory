using Common.Networking.Packets.Interfaces;

namespace LoginServer.Handlers.Packets.Structs;

[Common.Networking.Packets.Attributes.PacketHandler(1)]
public record ClientLoginPacket : IPacketStructure
{
    [Common.Networking.Packets.Attributes.PacketOrder(0)]
    public string UserName { get; init; }
    
    [Common.Networking.Packets.Attributes.PacketOrder(1)]
    public string Password { get; init; }
}