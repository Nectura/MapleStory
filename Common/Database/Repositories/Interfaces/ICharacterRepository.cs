using Common.Database.Models.Interfaces;
using Common.Database.Repositories.Abstract.Interfaces;

namespace Common.Database.Repositories.Interfaces;

public interface ICharacterRepository<T> : IEntityRepository<T> where T : class, ICharacter
{
}