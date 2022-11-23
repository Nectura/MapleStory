using ChannelServer.Chat.Enum;
using ChannelServer.Chat.Models.Interfaces;

namespace ChannelServer.Chat.Services.Interfaces;

public interface IMqttChatService<in T> : IChatService<T> where T : IChatMessage
{
    Task ConnectAsync(CancellationToken cancellationToken = default, params EMqttChatTopic[] topics);
    Task DisconnectAsync(CancellationToken cancellationToken = default);
}