using Common.Enums;
using Common.Networking.Packets.Attributes;
using Common.Networking.Packets.Interfaces;

namespace LoginServer.Packets.Models;

public record struct SelectWorldPacket : IPacketStructure
{
    [PacketField(0)] public EWorld WorldId;
    [PacketField(1)] public byte ChannelId;
}