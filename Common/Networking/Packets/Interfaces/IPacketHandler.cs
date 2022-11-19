using Common.Networking.Packets.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Networking.Packets.Interfaces;

public interface IPacketHandler
{
    EClientOperationCode Opcode { get; init; }
    void HandlePacket(IServiceScopeFactory scopeFactory, GameClient client, GameMessageBuffer buffer);
}