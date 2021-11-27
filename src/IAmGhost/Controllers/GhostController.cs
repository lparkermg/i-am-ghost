using IAmGhost.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IAmGhost.Controllers;

[ApiController]
[Route("[controller]")]
public class GhostController : ControllerBase
{
    private readonly IGhostService _ghostService;
    public GhostController(IGhostService ghostService)
    {
        _ghostService = ghostService;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Guid id, [FromBody]string content)
    {
        try
        {
            var step = await _ghostService.AddStep(id, content);
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

    [HttpGet]
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

    [HttpGet("/{id}/{stepId}")]
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
