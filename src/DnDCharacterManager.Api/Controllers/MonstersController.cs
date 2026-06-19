using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DnDCharacterManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonstersController : ControllerBase
{
    private readonly IMonsterService _monsterService;

    public MonstersController(IMonsterService monsterService)
    {
        _monsterService = monsterService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MonsterDto>>> GetAll(CancellationToken cancellationToken)
    {
        var monsters = await _monsterService.GetAllAsync(cancellationToken);
        return Ok(monsters);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MonsterDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var monster = await _monsterService.GetByIdAsync(id, cancellationToken);
        return monster is null ? NotFound() : Ok(monster);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<MonsterDto>>> Search([FromQuery] string name, CancellationToken cancellationToken)
    {
        var monsters = await _monsterService.SearchAsync(name, cancellationToken);
        return Ok(monsters);
    }

    [HttpGet("type/{creatureType}")]
    public async Task<ActionResult<IReadOnlyList<MonsterDto>>> GetByType(CreatureType creatureType, CancellationToken cancellationToken)
    {
        var monsters = await _monsterService.GetByCreatureTypeAsync(creatureType, cancellationToken);
        return Ok(monsters);
    }

    [HttpPost]
    public async Task<ActionResult<MonsterDto>> Create([FromBody] MonsterDto dto, CancellationToken cancellationToken)
    {
        var created = await _monsterService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<MonsterDto>> Update(Guid id, [FromBody] MonsterDto dto, CancellationToken cancellationToken)
    {
        var updated = await _monsterService.UpdateAsync(id, dto, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _monsterService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
