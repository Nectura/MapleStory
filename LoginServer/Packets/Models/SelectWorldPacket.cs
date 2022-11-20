using Common.Enums;
using Common.Networking.Packets.Attributes;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;

namespace LoginServer.Packets.Models;

[PacketHandler(EClientOperationCode.SelectWorld)]
public record SelectWorldPacket : IPacketStructure
{
    [PacketProperty(0)] public EWorld WorldId { get; init; }
    [PacketProperty(1)] public byte ChannelId { get; init; }
}