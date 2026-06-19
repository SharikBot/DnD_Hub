using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Core.Entities;
using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Core.Patterns.Singleton;

namespace DnDCharacterManager.Desktop.Services;

public sealed class ApiClient : IApiClient, IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _httpClient;
    private bool _disposed;

    public ApiClient(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
        BaseUrl = AppConfiguration.Instance.ApiBaseUrl.TrimEnd('/');
        _httpClient.BaseAddress = new Uri(BaseUrl + "/");
        _httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");
    }

    public string BaseUrl
    {
        get => _httpClient.BaseAddress?.ToString().TrimEnd('/') ?? string.Empty;
        set
        {
            var normalized = value.TrimEnd('/');
            _httpClient.BaseAddress = new Uri(normalized + "/");
        }
    }

    public async Task<IReadOnlyList<CharacterListItemDto>> GetCharactersAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await GetAsync<List<CharacterListItemDto>>($"api/characters/user/{userId}", cancellationToken);
        return result ?? [];
    }

    public Task<CharacterDto?> GetCharacterAsync(Guid id, CancellationToken cancellationToken = default) =>
        GetAsync<CharacterDto>($"api/characters/{id}", cancellationToken);

    public async Task<CharacterDto> CreateCharacterAsync(CreateCharacterDto dto, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/characters", dto, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CharacterDto>(JsonOptions, cancellationToken))!;
    }

    public async Task<CharacterDto?> UpdateCharacterAsync(Guid id, CreateCharacterDto dto, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/characters/{id}", dto, JsonOptions, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<CharacterDto>(JsonOptions, cancellationToken);
    }

    public async Task<CharacterDto?> UpdateCharacterSheetAsync(Guid id, UpdateCharacterSheetDto dto, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, $"api/characters/{id}/sheet")
        {
            Content = JsonContent.Create(dto, options: JsonOptions)
        };
        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<CharacterDto>(JsonOptions, cancellationToken);
    }

    public async Task<bool> DeleteCharacterAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"api/characters/{id}", cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<byte[]?> DownloadCharacterPdfAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/characters/{id}/pdf", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MonsterDto>> SearchMonstersAsync(string? search = null, CancellationToken cancellationToken = default)
    {
        var url = string.IsNullOrWhiteSpace(search)
            ? "api/monsters"
            : $"api/monsters/search?name={Uri.EscapeDataString(search)}";
        var result = await GetAsync<List<MonsterDto>>(url, cancellationToken);
        return result ?? [];
    }

    public Task<MonsterDto?> GetMonsterAsync(Guid id, CancellationToken cancellationToken = default) =>
        GetAsync<MonsterDto>($"api/monsters/{id}", cancellationToken);

    public async Task<IReadOnlyList<RuleDto>> SearchRulesAsync(string? search = null, CancellationToken cancellationToken = default)
    {
        var url = string.IsNullOrWhiteSpace(search)
            ? "api/rules"
            : $"api/rules/search?title={Uri.EscapeDataString(search)}";
        var result = await GetAsync<List<RuleDto>>(url, cancellationToken);
        return result ?? [];
    }

    public async Task<IReadOnlyList<RuleDto>> GetRulesByCategoryAsync(RuleCategory category, CancellationToken cancellationToken = default)
    {
        var result = await GetAsync<List<RuleDto>>($"api/rules/category/{category}", cancellationToken);
        return result ?? [];
    }

    public Task<RuleDto?> GetRuleAsync(Guid id, CancellationToken cancellationToken = default) =>
        GetAsync<RuleDto>($"api/rules/{id}", cancellationToken);

    public async Task<AiGenerationResponseDto> GenerateAiContentAsync(AiGenerationRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/ai/generate", request, JsonOptions, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return new AiGenerationResponseDto
            {
                IsSuccess = false,
                ErrorMessage = $"Ошибка API: {(int)response.StatusCode} {response.ReasonPhrase}"
            };
        }

        return (await response.Content.ReadFromJsonAsync<AiGenerationResponseDto>(JsonOptions, cancellationToken))
               ?? new AiGenerationResponseDto { IsSuccess = false, ErrorMessage = "Пустой ответ сервера." };
    }

    public async Task<IReadOnlyList<Race>> GetRacesAsync(CancellationToken cancellationToken = default)
    {
        var result = await GetAsync<List<Race>>("api/reference/races", cancellationToken);
        return result ?? [];
    }

    public async Task<IReadOnlyList<CharacterClass>> GetClassesAsync(CancellationToken cancellationToken = default)
    {
        var result = await GetAsync<List<CharacterClass>>("api/reference/classes", cancellationToken);
        return result ?? [];
    }

    public async Task<IReadOnlyList<Background>> GetBackgroundsAsync(CancellationToken cancellationToken = default)
    {
        var result = await GetAsync<List<Background>>("api/reference/backgrounds", cancellationToken);
        return result ?? [];
    }

    public async Task<IReadOnlyList<Trait>> GetTraitsAsync(CancellationToken cancellationToken = default)
    {
        var result = await GetAsync<List<Trait>>("api/reference/traits", cancellationToken);
        return result ?? [];
    }

    public async Task<IReadOnlyList<Spell>> GetSpellsAsync(CancellationToken cancellationToken = default)
    {
        var result = await GetAsync<List<Spell>>("api/reference/spells", cancellationToken);
        return result ?? [];
    }

    private async Task<T?> GetAsync<T>(string relativeUrl, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(relativeUrl, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _httpClient.Dispose();
        _disposed = true;
    }
}
