using Common.Networking;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var server = new TcpServer(8484);
server.StartListeningForConnections();
server.StartAcceptingConnections();

app.MapGet("/", () => "Hello World!");

app.Run();