namespace Common.Database.Models.Interfaces;

public interface IAccountRestriction
{
    Guid Id { get; set; }
    int AccountId { get; set; }
    int? IssuedByAccountId { get; set; }
    string? Reason { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime ExpirationTime { get; set; }
    bool HasExpired { get; }
}