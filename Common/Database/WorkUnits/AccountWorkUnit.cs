using Common.Database.Models;
using Common.Database.Repositories;
using Common.Database.Repositories.Interfaces;
using Common.Database.WorkUnits.Abstract;

namespace Common.Database.WorkUnits;

/* public class AccountWorkUnit : UnitOfWork
{
    public AccountWorkUnit(EntityContext entityContext) : base(entityContext)
    {
        UserAccounts = new AccountRepository<Account>(entityContext);
        AccountRestrictions = new AccountRestrictionRepository<AccountRestriction>(entityContext);
    }
    
    public IAccountRepository UserAccounts { get; }
    public IAccountRestrictionRepository AccountRestrictions { get; }
} */