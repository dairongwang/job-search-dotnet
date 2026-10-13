using Microsoft.AspNetCore.Mvc;
using JobSearchDashboard.Api.Models;
using JobSearchDashboard.Api.Services;

namespace JobSearchDashboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigController : ControllerBase
{
    private readonly IConfigService _config;

    public ConfigController(IConfigService config) => _config = config;

    [HttpGet]
    public async Task<ActionResult<AppConfig>> Get() => await _config.LoadAsync();

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] AppConfig config)
    {
        await _config.SaveAsync(config);
        return NoContent();
    }
}
