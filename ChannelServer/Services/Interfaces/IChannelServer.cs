using Common.Networking;
using Common.Networking.Abstract.Interfaces;

namespace ChannelServer.Services.Interfaces;

public interface IChannelServer : IGameServer
{
    Dictionary<uint, GameClient> ConnectedPeers { get; init; }
}