using System.Net;
using Common.Database;
using Common.Networking;
using LoginServer.Handlers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

var dbConStr = builder.Configuration.GetConnectionString("MapleStory");

builder.Services.AddDbContext<EntityContext>(options =>
    options.UseMySql(dbConStr, ServerVersion.AutoDetect(dbConStr))
        .LogTo(Console.WriteLine, LogLevel.Warning)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors());

var app = builder.Build();
//Test.Read<ClientStartPacket>(new GameMessageBuffer(new byte [] { 05, 00, 0x68, 0x65, 0x6C, 0x6C, 0x6F, 04, 00, 0x68, 0x65, 0x6C, 0x6C }), out var packetInstance);
//Console.WriteLine($"Serialized Packet Instance: {JsonConvert.SerializeObject(packetInstance)}");

var _ = new GameServer(new IPEndPoint(IPAddress.Any, 8484));

app.Run();