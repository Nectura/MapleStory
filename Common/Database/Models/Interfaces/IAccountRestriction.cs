namespace Common.Database.Models.Interfaces;

public interface IAccountRestriction
{
    Guid Id { get; set; }
    uint AccountId { get; set; }
    uint? IssuedByAccountId { get; set; }
    string? Reason { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime ExpirationTime { get; set; }
    bool HasExpired { get; }
}