using Common.Database.WorkUnits.Abstract;

namespace Common.Database.WorkUnits;

public class AccountWorkUnit : UnitOfWork
{
    public AccountWorkUnit(EntityContext entityContext) : base(entityContext)
    {
        UserAccounts = new UserAccountRepository(entityContext);
        UserAccountAuths = new UserAccountAuthRepository(entityContext);
    }
    
    public IUserAccountRepository UserAccounts { get; }
    public IUserAccountAuthRepository UserAccountAuths { get; }
}