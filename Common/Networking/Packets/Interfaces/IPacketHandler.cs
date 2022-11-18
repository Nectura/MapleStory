using Common.Networking.OperationCodes;

namespace Common.Networking.Packets.Interfaces;

public interface IPacketHandler
{
    EClientOperationCode Opcode { get; init; }
    Task HandlePacketAsync(GameClient client, GameMessageBuffer buffer);
}