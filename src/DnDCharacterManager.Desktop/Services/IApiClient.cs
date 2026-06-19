using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Core.Entities;
using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Core.Patterns.Singleton;

namespace DnDCharacterManager.Desktop.Services;

public interface IApiClient
{
    string BaseUrl { get; set; }

    Task<IReadOnlyList<CharacterListItemDto>> GetCharactersAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<CharacterDto?> GetCharacterAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CharacterDto> CreateCharacterAsync(CreateCharacterDto dto, CancellationToken cancellationToken = default);
    Task<CharacterDto?> UpdateCharacterAsync(Guid id, CreateCharacterDto dto, CancellationToken cancellationToken = default);
    Task<CharacterDto?> UpdateCharacterSheetAsync(Guid id, UpdateCharacterSheetDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteCharacterAsync(Guid id, CancellationToken cancellationToken = default);
    Task<byte[]?> DownloadCharacterPdfAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MonsterDto>> SearchMonstersAsync(string? search = null, CancellationToken cancellationToken = default);
    Task<MonsterDto?> GetMonsterAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RuleDto>> SearchRulesAsync(string? search = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RuleDto>> GetRulesByCategoryAsync(RuleCategory category, CancellationToken cancellationToken = default);
    Task<RuleDto?> GetRuleAsync(Guid id, CancellationToken cancellationToken = default);

    Task<AiGenerationResponseDto> GenerateAiContentAsync(AiGenerationRequestDto request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Race>> GetRacesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CharacterClass>> GetClassesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Background>> GetBackgroundsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Trait>> GetTraitsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Spell>> GetSpellsAsync(CancellationToken cancellationToken = default);
}
