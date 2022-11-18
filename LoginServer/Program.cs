using System.Net;
using Common.Database;
using Common.Networking;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var dbConStr = builder.Configuration.GetConnectionString("MapleStory");

builder.Services.AddDbContext<EntityContext>(options =>
    options.UseMySql(dbConStr, ServerVersion.AutoDetect(dbConStr))
        .LogTo(Console.WriteLine, LogLevel.Warning)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors());

var app = builder.Build();

var _ = new GameServer(new IPEndPoint(IPAddress.Any, 8484));

app.Run();