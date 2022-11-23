﻿using ChannelServer.Chat.Models.Interfaces;

namespace ChannelServer.Chat.Models;

public record struct GeneralChatMessage : IGeneralMessage
{
    public uint SenderCharacterId { get; init; }
    public string Message { get; init; }
    public uint MapId { get; init; }
}