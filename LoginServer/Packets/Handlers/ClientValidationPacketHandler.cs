using Common.Networking;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;

namespace LoginServer.Packets.Handlers;

public sealed class ClientValidationPacketHandler : IAsyncPacketHandler
{
    public EClientOperationCode Opcode { get; init; } = EClientOperationCode.ClientValidation;

    public Task HandlePacketAsync(IServiceScopeFactory scopeFactory, GameClient client, GameMessageBuffer buffer, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}