using Common.Networking;
using Common.Networking.Extensions;
using Common.Networking.OperationCodes;
using Common.Networking.Packets.Interfaces;
using Newtonsoft.Json;

namespace LoginServer.Handlers.Packets;

public sealed class ClientLoginPacketHandler : IPacketHandler
{
    public EClientOperationCode Opcode { get; init; } = EClientOperationCode.ClientLogin;

    public Task HandlePacketAsync(GameClient client, GameMessageBuffer buffer)
    {
        GameMessageBufferExtensions.Read<Structs.ClientLoginPacket>(buffer, out var packetInstance);
        Console.WriteLine($"Received And Serialized Packet Instance: {JsonConvert.SerializeObject(packetInstance)}");
        return Task.CompletedTask;
    }
}