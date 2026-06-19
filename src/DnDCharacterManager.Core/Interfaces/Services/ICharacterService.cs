using DnDCharacterManager.Core.DTOs;

namespace DnDCharacterManager.Core.Interfaces.Services;

public interface ICharacterService
{
    Task<CharacterDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CharacterListItemDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<CharacterDto> CreateAsync(CreateCharacterDto dto, CancellationToken cancellationToken = default);

    Task<CharacterDto?> UpdateAsync(Guid id, CreateCharacterDto dto, CancellationToken cancellationToken = default);

    Task<CharacterDto?> UpdateSheetAsync(Guid id, UpdateCharacterSheetDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
