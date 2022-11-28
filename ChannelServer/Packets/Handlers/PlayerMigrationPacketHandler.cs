using ChannelServer.Packets.Models;
using ChannelServer.Services.Interfaces;
using Common.Database.Repositories.Interfaces;
using Common.Enums;
using Common.Networking;
using Common.Networking.Extensions;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChannelServer.Packets.Handlers;

public sealed class PlayerMigrationPacketHandler : IAsyncPacketHandler
{
    public EClientOperationCode Opcode { get; init; } = EClientOperationCode.PlayerMigration;

    public async Task HandlePacketAsync(IServiceScopeFactory scopeFactory, GameClient client, GameMessageBuffer buffer, CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var packetInstance = buffer.ParsePacketInstance<PlayerMigrationPacket>();
        var repository = scope.ServiceProvider.GetRequiredService<ICharacterRepository>();
        var character = await repository
            .Query(m => m.Id == packetInstance.CharacterId)
            .Include(m => m.Account)
            .Include(m => m.Inventory).ThenInclude(m => m!.TabItems)
            .FirstOrDefaultAsync(cancellationToken);
        if (character == default)
            throw new ArgumentException("Invalid character id");
        client.World = EWorld.Kradia;
        client.Channel = 0; // channel index
        client.Character = character;
        client.Account = character.Account!;
        var channelServer = scope.ServiceProvider.GetRequiredService<IChannelServer>();
        channelServer.ConnectedPeers.Remove(client.Account.Id, out _);
        channelServer.ConnectedPeers.TryAdd(client.Account.Id, client);
        SendSetField(client, true);
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
                .WriteUInt(client.Character!.MapId)
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