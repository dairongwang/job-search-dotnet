using Microsoft.AspNetCore.Mvc;
using JobSearchDashboard.Api.Models;
using JobSearchDashboard.Api.Services;

namespace JobSearchDashboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly IJobSearchService _search;
    private readonly IConfigService _config;

    public SearchController(IJobSearchService search, IConfigService config)
    {
        _search = search;
        _config = config;
    }

    [HttpGet]
    public async Task<ActionResult<List<JobListing>>> Search([FromQuery] string? keyword = null, CancellationToken ct = default)
    {
        var config = await _config.LoadAsync();
        var results = await _search.SearchAsync(config, keyword, ct);
        return Ok(results);
    }

    [HttpPost("test-site")]
    public async Task<ActionResult<List<JobListing>>> TestSite([FromBody] SiteConfig site, CancellationToken ct = default)
    {
        var config = await _config.LoadAsync();
        var results = await _search.TestSiteAsync(site, config, ct);
        return Ok(results);
    }
}
