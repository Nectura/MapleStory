using Common.Networking.Configuration;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Networking.Packets;

public sealed class PacketProcessor : IPacketProcessor
{
    private readonly Dictionary<EClientOperationCode, IAsyncPacketHandler> _packetHandlers;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ServerConfig _serverConfig;
    private readonly ILogger<PacketProcessor> _logger;

    public PacketProcessor(
        IServiceProvider serviceProvider,
        IServiceScopeFactory scopeFactory,
        IOptions<ServerConfig> serverConfig,
        ILogger<PacketProcessor> logger)
    {
        _packetHandlers = serviceProvider.GetServices<IAsyncPacketHandler>().ToDictionary(m => m.Opcode, m => m);
        _scopeFactory = scopeFactory;
        _logger = logger;
        _serverConfig = serverConfig.Value;
    }

    public void ProcessPacket(GameClient client, GameMessageBuffer buffer)
    {
        var opcode = buffer.ReadUShort();

        if (!Enum.IsDefined(typeof(EClientOperationCode), opcode))
        {
            _logger.LogWarning("Received an unknown packet: [{opcode}] {packet}", opcode, BitConverter.ToString(buffer.GetBytes()).Replace('-', ' '));
            return;
        }

        var packetHandlerKey = (EClientOperationCode) opcode;
        var opcodeName = Enum.GetName(typeof(EClientOperationCode), opcode);
        var packet = BitConverter.ToString(buffer.GetBytes()).Replace('-', ' ');
        
        if (!_packetHandlers.ContainsKey(packetHandlerKey))
        {
            _logger.LogWarning("Failed to find a packet handler for {opcode}: {packet}", opcodeName, packet);
            return;
        }
        
        if (_serverConfig.LogAllPackets)
            _logger.LogInformation("Packet Received: [{opcode}]: {packet}", opcodeName, packet);

        Task.Run(async () =>
        {
            try
            {
                await _packetHandlers[packetHandlerKey].HandlePacketAsync(_scopeFactory, client, buffer);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to process packet {opcode}: {errorMsg}{newLine}{stackTrace}", opcodeName, ex.Message, Environment.NewLine, ex.StackTrace);
            }
        });
    }
}