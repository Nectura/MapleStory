using System.Net;
using Common.Database.Repositories.Interfaces;
using Common.Networking;
using Common.Networking.Extensions;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;
using LoginServer.Packets.Enums;
using LoginServer.Packets.Models;

namespace LoginServer.Packets.Handlers;

public sealed class SelectCharacterPacketHandler : IAsyncPacketHandler
{
    public EClientOperationCode Opcode { get; init; } = EClientOperationCode.SelectCharacter;

    public async Task HandlePacketAsync(IServiceScopeFactory scopeFactory, GameClient client, GameMessageBuffer buffer, CancellationToken cancellationToken = default)
    {        
        await using var scope = scopeFactory.CreateAsyncScope();
        var packetInstance = buffer.ParsePacketInstance<SelectCharacterPacket>();
        var repository = scope.ServiceProvider.GetRequiredService<ICharacterRepository>();
        var character = await repository.FindAsync(packetInstance.CharacterId, cancellationToken);
        var writerBuffer = new GameMessageBuffer(EServerOperationCode.SelectCharacterResult);
        if (character == default)
        {
            writerBuffer
                .WriteByte((byte)ELoginResult.Error)
                .WriteByte();
        }
        else
        {
            writerBuffer
                .WriteByte((byte)ELoginResult.Success)
                .WriteByte()
                .WriteIpEndpoint(new IPEndPoint(IPAddress.Parse("GameServerAddressHere"), 7575))
                .WriteInt(character.Id)
                .WriteByte()
                .WriteInt();
        }
        client.Send(writerBuffer);
    }
}