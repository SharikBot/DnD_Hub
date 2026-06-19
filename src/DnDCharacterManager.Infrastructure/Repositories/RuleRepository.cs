using DnDCharacterManager.Core.Entities;
using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Core.Interfaces.Repositories;
using DnDCharacterManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DnDCharacterManager.Infrastructure.Repositories;

public class RuleRepository : Repository<Rule>, IRuleRepository
{
    public RuleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Rule>> GetByCategoryAsync(RuleCategory category, CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Where(r => r.Category == category)
            .OrderBy(r => r.Title)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Rule>> SearchByTitleAsync(string title, CancellationToken cancellationToken = default)
    {
        var term = title.Trim().ToLowerInvariant();
        return await DbSet
            .AsNoTracking()
            .Where(r => r.Title.ToLower().Contains(term))
            .OrderBy(r => r.Title)
            .ToListAsync(cancellationToken);
    }
}
