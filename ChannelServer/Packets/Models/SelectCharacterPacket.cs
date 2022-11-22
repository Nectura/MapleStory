using Common.Networking.Packets.Attributes;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;

namespace ChannelServer.Packets.Models;

[PacketHandler(EClientOperationCode.UserChat)]
public record UserChatPacket : IPacketStructure
{
    [PacketProperty(0)] public int Tick { get; init; }
    [PacketProperty(1)] public string Text { get; init; } = "";
    [PacketProperty(2)] public bool BubbleOnly { get; init; }
}