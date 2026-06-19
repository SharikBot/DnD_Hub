using DnDCharacterManager.Core.DTOs;

namespace DnDCharacterManager.Application.Validators;

public class CreateCharacterValidator
{
    public IReadOnlyList<string> Validate(CreateCharacterDto dto)
    {
        var errors = new List<string>();

        if (dto is null)
        {
            errors.Add("DTO is required.");
            return errors;
        }

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            errors.Add("Character name is required.");
        }
        else if (dto.Name.Length > 100)
        {
            errors.Add("Character name must not exceed 100 characters.");
        }

        if (dto.UserId == Guid.Empty)
        {
            errors.Add("UserId is required.");
        }

        if (dto.RaceId == Guid.Empty)
        {
            errors.Add("RaceId is required.");
        }

        if (dto.CharacterClassId == Guid.Empty)
        {
            errors.Add("CharacterClassId is required.");
        }

        if (dto.BackgroundId == Guid.Empty)
        {
            errors.Add("BackgroundId is required.");
        }

        return errors;
    }
}