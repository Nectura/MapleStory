using Common.Database.Models.Interfaces;
using Common.Database.WorkUnits.Interfaces;
using Common.Enums;
using Common.Networking;
using Common.Networking.Extensions;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;
using LoginServer.Packets.Enums;
using LoginServer.Packets.Models;

namespace LoginServer.Packets.Handlers;

public sealed class SelectWorldPacketHandler : IAsyncPacketHandler
{
    public EClientOperationCode Opcode { get; init; } = EClientOperationCode.SelectWorld;

    public async Task HandlePacketAsync(IServiceScopeFactory scopeFactory, GameClient client, GameMessageBuffer buffer,
        CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var packetInstance = buffer.ParsePacketInstance<SelectWorldPacket>();
        SendWorldSelect(client, packetInstance.WorldId, packetInstance.ChannelId);
    }
    
    private void SendWorldSelect(GameClient client, EWorld world, byte channelId)
    {
        if (client.Account == null)
            throw new ArgumentException("The game client is expected to have a reference to the user's account by now!");
        client.Send(new GameMessageBuffer(EServerOperationCode.SelectWorldResult)
            .WriteByte((byte)ELoginResult.Success)
            .WriteByte() // character count
            .WriteByte()
            // foreach character here
            // add character entry
            .WriteByte(2) // pic 0 - register 1 - request 2 - disable
            .WriteByte()
            .WriteInt(client.Account.CharacterSlots)
            .WriteInt() // buy character count
            .WriteLong()
        );
    }
}