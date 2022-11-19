using Common.Networking.Packets.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Networking.Packets.Interfaces;

public interface IAsyncPacketHandler
{
    EClientOperationCode Opcode { get; init; }
    Task HandlePacketAsync(IServiceScopeFactory scopeFactory, GameClient client, GameMessageBuffer buffer, CancellationToken cancellationToken = default);
}