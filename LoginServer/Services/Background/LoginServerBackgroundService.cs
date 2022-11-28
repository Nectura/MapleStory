using LoginServer.Services.Interfaces;

namespace LoginServer.Services.Background;

public sealed class LoginServerBackgroundService : IHostedService
{
    private readonly ILoginServer _loginServer;
    private readonly ILogger<LoginServerBackgroundService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public LoginServerBackgroundService(
        ILoginServer loginServer,
        ILogger<LoginServerBackgroundService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _loginServer = loginServer;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _loginServer.Start();

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _loginServer.DisposeAsync();
    }
}