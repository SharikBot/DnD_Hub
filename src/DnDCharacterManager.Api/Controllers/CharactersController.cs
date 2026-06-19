using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DnDCharacterManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CharactersController : ControllerBase
{
    private readonly ICharacterService _characterService;
    private readonly ICharacterPdfService _characterPdfService;

    public CharactersController(ICharacterService characterService, ICharacterPdfService characterPdfService)
    {
        _characterService = characterService;
        _characterPdfService = characterPdfService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CharacterDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var character = await _characterService.GetByIdAsync(id, cancellationToken);
        return character is null ? NotFound() : Ok(character);
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<IReadOnlyList<CharacterListItemDto>>> GetByUser(Guid userId, CancellationToken cancellationToken)
    {
        var characters = await _characterService.GetByUserIdAsync(userId, cancellationToken);
        return Ok(characters);
    }

    [HttpPost]
    public async Task<ActionResult<CharacterDto>> Create([FromBody] CreateCharacterDto dto, CancellationToken cancellationToken)
    {
        var created = await _characterService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CharacterDto>> Update(Guid id, [FromBody] CreateCharacterDto dto, CancellationToken cancellationToken)
    {
        var updated = await _characterService.UpdateAsync(id, dto, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _characterService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("{id:guid}/pdf")]
    public async Task<IActionResult> ExportPdf(Guid id, CancellationToken cancellationToken)
    {
        var pdf = await _characterPdfService.GenerateCharacterSheetAsync(id, cancellationToken);
        return File(pdf, "application/pdf", $"character-{id}.pdf");
    }

    [HttpPatch("{id:guid}/sheet")]
    public async Task<ActionResult<CharacterDto>> UpdateSheet(Guid id, [FromBody] UpdateCharacterSheetDto dto, CancellationToken cancellationToken)
    {
        var updated = await _characterService.UpdateSheetAsync(id, dto, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }
}
