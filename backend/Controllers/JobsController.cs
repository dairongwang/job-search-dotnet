using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobSearchDashboard.Api.Data;
using JobSearchDashboard.Api.Models;

namespace JobSearchDashboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly AppDbContext _db;

    public JobsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Job>>> GetAll([FromQuery] string? status = null)
    {
        var q = _db.Jobs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(status))
            q = q.Where(j => j.Status == status);
        return await q.OrderByDescending(j => j.UpdatedAt).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Job>> Get(int id)
    {
        var job = await _db.Jobs.FindAsync(id);
        return job is null ? NotFound() : job;
    }

    [HttpPost]
    public async Task<ActionResult<Job>> Create(Job job)
    {
        job.CreatedAt = DateTime.UtcNow;
        job.UpdatedAt = DateTime.UtcNow;
        _db.Jobs.Add(job);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = job.Id }, job);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Job job)
    {
        if (id != job.Id) return BadRequest();
        job.UpdatedAt = DateTime.UtcNow;
        _db.Entry(job).State = EntityState.Modified;
        try { await _db.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException) { if (!await _db.Jobs.AnyAsync(j => j.Id == id)) return NotFound(); throw; }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var job = await _db.Jobs.FindAsync(id);
        if (job is null) return NotFound();
        _db.Jobs.Remove(job);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("from-listing")]
    public async Task<ActionResult<Job>> SaveFromListing([FromBody] JobListing listing)
    {
        var job = new Job
        {
            Title = listing.Title,
            Organization = listing.Organization,
            Location = listing.Location,
            Workplace = listing.Workplace,
            Url = listing.Url,
            Source = listing.Source,
            Description = listing.Description,
            Salary = listing.SalaryText,
            JobType = listing.JobType,
            Schedule = listing.Schedule,
            ExternalId = listing.ExternalId,
            Status = "Saved",
            DateFound = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _db.Jobs.Add(job);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = job.Id }, job);
    }
}
