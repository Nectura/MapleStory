using Common.Database.Models;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618

namespace Common.Database;

public sealed class EntityContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<AccountRestriction> AccountRestrictions { get; set; }
    public DbSet<Character> Characters { get; set; }

    public EntityContext(DbContextOptions<EntityContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>().HasOne(m => m.Restriction).WithOne(m => m.Account)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Account>().HasMany(m => m.Characters).WithOne(m => m.Account)
            .OnDelete(DeleteBehavior.Cascade);
    }
}