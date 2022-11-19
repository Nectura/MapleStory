using Common.Networking;
using Common.Networking.Enums;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;

namespace LoginServer.Packets.Handlers;

public sealed class ClientValidationPacketHandler : IAsyncPacketHandler
{
    public EClientOperationCode Opcode { get; init; } = EClientOperationCode.ClientValidation;

    public Task HandlePacketAsync(GameClient client, GameMessageBuffer buffer, CancellationToken cancellationToken = default)
    {
        client.Send(new GameMessage(EServerOperationCode.SetLoginBackground)
        {
            { EGameMessageType.str, "MapLogin" }
        });
        return Task.CompletedTask;
    }
}