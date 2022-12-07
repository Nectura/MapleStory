using Common.Networking;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;
using Serilog;

namespace LoginServer.Packets.Handlers;

public sealed class ExceptionLogPacketHandler : IAsyncPacketHandler
{
    private readonly ILogger<ExceptionLogPacketHandler> _logger;
    
    public EClientOperationCode Opcode { get; init; } = EClientOperationCode.ExceptionLog;

    public ExceptionLogPacketHandler(ILogger<ExceptionLogPacketHandler> logger)
    {
        _logger = logger;
    }
    
    public Task HandlePacketAsync(IServiceScopeFactory scopeFactory, GameClient client, GameMessageBuffer buffer,
        CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Client {remoteAddress} sent an exception log: {exceptionMsg}", client.EndPoint?.ToString() ?? client.IpAddress.ToString(), buffer.ReadString());
        return Task.CompletedTask;
    }
}