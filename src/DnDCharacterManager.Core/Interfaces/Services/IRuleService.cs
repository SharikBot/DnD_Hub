using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Core.Enums;

namespace DnDCharacterManager.Core.Interfaces.Services;

public interface IRuleService
{
    Task<IReadOnlyList<RuleDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<RuleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<RuleDto> CreateAsync(RuleDto dto, CancellationToken cancellationToken = default);

    Task<RuleDto?> UpdateAsync(Guid id, RuleDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RuleDto>> GetByCategoryAsync(RuleCategory category, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RuleDto>> SearchAsync(string title, CancellationToken cancellationToken = default);
}
