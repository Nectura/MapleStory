using Common.Networking.Packets.Attributes;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;

namespace ChannelServer.Packets.Models;

[PacketHandler(EClientOperationCode.PlayerMigration)]
public record PlayerMigrationPacket : IPacketStructure
{
    [PacketProperty(0)] public int CharacterId { get; init; }
}