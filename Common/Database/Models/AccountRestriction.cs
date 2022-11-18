using System.ComponentModel.DataAnnotations.Schema;
using Common.Database.Models.Interfaces;

namespace Common.Database.Models;

public class AccountRestriction : IAccountRestriction
{
    public Guid Id { get; set; }

    public int AccountId { get; set; }
    public virtual Account? Account { get; set; }

    public int? IssuedByAccountId { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpirationTime { get; set; }

    [NotMapped] 
    public bool HasExpired => DateTime.UtcNow >= ExpirationTime;
}