using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobSearchDashboard.Api.Data;

namespace JobSearchDashboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetStats()
    {
        var jobs = await _db.Jobs.ToListAsync();
        var contacts = await _db.Contacts.CountAsync();
        var outreach = await _db.Outreach.CountAsync();

        var byStatus = jobs.GroupBy(j => j.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToList();

        var overdue = jobs.Count(j =>
            !string.IsNullOrEmpty(j.NextActionDate) &&
            DateTime.TryParse(j.NextActionDate, out var d) && d.Date < DateTime.UtcNow.Date);

        return Ok(new
        {
            TotalJobs = jobs.Count,
            ByStatus = byStatus,
            Contacts = contacts,
            Outreach = outreach,
            OverdueFollowUps = overdue
        });
    }
}
