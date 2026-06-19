using DnDCharacterManager.Application.Validators;
using DnDCharacterManager.Core.DTOs;

namespace DnDCharacterManager.Tests.Validators;

public class CreateCharacterValidatorTests
{
    private readonly CreateCharacterValidator _validator = new();

    [Fact]
    public void Validate_ReturnsNoErrors_ForValidDto()
    {
        var dto = CreateValidDto();

        var errors = _validator.Validate(dto);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenDtoIsNull()
    {
        var errors = _validator.Validate(null!);

        Assert.Contains("DTO is required.", errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenNameIsEmpty()
    {
        var dto = CreateValidDto();
        dto.Name = "   ";

        var errors = _validator.Validate(dto);

        Assert.Contains("Character name is required.", errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenNameExceedsMaxLength()
    {
        var dto = CreateValidDto();
        dto.Name = new string('A', 101);

        var errors = _validator.Validate(dto);

        Assert.Contains("Character name must not exceed 100 characters.", errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenUserIdIsEmpty()
    {
        var dto = CreateValidDto();
        dto.UserId = Guid.Empty;

        var errors = _validator.Validate(dto);

        Assert.Contains("UserId is required.", errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenRaceIdIsEmpty()
    {
        var dto = CreateValidDto();
        dto.RaceId = Guid.Empty;

        var errors = _validator.Validate(dto);

        Assert.Contains("RaceId is required.", errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenCharacterClassIdIsEmpty()
    {
        var dto = CreateValidDto();
        dto.CharacterClassId = Guid.Empty;

        var errors = _validator.Validate(dto);

        Assert.Contains("CharacterClassId is required.", errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenBackgroundIdIsEmpty()
    {
        var dto = CreateValidDto();
        dto.BackgroundId = Guid.Empty;

        var errors = _validator.Validate(dto);

        Assert.Contains("BackgroundId is required.", errors);
    }

    [Fact]
    public void Validate_ReturnsMultipleErrors_WhenSeveralFieldsInvalid()
    {
        var dto = new CreateCharacterDto
        {
            Name = "",
            UserId = Guid.Empty,
            RaceId = Guid.Empty,
            CharacterClassId = Guid.Empty,
            BackgroundId = Guid.Empty
        };

        var errors = _validator.Validate(dto);

        Assert.True(errors.Count >= 5);
    }

    private static CreateCharacterDto CreateValidDto() => new()
    {
        Name = "Legolas",
        UserId = Guid.NewGuid(),
        RaceId = Guid.NewGuid(),
        CharacterClassId = Guid.NewGuid(),
        BackgroundId = Guid.NewGuid()
    };
}
