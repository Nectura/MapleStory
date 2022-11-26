using Common.Networking;
using Common.Networking.Extensions;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;
using LoginServer.Configuration;
using LoginServer.Packets.Enums;
using LoginServer.Packets.Models;
using Microsoft.Extensions.Options;

namespace LoginServer.Packets.Handlers;

public sealed class SelectWorldPacketHandler : IAsyncPacketHandler
{
    public EClientOperationCode Opcode { get; init; } = EClientOperationCode.SelectWorld;

    public async Task HandlePacketAsync(IServiceScopeFactory scopeFactory, GameClient client, GameMessageBuffer buffer,
        CancellationToken cancellationToken = default)
    {
        if (client.Account == null)
            throw new ArgumentException("The game client is expected to have a reference to the user's account by now!");
        await using var scope = scopeFactory.CreateAsyncScope();
        var loginConfig = scope.ServiceProvider.GetRequiredService<IOptions<LoginConfig>>().Value;
        var packetInstance = buffer.ParsePacketInstance<SelectWorldPacket>();
        client.World = packetInstance.WorldId;
        client.Channel = packetInstance.ChannelId;
        if (!loginConfig.EnablePic)
            SendWorldSelect(client, EPicStatus.Disabled);
        else
            SendWorldSelect(client, client.Account.HasPic ? EPicStatus.Requested : EPicStatus.Unregistered);
    }
    
    private void SendWorldSelect(GameClient client, EPicStatus picStatus)
    {
        if (client.Account == null)
            throw new ArgumentException("The game client is expected to have a reference to the user's account by now!");
        var buffer = new GameMessageBuffer(EServerOperationCode.SelectWorldResult)
            .WriteByte((byte)ELoginResult.Success)
            .WriteByte((byte)(client.Account.Characters?.Count ?? 0));
        if (client.Account.Characters != default)
            foreach (var character in client.Account.Characters)
                buffer.WriteCharacterInfo(character);
        buffer.WriteByte((byte)picStatus) // pic 0 - register 1 - request 2 - disable
            .WriteByte()
            .WriteUInt(client.Account.CharacterSlots)
            .WriteInt() // buy character count
            .WriteLong();
        client.Send(buffer);
    }

    private enum EPicStatus : byte
    {
        Unregistered,
        Requested,
        Disabled
    }
}