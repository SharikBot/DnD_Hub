namespace DnDCharacterManager.Core.DTOs;

public class AiGenerationResponseDto
{
    public string GeneratedText { get; set; } = string.Empty;

    public bool IsSuccess { get; set; }

    public string? ErrorMessage { get; set; }

    public int TokensUsed { get; set; }
}
