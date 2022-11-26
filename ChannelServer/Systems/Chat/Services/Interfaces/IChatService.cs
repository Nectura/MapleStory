using ChannelServer.Systems.Chat.Models.Interfaces;

namespace ChannelServer.Systems.Chat.Services.Interfaces;

public interface IChatService<in T> where T : IChatMessage
{
    void HandleChatMessage(T chatMessage);
}