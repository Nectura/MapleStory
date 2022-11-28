using Common.Database.Interfaces;
using Common.Database.Models;
using Common.Database.Repositories.Abstract;
using Common.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Common.Database.Repositories;

public sealed class CharacterRepository : EntityRepository<Character>, ICharacterRepository
{
    public CharacterRepository(IEntityContext context) : base((DbContext)context)
    {
    }
}