using Common.Database.Models;
using Common.Database.Repositories.Abstract;
using Common.Database.Repositories.Interfaces;

namespace Common.Database.Repositories;

public sealed class AccountRestrictionRepository : EntityRepository<AccountRestriction>, IAccountRestrictionRepository<AccountRestriction>
{
    public AccountRestrictionRepository(EntityContext context) : base(context)
    {
    }
}