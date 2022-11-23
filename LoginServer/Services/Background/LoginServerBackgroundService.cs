﻿using Common.Enums;
using Common.Networking;
using Common.Networking.Abstract;
using Common.Networking.Configuration;
using LoginServer.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace LoginServer.Services.Background;

public sealed class LoginServerBackgroundService : IHostedService
{
    private readonly ILoginServer _loginServer;
    private readonly ILogger<LoginServerBackgroundService> _logger;

    public LoginServerBackgroundService(
        ILoginServer loginServer,
        ILogger<LoginServerBackgroundService> logger)
    {
        _loginServer = loginServer;
        _logger = logger;
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