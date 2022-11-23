using Common.Networking;
using Common.Networking.Abstract.Interfaces;

namespace LoginServer.Services.Interfaces;

public interface ILoginServer : IGameServer
{
    HashSet<GameClient> ConnectedPeers { get; init; }
}