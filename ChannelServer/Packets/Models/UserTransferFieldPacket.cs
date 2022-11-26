using Common.Networking.Packets.Attributes;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;

namespace ChannelServer.Packets.Models;

[PacketHandler(EClientOperationCode.UserTransferFieldRequest)]
public record struct UserTransferFieldPacket : IPacketStructure
{
    [PacketField(0)] public byte Portals;
    [PacketField(1)] public int MapId;
    [PacketField(2)] public string PortalLabel;
    [PacketField(3)] public byte Unknown;
    [PacketField(4)] public bool Wheel;
}