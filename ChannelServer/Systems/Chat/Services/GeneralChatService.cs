using ChannelServer.Services.Interfaces;
using ChannelServer.Systems.Chat.Models.Interfaces;
using ChannelServer.Systems.Chat.Services.Interfaces;
using Common.Networking;
using Common.Networking.Packets.Enums;

namespace ChannelServer.Systems.Chat.Services;

public sealed class GeneralChatService : IChatService<IGeneralMessage>
{
    private readonly IChannelServer _channelServer;

    public GeneralChatService(IChannelServer channelServer)
    {
        _channelServer = channelServer;
    }

    public void HandleChatMessage(IGeneralMessage chatMessage)
    {
        BroadcastToMapPlayers(chatMessage);
    }

    private void BroadcastToMapPlayers(IGeneralMessage chatMessage)
    {
        var mapPeers = _channelServer.ConnectedPeers.Values
            .Where(m => m.Character.MapId == chatMessage.MapId)
            .ToList();
        foreach (var peer in mapPeers)
            peer.Send(new GameMessageBuffer(EServerOperationCode.UserChat)
                .WriteUInt(peer.Character.Id)
                .WriteBool() // client.Account.AccountType == EAccountType.GameMaster [needs to match the SendLoginSuccessResult packet]
                .WriteString(chatMessage.Message)
                .WriteBool()
            );
    }
}