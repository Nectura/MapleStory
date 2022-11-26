namespace ChannelServer.Systems.Chat.Models.Interfaces;

public interface IPartyMessage : IChatMessage
{
    uint PartyId { get; init; }
}