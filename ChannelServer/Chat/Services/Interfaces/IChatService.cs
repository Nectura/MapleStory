using ChannelServer.Chat.Models.Interfaces;

namespace ChannelServer.Chat.Services.Interfaces;

public interface IChatService<in T> where T : IChatMessage
{
    void HandleChatMessage(T chatMessage);
}