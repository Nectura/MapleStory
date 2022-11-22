using Common.Networking.Packets.Attributes;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;

namespace LoginServer.Packets.Models;

[PacketHandler(EClientOperationCode.CheckDuplicatedID)]
public record CharacterNameCheckPacket : IPacketStructure
{
    [PacketProperty(0)] public string Name { get; init; } = "";
}