namespace DnDCharacterManager.Core.DTOs;

public class AiGenerationRequestDto
{
    public string ContentType { get; set; } = string.Empty;

    public string Prompt { get; set; } = string.Empty;

    public Guid? CharacterId { get; set; }

    public int MaxTokens { get; set; } = 1024;
}
