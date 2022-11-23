using ChannelServer.Chat.Models;
using ChannelServer.Chat.Models.Interfaces;
using ChannelServer.Chat.Services.Interfaces;
using ChannelServer.Packets.Models;
using Common.Networking;
using Common.Networking.Extensions;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;
using Newtonsoft.Json;

namespace ChannelServer.Packets.Handlers;

public sealed class UserChatPacketHandler : IAsyncPacketHandler
{
    public EClientOperationCode Opcode { get; init; } = EClientOperationCode.UserChat;

    public async Task HandlePacketAsync(IServiceScopeFactory scopeFactory, GameClient client, GameMessageBuffer buffer, CancellationToken cancellationToken = default)
    {
        var packetInstance = buffer.ParsePacketInstance<UserChatPacket>();
        if (packetInstance.Text.StartsWith('!'))
        {
            var commandSegments = packetInstance.Text[1..].Split(' ');
            switch (commandSegments[0])
            {
                case "henesys":
                    //var mapId = uint.Parse(commandSegments[1]);
                    //var portal = byte.Parse(commandSegments[2]);
                    //Console.WriteLine($"Received Map Change Command: Map={mapId} , Portal={portal}");
                    client.Character.MapId = 100000000;
                    client.Character.SpawnPoint = 0;
                    SendSetField(client, false);
                    break;
            }
            return;
        }
        await using var scope = scopeFactory.CreateAsyncScope();
        var chatService = scope.ServiceProvider.GetRequiredService<IChatService<IGeneralMessage>>();
        chatService.HandleChatMessage(new GeneralChatMessage
        {
            SenderCharacterId = client.Character.Id,
            MapId = client.Character.MapId,
            Message = packetInstance.Text
        });
    }
    
    private void SendSetField(GameClient client, bool connect)
    {
        var buffer = new GameMessageBuffer(EServerOperationCode.SetField)
            .WriteUInt(client.Channel)
            .WriteUInt((uint)client.World)
            .WriteByte(1) // NotifierMessage
            .WriteInt()
            .WriteBool(connect)
            .WriteUShort(); // notifierCheck
        if (connect)
        {
            var random = new Random();
            buffer
                .WriteInt(random.Next())
                .WriteInt(random.Next())
                .WriteInt(random.Next());
            buffer.WriteCharacterData(client);
        }
        else
        {
            buffer
                .WriteByte()
                .WriteUInt(client.Character.MapId)
                .WriteByte(client.Character.SpawnPoint)
                .WriteUInt(client.Character.HitPoints)
                .WriteBool(false);
        }
        buffer
            .WriteDateTime(DateTime.UtcNow)
            .WriteInt();
        client.Send(buffer);
    }
}