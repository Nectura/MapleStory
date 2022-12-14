using Common.Database.Models;
using Common.Database.Repositories.Abstract;
using Common.Database.Repositories.Interfaces;

namespace Common.Database.Repositories;

public sealed class CharacterRepository : EntityRepository<Character>, ICharacterRepository
{
    public CharacterRepository(EntityContext context) : base(context)
    {
    }
}