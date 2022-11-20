using Common.Database;
using Common.Database.Repositories;
using Common.Database.Repositories.Interfaces;
using Common.Database.WorkUnits;
using Common.Database.WorkUnits.Interfaces;
using Common.Networking;
using Common.Networking.Configuration;
using Common.Networking.Packets;
using Common.Networking.Packets.Interfaces;
using Common.Services;
using Common.Services.Interfaces;
using LoginServer.Configuration;
using LoginServer.Packets.Handlers;
using LoginServer.Services.Background;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .CreateLogger();

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);
    loggingBuilder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
    loggingBuilder.AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.Warning);
    loggingBuilder.AddSerilog(dispose: true);
});

var dbConStr = builder.Configuration.GetConnectionString("MapleStory");

builder.Services.AddSingleton<IAsyncPacketHandler, ClientValidationPacketHandler>();
builder.Services.AddSingleton<IAsyncPacketHandler, ClientStartPacketHandler>();
builder.Services.AddSingleton<IAsyncPacketHandler, ClientLoginPacketHandler>();
builder.Services.AddSingleton<IAsyncPacketHandler, CheckUserLimitPacketHandler>();
builder.Services.AddSingleton<IAsyncPacketHandler, WorldInfoRequestPacketHandler>();
builder.Services.AddSingleton<IAsyncPacketHandler, SelectWorldPacketHandler>();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountRestrictionRepository, AccountRestrictionRepository>();
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();

builder.Services.AddScoped<IAccountWorkUnit, AccountWorkUnit>();

builder.Services.AddSingleton<GameServer>();
builder.Services.AddSingleton<IPacketProcessor, PacketProcessor>();
builder.Services.AddSingleton<IAuthService, Sha3AuthService>();

builder.Services.AddHostedService<LoginServerBackgroundService>();

builder.Services.AddDbContext<EntityContext>(options =>
    options.UseMySql(dbConStr, ServerVersion.AutoDetect(dbConStr))
        .LogTo(Console.WriteLine, LogLevel.Warning)
        //.EnableSensitiveDataLogging()
        .EnableDetailedErrors());

builder.Services.AddOptions();
builder.Services.Configure<ServerConfig>(builder.Configuration.GetSection("Server"));
builder.Services.Configure<LoginConfig>(builder.Configuration.GetSection("Login"));

var app = builder.Build();

app.Run();