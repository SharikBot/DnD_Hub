using DnDCharacterManager.Core.Entities;
using DnDCharacterManager.Core.Interfaces.Repositories;
using DnDCharacterManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DnDCharacterManager.Infrastructure.Repositories;

public class CharacterRepository : Repository<Character>, ICharacterRepository
{
    public CharacterRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Character>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Include(c => c.Race)
            .Include(c => c.CharacterClass)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync(cancellationToken);

    public async Task<Character?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(c => c.Race)
            .Include(c => c.CharacterClass)
            .Include(c => c.Background)
            .Include(c => c.Inventory!)
                .ThenInclude(i => i.Items)
            .Include(c => c.CharacterTraits).ThenInclude(ct => ct.Trait)
            .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill)
            .Include(c => c.CharacterSpells).ThenInclude(csp => csp.Spell)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
}
