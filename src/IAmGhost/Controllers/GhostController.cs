using IAmGhost.Interfaces;
using IAmGhost.Requests;
using Microsoft.AspNetCore.Mvc;

namespace IAmGhost.Controllers;

/// <summary>
/// The api controller for the <see cref="IGhostService"/>.
/// </summary>
[ApiController]
[Route("[controller]")]
public class GhostController : ControllerBase
{
    private readonly IGhostService _ghostService;

    /// <summary>
    /// Initialises the <see cref="GhostController"/> class.
    /// </summary>
    /// <param name="ghostService">The implementation of <see cref="IGhostService"/> to use.</param>
    public GhostController(IGhostService ghostService)
    {
        _ghostService = ghostService;
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Post(Guid id, [FromBody]AddRequest content)
    {
        try
        {
            var step = await _ghostService.AddStep(id, content.Snapshot);
            return Created($"ghost/{id}/{step}", null);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        try
        {
            var ghost = await _ghostService.Get(id);
            return ghost == null ? NotFound() : Ok(ghost);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/{stepId}")]
    public async Task<IActionResult> GetStep(Guid id, int stepId)
    {
        try
        {
            var step = await _ghostService.GetStep(id, stepId);
            return step == null ? NotFound() : Ok(step);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch
        {
            return NotFound();
        }
    }
}
