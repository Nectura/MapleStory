using Common.Database.Repositories.Interfaces;

namespace Common.Database.WorkUnits.Interfaces;

public interface IAccountWorkUnit : IUnitOfWork
{
    IAccountRepository Accounts { get; }
    IAccountRestrictionRepository AccountRestrictions { get; }
}