using Common.Database.Repositories;
using Common.Database.Repositories.Interfaces;
using Common.Database.WorkUnits.Abstract;
using Common.Database.WorkUnits.Interfaces;

namespace Common.Database.WorkUnits;

public sealed class AccountWorkUnit : UnitOfWork, IAccountWorkUnit
{
    public AccountWorkUnit(EntityContext entityContext) : base(entityContext)
    {
        Accounts = new AccountRepository(entityContext);
        AccountRestrictions = new AccountRestrictionRepository(entityContext);
    }
    
    public IAccountRepository Accounts { get; }
    public IAccountRestrictionRepository AccountRestrictions { get; }
}