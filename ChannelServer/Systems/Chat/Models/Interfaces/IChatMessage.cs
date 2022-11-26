namespace ChannelServer.Systems.Chat.Models.Interfaces;

public interface IChatMessage
{
    uint SenderCharacterId { get; init; }
    string Message { get; init; }
}