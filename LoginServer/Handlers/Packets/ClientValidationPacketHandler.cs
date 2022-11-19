using Common.Networking;
using Common.Networking.Enums;
using Common.Networking.OperationCodes;
using Common.Networking.Packets.Interfaces;

namespace LoginServer.Handlers.Packets;

public sealed class ClientValidationPacketHandler : IPacketHandler
{
    public EClientOperationCode Opcode { get; init; } = EClientOperationCode.ClientValidation;

    public Task HandlePacketAsync(GameClient client, GameMessageBuffer buffer)
    {
        client.Send(new GameMessage(EServerOperationCode.SetLoginBackground)
        {
            { EGameMessageType.str, "MapLogin" }
        });
        return Task.CompletedTask;
    }
}