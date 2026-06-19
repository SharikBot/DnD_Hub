using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Core.Entities;
using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Core.Interfaces.Repositories;
using DnDCharacterManager.Core.Interfaces.Services;

namespace DnDCharacterManager.Application.Services;

public class RuleService : IRuleService
{
    private readonly IUnitOfWork _unitOfWork;

    public RuleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<RuleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var rules = await _unitOfWork.Rules.GetAllAsync(cancellationToken);
        return rules.Select(MapToDto).ToList();
    }

    public async Task<RuleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var rule = await _unitOfWork.Rules.GetByIdAsync(id, cancellationToken);
        return rule is null ? null : MapToDto(rule);
    }

    public async Task<RuleDto> CreateAsync(RuleDto dto, CancellationToken cancellationToken = default)
    {
        var entity = MapToEntity(dto);
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Rules.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    public async Task<RuleDto?> UpdateAsync(Guid id, RuleDto dto, CancellationToken cancellationToken = default)
    {
        var rule = await _unitOfWork.Rules.GetByIdAsync(id, cancellationToken);
        if (rule is null)
        {
            return null;
        }

        rule.Title = dto.Title.Trim();
        rule.Content = dto.Content;
        rule.Category = dto.Category;
        rule.Source = dto.Source;
        rule.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Rules.UpdateAsync(rule, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(rule);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Rules.ExistsAsync(id, cancellationToken))
        {
            return false;
        }

        await _unitOfWork.Rules.DeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<RuleDto>> GetByCategoryAsync(RuleCategory category, CancellationToken cancellationToken = default)
    {
        var rules = await _unitOfWork.Rules.GetByCategoryAsync(category, cancellationToken);
        return rules.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<RuleDto>> SearchAsync(string title, CancellationToken cancellationToken = default)
    {
        var rules = await _unitOfWork.Rules.SearchByTitleAsync(title, cancellationToken);
        return rules.Select(MapToDto).ToList();
    }

    private static RuleDto MapToDto(Rule rule) => new()
    {
        Id = rule.Id,
        Title = rule.Title,
        Content = rule.Content,
        Category = rule.Category,
        Source = rule.Source
    };

    private static Rule MapToEntity(RuleDto dto) => new()
    {
        Title = dto.Title.Trim(),
        Content = dto.Content,
        Category = dto.Category,
        Source = dto.Source
    };
}
