using Common.Database.Repositories.Interfaces;
using Common.Networking;
using Common.Networking.Extensions;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;
using LoginServer.Packets.Enums;
using LoginServer.Packets.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginServer.Packets.Handlers;

public sealed class CharacterDeletionHandler : IAsyncPacketHandler
{
    public EClientOperationCode Opcode { get; init; } = EClientOperationCode.DeleteCharacter;

    public async Task HandlePacketAsync(IServiceScopeFactory scopeFactory, GameClient client, GameMessageBuffer buffer, CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var packetInstance = buffer.ParsePacketInstance<CharacterDeletionPacket>();
        var repository = scope.ServiceProvider.GetRequiredService<ICharacterRepository>();
        var hasSucceeded = (await repository.Query(m => m.Id == packetInstance.CharacterId).ExecuteDeleteAsync(cancellationToken)) == 1;
        client.Send(new GameMessageBuffer(EServerOperationCode.DeleteCharacterResult)
            .WriteInt(packetInstance.CharacterId)
            .WriteByte((byte) (hasSucceeded ? ELoginResult.Success : ELoginResult.Error))
        );
    }
}