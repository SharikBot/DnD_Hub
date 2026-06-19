using DnDCharacterManager.Core.Entities;
using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Core.Interfaces.Repositories;
using DnDCharacterManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DnDCharacterManager.Infrastructure.Repositories;

public class MonsterRepository : Repository<Monster>, IMonsterRepository
{
    public MonsterRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Monster>> SearchByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var term = name.Trim().ToLowerInvariant();
        return await DbSet
            .AsNoTracking()
            .Where(m => m.Name.ToLower().Contains(term))
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Monster>> GetByCreatureTypeAsync(CreatureType creatureType, CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Where(m => m.CreatureType == creatureType)
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);
}
