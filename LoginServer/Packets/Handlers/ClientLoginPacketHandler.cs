using Common.Networking;
using Common.Networking.Enums;
using Common.Networking.Extensions;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;
using LoginServer.Packets.Enums;
using LoginServer.Packets.Models;
using Newtonsoft.Json;

namespace LoginServer.Packets.Handlers;

public sealed class ClientLoginPacketHandler : IAsyncPacketHandler
{
    public EClientOperationCode Opcode { get; init; } = EClientOperationCode.ClientLogin;

    public Task HandlePacketAsync(GameClient client, GameMessageBuffer buffer, CancellationToken cancellationToken = default)
    {
        var packetInstance = buffer.ParsePacketInstance<AccountLoginPacket>();
        Console.WriteLine($"Received And Serialized Packet Instance: {JsonConvert.SerializeObject(packetInstance)}");
        client.Send(new GameMessage(EServerOperationCode.CheckPasswordResult)
        {
            { EGameMessageType.u8, ELoginResult.Unregistered }
        });
        return Task.CompletedTask;
    }
}