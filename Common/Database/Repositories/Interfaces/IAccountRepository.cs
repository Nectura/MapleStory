using Common.Database.Models.Interfaces;
using Common.Database.Repositories.Abstract.Interfaces;

namespace Common.Database.Repositories.Interfaces;

public interface IAccountRepository<T> : IEntityRepository<T> where T : class, IAccount
{
}