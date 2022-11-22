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

    public Task HandlePacketAsync(IServiceScopeFactory scopeFactory, GameClient client, GameMessageBuffer buffer, CancellationToken cancellationToken = default)
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
            return Task.CompletedTask;
        }
        client.Send(new GameMessageBuffer(EServerOperationCode.UserChat)
            .WriteInt(client.Character.Id)
            .WriteBool() // client.Account.AccountType == EAccountType.GameMaster [needs to match the SendLoginSuccessResult packet]
            .WriteString(packetInstance.Text)
            .WriteBool(packetInstance.BubbleOnly)
        );
        return Task.CompletedTask;
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