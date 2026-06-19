using DnDCharacterManager.Core.DTOs;

namespace DnDCharacterManager.Core.Interfaces.Services;

public interface IGeminiAiService
{
    Task<AiGenerationResponseDto> GenerateAsync(AiGenerationRequestDto request, CancellationToken cancellationToken = default);
}
