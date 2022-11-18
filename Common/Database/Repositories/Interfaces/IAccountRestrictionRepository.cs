using Common.Database.Models.Interfaces;
using Common.Database.Repositories.Abstract.Interfaces;

namespace Common.Database.Repositories.Interfaces;

public interface IAccountRestrictionRepository<T> : IEntityRepository<T> where T : class, IAccountRestriction
{
}