using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Networking.Packets.Services;

public sealed class PacketProcessor
{
    private readonly Dictionary<EClientOperationCode, IAsyncPacketHandler> _packetHandlers;

    public PacketProcessor(IServiceProvider serviceProvider)
    {
        _packetHandlers = serviceProvider.GetServices<IAsyncPacketHandler>().ToDictionary(m => m.Opcode, m => m);
    }

    public void ProcessPacket(GameClient client, GameMessageBuffer buffer)
    {
        var opcodeVal = buffer.Read16U();

        if (!Enum.IsDefined(typeof(EClientOperationCode), opcodeVal))
        {
            Console.WriteLine($"Client sent an unhandled packet ({opcodeVal:X}): {BitConverter.ToString(buffer.GetBytes()).Replace('-', ' ')}");
            return;
        }

        var opcode = (EClientOperationCode) opcodeVal;
        
        if (!_packetHandlers.ContainsKey(opcode))
        {
            Console.WriteLine($"Client sent an unregistered packet ({opcode:X}): {BitConverter.ToString(buffer.GetBytes()).Replace('-', ' ')}");
            return;
        }
        
        Task.Run(async () => await _packetHandlers[opcode].HandlePacketAsync(client, buffer));
    }
}