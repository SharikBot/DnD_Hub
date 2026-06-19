using DnDCharacterManager.Core.Entities;
using DnDCharacterManager.Core.Enums;

namespace DnDCharacterManager.Core.Interfaces.Repositories;

public interface IMonsterRepository : IRepository<Monster>
{
    Task<IReadOnlyList<Monster>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Monster>> GetByCreatureTypeAsync(CreatureType creatureType, CancellationToken cancellationToken = default);
}
