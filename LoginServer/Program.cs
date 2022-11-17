using Common.Networking;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var _ = new TcpServer(8484);

app.MapGet("/", () => "Hello World!");

app.Run();