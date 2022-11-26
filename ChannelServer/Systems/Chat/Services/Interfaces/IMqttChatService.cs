using ChannelServer.Systems.Chat.Enum;
using ChannelServer.Systems.Chat.Models.Interfaces;

namespace ChannelServer.Systems.Chat.Services.Interfaces;

public interface IMqttChatService<in T> : IChatService<T> where T : IChatMessage
{
    Task ConnectAsync(CancellationToken cancellationToken = default, params EMqttChatTopic[] topics);
    Task DisconnectAsync(CancellationToken cancellationToken = default);
}