using Common.Networking.Packets.Attributes;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;

namespace ChannelServer.Packets.Models;

[PacketHandler(EClientOperationCode.UserChat)]
public record struct UserChatPacket : IPacketStructure
{
    [PacketField(0)] public int Tick;
    [PacketField(1)] public string Text;
    [PacketField(2)] public bool BubbleOnly;
}