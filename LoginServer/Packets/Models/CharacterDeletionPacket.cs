using Common.Networking.Packets.Attributes;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;

namespace LoginServer.Packets.Models;

[PacketHandler(EClientOperationCode.DeleteCharacter)]
public record CharacterDeletionPacket : IPacketStructure
{
    [PacketProperty(0)] public byte Unknown { get; init; }
    [PacketProperty(1)] public string Birthday { get; init; } = "";
    [PacketProperty(2)] public int CharacterId { get; init; }
}