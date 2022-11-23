using ChannelServer.Chat.Models.Interfaces;
using ChannelServer.Chat.Services.Interfaces;
using ChannelServer.Services.Interfaces;
using Common.Networking;
using Common.Networking.Abstract;
using Common.Networking.Packets.Enums;

namespace ChannelServer.Chat.Services;

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