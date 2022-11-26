using ChannelServer.Systems.Chat.Models.Interfaces;

namespace ChannelServer.Systems.Chat.Models;

public record struct GeneralChatMessage : IGeneralMessage
{
    public uint SenderCharacterId { get; init; }
    public string Message { get; init; }
    public uint MapId { get; init; }
}