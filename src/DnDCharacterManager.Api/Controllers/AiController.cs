using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DnDCharacterManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly IGeminiAiService _geminiAiService;

    public AiController(IGeminiAiService geminiAiService)
    {
        _geminiAiService = geminiAiService;
    }

    [HttpPost("generate")]
    public async Task<ActionResult<AiGenerationResponseDto>> Generate([FromBody] AiGenerationRequestDto request, CancellationToken cancellationToken)
    {
        var response = await _geminiAiService.GenerateAsync(request, cancellationToken);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpPost("generate/backstory")]
    public async Task<ActionResult<AiGenerationResponseDto>> GenerateBackstory([FromBody] AiGenerationRequestDto request, CancellationToken cancellationToken)
    {
        request.ContentType = "backstory";
        var response = await _geminiAiService.GenerateAsync(request, cancellationToken);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpPost("generate/npc")]
    public async Task<ActionResult<AiGenerationResponseDto>> GenerateNpc([FromBody] AiGenerationRequestDto request, CancellationToken cancellationToken)
    {
        request.ContentType = "npc";
        var response = await _geminiAiService.GenerateAsync(request, cancellationToken);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpPost("generate/encounter")]
    public async Task<ActionResult<AiGenerationResponseDto>> GenerateEncounter([FromBody] AiGenerationRequestDto request, CancellationToken cancellationToken)
    {
        request.ContentType = "encounter";
        var response = await _geminiAiService.GenerateAsync(request, cancellationToken);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }
}
