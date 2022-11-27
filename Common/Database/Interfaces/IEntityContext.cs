using Common.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Common.Database.Interfaces;

public interface IEntityContext
{
    DbSet<Account> Accounts { get; set; }
    DbSet<AccountRestriction> AccountRestrictions { get; set; }
    DbSet<Character> Characters { get; set; }
    DbSet<Inventory> Inventories { get; set; }
    DbSet<EquippableItem> EquippableItems { get; set; }
    DbSet<ConsumableItem> ConsumableItems { get; set; }
    DbSet<SetupItem> SetupItems { get; set; }
    DbSet<EtceteraItem> EtceteraItems { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}