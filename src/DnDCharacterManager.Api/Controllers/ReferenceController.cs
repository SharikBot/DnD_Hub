using DnDCharacterManager.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DnDCharacterManager.Api.Controllers;

[ApiController]
[Route("api/reference")]
public class ReferenceController : ControllerBase
{
    private readonly IReferenceService _referenceService;

    public ReferenceController(IReferenceService referenceService)
    {
        _referenceService = referenceService;
    }

    [HttpGet("races")]
    public async Task<IActionResult> GetRaces(CancellationToken cancellationToken) =>
        Ok(await _referenceService.GetRacesAsync(cancellationToken));

    [HttpGet("classes")]
    public async Task<IActionResult> GetClasses(CancellationToken cancellationToken) =>
        Ok(await _referenceService.GetClassesAsync(cancellationToken));

    [HttpGet("backgrounds")]
    public async Task<IActionResult> GetBackgrounds(CancellationToken cancellationToken) =>
        Ok(await _referenceService.GetBackgroundsAsync(cancellationToken));

    [HttpGet("traits")]
    public async Task<IActionResult> GetTraits(CancellationToken cancellationToken) =>
        Ok(await _referenceService.GetTraitsAsync(cancellationToken));

    [HttpGet("spells")]
    public async Task<IActionResult> GetSpells(CancellationToken cancellationToken) =>
        Ok(await _referenceService.GetSpellsAsync(cancellationToken));
}
