using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DnDCharacterManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RulesController : ControllerBase
{
    private readonly IRuleService _ruleService;

    public RulesController(IRuleService ruleService)
    {
        _ruleService = ruleService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RuleDto>>> GetAll(CancellationToken cancellationToken)
    {
        var rules = await _ruleService.GetAllAsync(cancellationToken);
        return Ok(rules);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RuleDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var rule = await _ruleService.GetByIdAsync(id, cancellationToken);
        return rule is null ? NotFound() : Ok(rule);
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<IReadOnlyList<RuleDto>>> GetByCategory(RuleCategory category, CancellationToken cancellationToken)
    {
        var rules = await _ruleService.GetByCategoryAsync(category, cancellationToken);
        return Ok(rules);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<RuleDto>>> Search([FromQuery] string title, CancellationToken cancellationToken)
    {
        var rules = await _ruleService.SearchAsync(title, cancellationToken);
        return Ok(rules);
    }

    [HttpPost]
    public async Task<ActionResult<RuleDto>> Create([FromBody] RuleDto dto, CancellationToken cancellationToken)
    {
        var created = await _ruleService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RuleDto>> Update(Guid id, [FromBody] RuleDto dto, CancellationToken cancellationToken)
    {
        var updated = await _ruleService.UpdateAsync(id, dto, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _ruleService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
