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
using Common.Services.Interfaces;
using LoginServer.Configuration;
using LoginServer.Packets.Handlers;
using LoginServer.Services.Background;
using LoginServer.Services.Interfaces;
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

builder.Services.AddSingleton<IAsyncPacketHandler, ExceptionLogPacketHandler>();
builder.Services.AddSingleton<IAsyncPacketHandler, ClientValidationPacketHandler>();
builder.Services.AddSingleton<IAsyncPacketHandler, ClientStartPacketHandler>();
builder.Services.AddSingleton<IAsyncPacketHandler, ClientLoginPacketHandler>();
builder.Services.AddSingleton<IAsyncPacketHandler, CheckUserLimitPacketHandler>();
builder.Services.AddSingleton<IAsyncPacketHandler, WorldInfoRequestPacketHandler>();
builder.Services.AddSingleton<IAsyncPacketHandler, SelectWorldPacketHandler>();
builder.Services.AddSingleton<IAsyncPacketHandler, CharacterNameCheckHandler>();
builder.Services.AddSingleton<IAsyncPacketHandler, CharacterCreationHandler>();
builder.Services.AddSingleton<IAsyncPacketHandler, CharacterDeletionHandler>();
builder.Services.AddSingleton<IAsyncPacketHandler, SelectCharacterPacketHandler>();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountRestrictionRepository, AccountRestrictionRepository>();
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryTabItemRepository, InventoryTabItemRepository>();

builder.Services.AddScoped<IAccountWorkUnit, AccountWorkUnit>();
builder.Services.AddScoped<IInventoryWorkUnit, InventoryWorkUnit>();

builder.Services.AddSingleton<ILoginServer, LoginServer.Services.LoginServer>();
builder.Services.AddSingleton<IPacketProcessor, PacketProcessor>();
builder.Services.AddSingleton<IAuthService, Sha3AuthService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

builder.Services.AddHostedService<LoginServerBackgroundService>();

var dbConStr = builder.Configuration.GetConnectionString("MapleStory");
builder.Services.AddDbContext<IEntityContext, EntityContext>(options =>
    options.UseMySql(dbConStr, ServerVersion.AutoDetect(dbConStr))
        .LogTo(Console.WriteLine, LogLevel.Warning)
        //.EnableSensitiveDataLogging()
        .EnableDetailedErrors());

builder.Services.AddOptions();
builder.Services.Configure<ServerConfig>(builder.Configuration.GetSection("Server"));
builder.Services.Configure<LoginConfig>(builder.Configuration.GetSection("Login"));
builder.Services.Configure<InventoryConfig>(builder.Configuration.GetSection("Inventory"));

var app = builder.Build();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.Run();