using Common.Enums;
using Common.Networking;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;

namespace LoginServer.Packets.Handlers;

public sealed class WorldInfoRequestPacketHandler : IAsyncPacketHandler
{
    public EClientOperationCode Opcode { get; init; } = EClientOperationCode.WorldInfoRequest;

    public Task HandlePacketAsync(IServiceScopeFactory scopeFactory, GameClient client, GameMessageBuffer buffer,
        CancellationToken cancellationToken = default)
    {
        SendWorldInfo(client);
        SendEndOfWorldInfo(client);
        return Task.CompletedTask;
    }
    
    private void SendWorldInfo(GameClient client)
    {
        var worldId = (byte)EWorld.Demethos;
        var responseBuffer = new GameMessageBuffer(EServerOperationCode.WorldInformation);
        responseBuffer
            .WriteByte(worldId) // worldId
            .WriteString(worldId.ToString()) // worldId
            .WriteByte() // worldState
            .WriteString("Event Message") // eventMsg
            .WriteUShort(1) // expRate
            .WriteUShort(1) // dropRate
            .WriteByte(20); // channelCount
        for (byte i = 0; i < 20;) // foreach channel
        {
            responseBuffer
                .WriteString($"{worldId}-{++i}") // channel name [WorldId-ChannelIndex+1]
                .WriteInt() // connected peers count
                .WriteByte(worldId) // world id
                .WriteByte(i) // channel index
                .WriteBool(); // is adult channel
        }
        responseBuffer.WriteShort(); // balloon count
        client.Send(responseBuffer);
    }

    private void SendEndOfWorldInfo(GameClient client)
    {
        client.Send(new GameMessageBuffer(EServerOperationCode.WorldInformation)
            .WriteByte(byte.MaxValue));
    }
}