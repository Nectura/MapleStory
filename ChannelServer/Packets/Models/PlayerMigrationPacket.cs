using Common.Networking.Packets.Attributes;
using Common.Networking.Packets.Interfaces;

namespace ChannelServer.Packets.Models;

public record struct PlayerMigrationPacket : IPacketStructure
{
    [PacketField(0)] public int CharacterId;
}