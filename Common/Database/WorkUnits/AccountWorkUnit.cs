using Common.Database.Interfaces;
using Common.Database.Repositories;
using Common.Database.Repositories.Interfaces;
using Common.Database.WorkUnits.Abstract;
using Common.Database.WorkUnits.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Common.Database.WorkUnits;

public sealed class AccountWorkUnit : UnitOfWork, IAccountWorkUnit
{
    public AccountWorkUnit(IEntityContext entityContext) : base((DbContext)entityContext)
    {
        Accounts = new AccountRepository(entityContext);
        AccountRestrictions = new AccountRestrictionRepository(entityContext);
    }
    
    public IAccountRepository Accounts { get; }
    public IAccountRestrictionRepository AccountRestrictions { get; }
}