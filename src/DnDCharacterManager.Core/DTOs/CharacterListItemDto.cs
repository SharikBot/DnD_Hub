namespace DnDCharacterManager.Core.DTOs;

public class CharacterListItemDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Level { get; set; }

    public string RaceName { get; set; } = string.Empty;

    public string ClassName { get; set; } = string.Empty;

    public DateTime UpdatedAt { get; set; }
}
