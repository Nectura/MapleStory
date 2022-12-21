using ChannelServer.Configuration;
using ChannelServer.Packets.Handlers;
using ChannelServer.Services.Background;
using ChannelServer.Services.Interfaces;
using ChannelServer.Systems.Chat.Models.Interfaces;
using ChannelServer.Systems.Chat.Services;
using ChannelServer.Systems.Chat.Services.Interfaces;
using Common.Database;
using Common.Database.Interfaces;
using Common.Database.Repositories;
using Common.Database.Repositories.Interfaces;
using Common.Database.WorkUnits;
using Common.Database.WorkUnits.Interfaces;
using Common.Interfaces.Inventory;
using Common.Middlewares;
using Common.Networking.Configuration;
using Common.Networking.Packets;
using Common.Networking.Packets.Interfaces;
using Common.Services;
using Microsoft.AspNetCore.Diagnostics;
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

builder.Services.AddSingleton<IAsyncPacketHandler, PlayerMigrationPacketHandler>();
builder.Services.AddSingleton<IAsyncPacketHandler, UserChatPacketHandler>();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountRestrictionRepository, AccountRestrictionRepository>();
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryTabItemRepository, InventoryTabItemRepository>();

builder.Services.AddScoped<IAccountWorkUnit, AccountWorkUnit>();
builder.Services.AddScoped<IInventoryWorkUnit, InventoryWorkUnit>();

builder.Services.AddSingleton<IChannelServer, ChannelServer.Services.ChannelServer>();
builder.Services.AddSingleton<IPacketProcessor, PacketProcessor>();
builder.Services.AddScoped<IChatService<IGeneralMessage>, GeneralChatService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

builder.Services.AddHostedService<ChannelServerBackgroundService>();

var dbConStr = builder.Configuration.GetConnectionString("MapleStory");
builder.Services.AddDbContext<IEntityContext, EntityContext>(options =>
    options.UseMySql(dbConStr, ServerVersion.AutoDetect(dbConStr))
        .LogTo(Console.WriteLine, LogLevel.Warning)
        //.EnableSensitiveDataLogging()
        .EnableDetailedErrors());

builder.Services.AddOptions();
builder.Services.Configure<ServerConfig>(builder.Configuration.GetSection("Server"));
builder.Services.Configure<ChannelConfig>(builder.Configuration.GetSection("Channel"));

var app = builder.Build();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.Run();