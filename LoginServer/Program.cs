using Common.Database;
using Common.Networking;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var dbConStr = builder.Configuration.GetConnectionString("MapleStory");

builder.Services.AddDbContext<EntityContext>(options =>
    options.UseMySql(dbConStr, ServerVersion.AutoDetect(dbConStr))
        .LogTo(Console.WriteLine, LogLevel.Warning)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors());

var _ = new TcpServer(8484);

app.MapGet("/", () => "Hello World!");

app.Run();