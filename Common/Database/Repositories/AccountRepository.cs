using Common.Database.Models;
using Common.Database.Repositories.Abstract;
using Common.Database.Repositories.Interfaces;

namespace Common.Database.Repositories;

public sealed class AccountRepository : EntityRepository<Account>, IAccountRepository<Account>
{
    public AccountRepository(EntityContext context) : base(context)
    {
    }
}