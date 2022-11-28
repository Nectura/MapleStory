using Common.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Common.Database.Interfaces;

public interface IEntityContext
{
    DbSet<Account> Accounts { get; set; }
    DbSet<AccountRestriction> AccountRestrictions { get; set; }
    DbSet<Character> Characters { get; set; }
    DbSet<Inventory> Inventories { get; set; }
    DbSet<InventoryTabItem> InventoryTabItems { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}