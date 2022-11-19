using Common.Networking.Packets.Enums;

namespace Common.Networking.Packets.Interfaces;

public interface IPacketHandler
{
    EClientOperationCode Opcode { get; init; }
    void HandlePacket(GameClient client, GameMessageBuffer buffer);
}