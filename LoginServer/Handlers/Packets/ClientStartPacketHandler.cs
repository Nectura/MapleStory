using Common.Networking;
using Common.Networking.Enums;
using Common.Networking.OperationCodes;
using Common.Networking.Packets.Interfaces;

namespace LoginServer.Handlers.Packets;

public sealed class ClientStartPacketHandler : IPacketHandler
{
    public EClientOperationCode Opcode { get; init; } = EClientOperationCode.ClientStart;

    public Task HandlePacketAsync(GameClient client, GameMessageBuffer buffer)
    {
        client.Send(new GameMessage(EServerOperationCode.SetLoginBg)
        {
            { EGameMessageType.str, "MapLogin" }
        });
        return Task.CompletedTask;
    }
}