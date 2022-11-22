using Common.Database.Repositories.Interfaces;
using Common.Networking;
using Common.Networking.Extensions;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;
using LoginServer.Packets.Models;

namespace LoginServer.Packets.Handlers;

public sealed class CharacterNameCheckHandler : IAsyncPacketHandler
{
    public EClientOperationCode Opcode { get; init; } = EClientOperationCode.CheckDuplicatedID;

    public async Task HandlePacketAsync(IServiceScopeFactory scopeFactory, GameClient client, GameMessageBuffer buffer, CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var packetInstance = buffer.ParsePacketInstance<CharacterNameCheckPacket>();
        var repository = scope.ServiceProvider.GetRequiredService<ICharacterRepository>();
        client.Send(new GameMessageBuffer(EServerOperationCode.CheckDuplicatedIDResult)
            .WriteString(packetInstance.Name)
            .WriteBool(await repository.AnyAsync(m => m.Name == packetInstance.Name, cancellationToken))
        );
    }
}