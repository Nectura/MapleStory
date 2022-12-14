using Common.Database.Models;
using Common.Database.Repositories.Abstract.Interfaces;

namespace Common.Database.Repositories.Interfaces;

public interface IAccountRepository : IEntityRepository<Account>
{
}