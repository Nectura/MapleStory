namespace ChannelServer.Chat.Models.Interfaces;

public interface IWhisperMessage : IChatMessage
{
    uint SenderChannelId { get; init; }
    uint ReceiverCharacterId { get; init; }
}