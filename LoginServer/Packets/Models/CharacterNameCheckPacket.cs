using Common.Networking.Packets.Attributes;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;

namespace LoginServer.Packets.Models;

[PacketHandler(EClientOperationCode.CheckDuplicatedID)]
public record struct CharacterNameCheckPacket : IPacketStructure
{
    [PacketField(0)] public string Name;
}