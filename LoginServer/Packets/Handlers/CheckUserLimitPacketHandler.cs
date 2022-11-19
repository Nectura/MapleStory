using Common.Networking;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;

namespace LoginServer.Packets.Handlers;

public sealed class CheckUserLimitPacketHandler : IAsyncPacketHandler
{
    public EClientOperationCode Opcode { get; init; } = EClientOperationCode.CheckUserLimit;

    public Task HandlePacketAsync(IServiceScopeFactory scopeFactory, GameClient client, GameMessageBuffer buffer, CancellationToken cancellationToken = default)
    {
        client.Send(new GameMessageBuffer(EServerOperationCode.CheckUserLimitResult)
                .WriteByte() // OverUserLimit
                .WriteByte() // PopulationLevel [0 - Normal, 1 - Highly Populated, 2 - Full]
        );
        return Task.CompletedTask;
    }
}