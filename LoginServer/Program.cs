using Common.Database;
using Common.Networking.Packets.Interfaces;
using Common.Networking.Services;
using LoginServer.Handlers.Packets;
using LoginServer.Services.Background;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var dbConStr = builder.Configuration.GetConnectionString("MapleStory");

#region Packet Handlers
builder.Services.AddSingleton<IPacketHandler, ClientValidationPacketHandler>();
builder.Services.AddSingleton<IPacketHandler, ClientLoginPacketHandler>();
#endregion

builder.Services.AddSingleton<PacketProcessor>();

builder.Services.AddHostedService<LoginServerBackgroundService>();

builder.Services.AddDbContext<EntityContext>(options =>
    options.UseMySql(dbConStr, ServerVersion.AutoDetect(dbConStr))
        .LogTo(Console.WriteLine, LogLevel.Warning)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors());

var app = builder.Build();

app.Run();