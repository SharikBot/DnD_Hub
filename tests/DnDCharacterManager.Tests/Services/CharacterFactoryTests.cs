using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Core.Patterns.Factory;

namespace DnDCharacterManager.Tests.Services;

public class CharacterFactoryTests
{
    private readonly CharacterFactory _factory = new();

    [Fact]
    public void Create_ReturnsValidCharacter_WithExpectedDefaults()
    {
        var userId = Guid.NewGuid();
        var dto = new CreateCharacterDto
        {
            Name = "  Aragorn  ",
            UserId = userId,
            RaceId = Guid.NewGuid(),
            CharacterClassId = Guid.NewGuid(),
            BackgroundId = Guid.NewGuid(),
            Alignment = AlignmentType.LawfulGood,
            Backstory = "  Ranger of the North  "
        };

        var abilityScores = new Dictionary<AbilityType, int>
        {
            [AbilityType.Str] = 16,
            [AbilityType.Dex] = 14,
            [AbilityType.Con] = 15,
            [AbilityType.Int] = 10,
            [AbilityType.Wis] = 12,
            [AbilityType.Cha] = 8
        };

        var character = _factory.Create(dto, abilityScores);

        Assert.NotEqual(Guid.Empty, character.Id);
        Assert.Equal("Aragorn", character.Name);
        Assert.Equal(userId, character.UserId);
        Assert.Equal(1, character.Level);
        Assert.Equal(AlignmentType.LawfulGood, character.Alignment);
        Assert.Equal("Ranger of the North", character.Backstory);
        Assert.Equal(16, character.Strength);
        Assert.Equal(14, character.Dexterity);
        Assert.Equal(15, character.Constitution);
        Assert.Equal(10, character.Intelligence);
        Assert.Equal(12, character.Wisdom);
        Assert.Equal(8, character.Charisma);
    }

    [Fact]
    public void Create_CalculatesHitPointsAndArmorClass_FromAbilityScores()
    {
        var dto = CreateMinimalDto();
        var abilityScores = new Dictionary<AbilityType, int>
        {
            [AbilityType.Str] = 10,
            [AbilityType.Dex] = 14,
            [AbilityType.Con] = 14,
            [AbilityType.Int] = 10,
            [AbilityType.Wis] = 10,
            [AbilityType.Cha] = 10
        };

        var character = _factory.Create(dto, abilityScores);

        Assert.Equal(10, character.MaxHitPoints);
        Assert.Equal(10, character.CurrentHitPoints);
        Assert.Equal(12, character.ArmorClass);
    }

    [Fact]
    public void Create_CreatesInventory_ForCharacter()
    {
        var character = _factory.Create(CreateMinimalDto(), DefaultAbilityScores());

        Assert.NotNull(character.Inventory);
        Assert.Equal(character.Id, character.Inventory.CharacterId);
        Assert.NotEqual(Guid.Empty, character.Inventory.Id);
    }

    [Fact]
    public void Create_Throws_WhenDtoIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _factory.Create(null!, DefaultAbilityScores()));
    }

    [Fact]
    public void Create_Throws_WhenAbilityScoresIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _factory.Create(CreateMinimalDto(), null!));
    }

    private static CreateCharacterDto CreateMinimalDto() => new()
    {
        Name = "Hero",
        UserId = Guid.NewGuid(),
        RaceId = Guid.NewGuid(),
        CharacterClassId = Guid.NewGuid(),
        BackgroundId = Guid.NewGuid()
    };

    private static Dictionary<AbilityType, int> DefaultAbilityScores() => new()
    {
        [AbilityType.Str] = 15,
        [AbilityType.Dex] = 14,
        [AbilityType.Con] = 13,
        [AbilityType.Int] = 12,
        [AbilityType.Wis] = 10,
        [AbilityType.Cha] = 8
    };
}
