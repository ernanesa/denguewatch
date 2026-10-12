using api.DTOs.Cases;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CasesController(ICasesService cases) : ControllerBase
{
    private readonly ICasesService _cases = cases;

    [HttpPost]
    public async Task<ActionResult<CaseResponse>> CreateAsync(CreateCaseRequest request, CancellationToken cancellationToken)
    {
        var created = await _cases.CreateAsync(request, cancellationToken);

        return CreatedAtAction("GetById", new { id = created.Id }, created);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CaseResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var cases = await _cases.GetAllAsync(cancellationToken);
        return Ok(cases);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CaseResponse>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest("Invalid Case ID");

        var result = await _cases.GetByIdAsync(id, cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(string id, UpdateCaseRequest request, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest("Invalid Case ID");

        var updated = await _cases.UpdateAsync(id, request, cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest("Invalid Case ID");

        var deleted = await _cases.DeleteAsync(id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}
