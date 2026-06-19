using DnDCharacterManager.Core.Entities;
using DnDCharacterManager.Core.Enums;

namespace DnDCharacterManager.Core.Interfaces.Repositories;

public interface IRuleRepository : IRepository<Rule>
{
    Task<IReadOnlyList<Rule>> GetByCategoryAsync(RuleCategory category, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Rule>> SearchByTitleAsync(string title, CancellationToken cancellationToken = default);
}
