using Common.Networking;
using Common.Networking.Enums;
using Common.Networking.Extensions;
using Common.Networking.OperationCodes;
using Common.Networking.Packets.Interfaces;
using LoginServer.Handlers.Packets.Enums;
using LoginServer.Handlers.Packets.Structs;
using Newtonsoft.Json;

namespace LoginServer.Handlers.Packets;

public sealed class ClientLoginPacketHandler : IPacketHandler
{
    public EClientOperationCode Opcode { get; init; } = EClientOperationCode.ClientLogin;

    public Task HandlePacketAsync(GameClient client, GameMessageBuffer buffer)
    {
        var packetInstance = buffer.ParsePacketInstance<ClientLoginPacket>();
        Console.WriteLine($"Received And Serialized Packet Instance: {JsonConvert.SerializeObject(packetInstance)}");
        client.Send(new GameMessage(EServerOperationCode.CheckPasswordResult)
        {
            { EGameMessageType.u8, ELoginResult.Unregistered }
        });
        return Task.CompletedTask;
    }
}