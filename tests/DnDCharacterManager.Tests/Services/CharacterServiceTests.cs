using DnDCharacterManager.Application.Services;
using DnDCharacterManager.Application.Validators;
using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Core.Patterns.Factory;
using DnDCharacterManager.Infrastructure.Data;
using DnDCharacterManager.Infrastructure.Repositories;
using DnDCharacterManager.Tests.TestHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

namespace DnDCharacterManager.Tests.Services;

public class CharacterServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CharacterService _service;
    private readonly TestSeedData _seed;

    public CharacterServiceTests()
    {
        _context = TestDataSeeder.CreateInMemoryContext();
        _seed = TestDataSeeder.SeedReferenceData(_context);

        var characterRepository = new CharacterRepository(_context);
        var monsterRepository = new MonsterRepository(_context);
        var ruleRepository = new RuleRepository(_context);
        var unitOfWork = new UnitOfWork(_context, characterRepository, monsterRepository, ruleRepository);
        var referenceRepository = new ReferenceRepository(_context);

        _service = new CharacterService(
            unitOfWork,
            referenceRepository,
            new CharacterFactory(),
            new MemoryCache(new MemoryCacheOptions()),
            new CreateCharacterValidator(),
            NullLogger<CharacterService>.Instance);
    }

    [Fact]
    public async Task CreateAsync_PersistsCharacter_AndReturnsDto()
    {
        var dto = CreateValidDto("Frodo");

        var result = await _service.CreateAsync(dto);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Frodo", result.Name);
        Assert.Equal("Human", result.RaceName);
        Assert.Equal("Fighter", result.ClassName);
        Assert.Equal("Soldier", result.BackgroundName);
        Assert.Equal(15, result.AbilityScores[AbilityType.Str]);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCharacter_AfterCreate()
    {
        var created = await _service.CreateAsync(CreateValidDto("Sam"));

        var loaded = await _service.GetByIdAsync(created.Id);

        Assert.NotNull(loaded);
        Assert.Equal(created.Id, loaded.Id);
        Assert.Equal("Sam", loaded.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var result = await _service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsUserCharacters()
    {
        await _service.CreateAsync(CreateValidDto("Merry"));
        await _service.CreateAsync(CreateValidDto("Pippin"));

        var list = await _service.GetByUserIdAsync(_seed.User.Id);

        Assert.Equal(2, list.Count);
        Assert.Contains(list, item => item.Name == "Merry");
        Assert.Contains(list, item => item.Name == "Pippin");
    }

    [Fact]
    public async Task UpdateAsync_ModifiesCharacter_WhenExists()
    {
        var created = await _service.CreateAsync(CreateValidDto("Gandalf"));
        var updateDto = CreateValidDto("Gandalf the Grey");
        updateDto.Alignment = AlignmentType.NeutralGood;

        var updated = await _service.UpdateAsync(created.Id, updateDto);

        Assert.NotNull(updated);
        Assert.Equal("Gandalf the Grey", updated.Name);
        Assert.Equal(AlignmentType.NeutralGood, updated.Alignment);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenCharacterNotFound()
    {
        var result = await _service.UpdateAsync(Guid.NewGuid(), CreateValidDto("Ghost"));

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_RemovesCharacter_AndReturnsTrue()
    {
        var created = await _service.CreateAsync(CreateValidDto("Sauron"));

        var deleted = await _service.DeleteAsync(created.Id);
        var loaded = await _service.GetByIdAsync(created.Id);

        Assert.True(deleted);
        Assert.Null(loaded);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenCharacterNotFound()
    {
        var deleted = await _service.DeleteAsync(Guid.NewGuid());

        Assert.False(deleted);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenValidationFails()
    {
        var invalid = CreateValidDto("");
        invalid.Name = "";

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(invalid));
    }

    public void Dispose() => _context.Dispose();

    private CreateCharacterDto CreateValidDto(string name) => new()
    {
        Name = name,
        UserId = _seed.User.Id,
        RaceId = _seed.Race.Id,
        CharacterClassId = _seed.CharacterClass.Id,
        BackgroundId = _seed.Background.Id,
        Alignment = AlignmentType.TrueNeutral,
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
}
