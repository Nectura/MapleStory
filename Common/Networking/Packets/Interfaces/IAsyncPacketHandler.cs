using Common.Networking.Packets.Enums;

namespace Common.Networking.Packets.Interfaces;

public interface IAsyncPacketHandler
{
    EClientOperationCode Opcode { get; init; }
    Task HandlePacketAsync(GameClient client, GameMessageBuffer buffer, CancellationToken cancellationToken = default);
}