using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobSearchDashboard.Api.Data;
using JobSearchDashboard.Api.Models;

namespace JobSearchDashboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OutreachController : ControllerBase
{
    private readonly AppDbContext _db;
    public OutreachController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Outreach>>> GetAll() =>
        await _db.Outreach.OrderByDescending(o => o.ActivityDate).ToListAsync();

    [HttpPost]
    public async Task<ActionResult<Outreach>> Create(Outreach item)
    {
        item.CreatedAt = DateTime.UtcNow;
        _db.Outreach.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = item.Id }, item);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var o = await _db.Outreach.FindAsync(id);
        if (o is null) return NotFound();
        _db.Outreach.Remove(o);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
