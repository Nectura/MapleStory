using Common.Networking.Packets.Attributes;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;

namespace LoginServer.Packets.Models;

[PacketHandler(EClientOperationCode.DeleteCharacter)]
public record struct CharacterDeletionPacket : IPacketStructure
{
    [PacketField(0)] public byte Unknown;
    [PacketField(1)] public string Birthday;
    [PacketField(2)] public int CharacterId;
}