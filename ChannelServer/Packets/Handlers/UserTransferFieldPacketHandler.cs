using ChannelServer.Packets.Models;
using Common.Networking;
using Common.Networking.Extensions;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;

namespace ChannelServer.Packets.Handlers;

public sealed class UserTransferFieldPacketHandler : IAsyncPacketHandler
{
    public EClientOperationCode Opcode { get; init; } = EClientOperationCode.UserTransferFieldRequest;

    public async Task HandlePacketAsync(IServiceScopeFactory scopeFactory, GameClient client, GameMessageBuffer buffer, CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var packetInstance = buffer.ParsePacketInstance<UserTransferFieldPacket>();
        switch (packetInstance.MapId)
        {
            case 0: // resurrection
                // update stats (hit points to 50, reduce xp unless they have safety charm on, in which case don't reduce any xp)
                // change map to nearest town
                break;
            
            case -1: // portal
                // find the portal the player used
                // null check, enable actions if null
                // otherwise change the map
                break;
            
            default: // admin '/m' command
                if (!client.Account!.IsAdmin)
                {
                    // Enable Actions
                    return;
                }
                // change map here
                break;
        }
    }
}