using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Core.Entities;
using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Core.Patterns.Factory;
using DnDCharacterManager.Infrastructure.Data;
using DnDCharacterManager.Infrastructure.Repositories;
using DnDCharacterManager.Tests.TestHelpers;

namespace DnDCharacterManager.Tests.Repositories;

public class CharacterRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CharacterRepository _repository;
    private readonly TestSeedData _seed;

    public CharacterRepositoryTests()
    {
        _context = TestDataSeeder.CreateInMemoryContext();
        _repository = new CharacterRepository(_context);
        _seed = TestDataSeeder.SeedReferenceData(_context);
    }

    [Fact]
    public async Task AddAsync_PersistsCharacter()
    {
        var character = CreateCharacter("Gimli");

        await _repository.AddAsync(character);
        await _context.SaveChangesAsync();

        var stored = await _repository.GetByIdAsync(character.Id);
        Assert.NotNull(stored);
        Assert.Equal("Gimli", stored.Name);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsCharactersForUser()
    {
        await _repository.AddAsync(CreateCharacter("Older"));
        await _repository.AddAsync(CreateCharacter("Newer"));
        await _context.SaveChangesAsync();

        var result = await _repository.GetByUserIdAsync(_seed.User.Id);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Name == "Older");
        Assert.Contains(result, c => c.Name == "Newer");
    }

    [Fact]
    public async Task GetByIdWithDetailsAsync_LoadsNavigationProperties()
    {
        var character = CreateCharacter("Boromir");
        await _repository.AddAsync(character);
        await _context.SaveChangesAsync();

        var loaded = await _repository.GetByIdWithDetailsAsync(character.Id);

        Assert.NotNull(loaded);
        Assert.NotNull(loaded.Race);
        Assert.NotNull(loaded.CharacterClass);
        Assert.NotNull(loaded.Background);
        Assert.Equal("Human", loaded.Race.Name);
        Assert.Equal("Fighter", loaded.CharacterClass.Name);
        Assert.Equal("Soldier", loaded.Background.Name);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_ForUnknownId()
    {
        var exists = await _repository.ExistsAsync(Guid.NewGuid());

        Assert.False(exists);
    }

    [Fact]
    public async Task DeleteAsync_RemovesCharacter()
    {
        var character = CreateCharacter("ToDelete");
        await _repository.AddAsync(character);
        await _context.SaveChangesAsync();

        await _repository.DeleteAsync(character.Id);
        await _context.SaveChangesAsync();

        var exists = await _repository.ExistsAsync(character.Id);
        Assert.False(exists);
    }

    public void Dispose() => _context.Dispose();

    private Character CreateCharacter(string name)
    {
        var factory = new CharacterFactory();
        var character = factory.Create(
            new CreateCharacterDto
            {
                Name = name,
                UserId = _seed.User.Id,
                RaceId = _seed.Race.Id,
                CharacterClassId = _seed.CharacterClass.Id,
                BackgroundId = _seed.Background.Id
            },
            new Dictionary<AbilityType, int>
            {
                [AbilityType.Str] = 15,
                [AbilityType.Dex] = 14,
                [AbilityType.Con] = 13,
                [AbilityType.Int] = 12,
                [AbilityType.Wis] = 10,
                [AbilityType.Cha] = 8
            });

        return character;
    }
}
