using System.Net;
using System.Net.Http.Json;
using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Infrastructure.Data;

namespace DnDCharacterManager.Tests.Integration;

public class ApiControllersIntegrationTests : IClassFixture<DndWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiControllersIntegrationTests(DndWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Reference_GetRaces_ReturnsSeededData()
    {
        var response = await _client.GetAsync("/api/reference/races");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("Человек", json, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Reference_GetClasses_ReturnsSeededData()
    {
        var response = await _client.GetAsync("/api/reference/classes");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("Воин", json, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Monsters_GetAll_ReturnsSeededData()
    {
        var monsters = await _client.GetFromJsonAsync<List<MonsterDto>>("/api/monsters");

        Assert.NotNull(monsters);
        Assert.NotEmpty(monsters);
        Assert.Contains(monsters, m => m.Name.Contains("Гоблин", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Rules_Search_ReturnsResults()
    {
        var rules = await _client.GetFromJsonAsync<List<RuleDto>>("/api/rules/search?title=Advantage");

        Assert.NotNull(rules);
        Assert.NotEmpty(rules);
    }

    [Fact]
    public async Task Characters_CreateGetDelete_FullCrudCycle()
    {
        var createDto = new CreateCharacterDto
        {
            UserId = SeedData.DemoUserId,
            Name = "Integration Hero",
            RaceId = SeedData.RaceHuman,
            CharacterClassId = SeedData.ClassFighter,
            BackgroundId = SeedData.BgSoldier,
            Alignment = AlignmentType.LawfulGood,
            AbilityScoreMethod = AbilityScoreMethod.StandardArray,
            AbilityScores = new Dictionary<AbilityType, int>
            {
                [AbilityType.Str] = 15,
                [AbilityType.Dex] = 14,
                [AbilityType.Con] = 13,
                [AbilityType.Int] = 12,
                [AbilityType.Wis] = 10,
                [AbilityType.Cha] = 8
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/characters", createDto);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<CharacterDto>();
        Assert.NotNull(created);
        Assert.Equal("Integration Hero", created.Name);

        var getResponse = await _client.GetAsync($"/api/characters/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var byUser = await _client.GetFromJsonAsync<List<CharacterListItemDto>>(
            $"/api/characters/user/{SeedData.DemoUserId}");
        Assert.NotNull(byUser);
        Assert.Contains(byUser, c => c.Id == created.Id);

        var deleteResponse = await _client.DeleteAsync($"/api/characters/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var notFound = await _client.GetAsync($"/api/characters/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, notFound.StatusCode);
    }

    [Fact]
    public async Task Characters_ExportPdf_ReturnsPdfBytes()
    {
        var createDto = new CreateCharacterDto
        {
            UserId = SeedData.DemoUserId,
            Name = "Pdf Hero",
            RaceId = SeedData.RaceElf,
            CharacterClassId = SeedData.ClassWizard,
            BackgroundId = SeedData.BgSage,
            Alignment = AlignmentType.NeutralGood,
            AbilityScoreMethod = AbilityScoreMethod.StandardArray,
            AbilityScores = new Dictionary<AbilityType, int>
            {
                [AbilityType.Str] = 8,
                [AbilityType.Dex] = 14,
                [AbilityType.Con] = 13,
                [AbilityType.Int] = 15,
                [AbilityType.Wis] = 12,
                [AbilityType.Cha] = 10
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/characters", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<CharacterDto>();
        Assert.NotNull(created);

        var pdfResponse = await _client.GetAsync($"/api/characters/{created.Id}/pdf");
        Assert.Equal(HttpStatusCode.OK, pdfResponse.StatusCode);
        Assert.Equal("application/pdf", pdfResponse.Content.Headers.ContentType?.MediaType);

        var bytes = await pdfResponse.Content.ReadAsByteArrayAsync();
        Assert.True(bytes.Length > 100);
        Assert.Equal((byte)'%', bytes[0]);
    }
}
