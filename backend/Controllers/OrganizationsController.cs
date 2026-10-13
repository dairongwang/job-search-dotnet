using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobSearchDashboard.Api.Data;
using JobSearchDashboard.Api.Models;

namespace JobSearchDashboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationsController : ControllerBase
{
    private readonly AppDbContext _db;
    public OrganizationsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Organization>>> GetAll() =>
        await _db.Organizations.OrderBy(o => o.Name).ToListAsync();

    [HttpPost]
    public async Task<ActionResult<Organization>> Create(Organization org)
    {
        org.CreatedAt = DateTime.UtcNow;
        _db.Organizations.Add(org);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = org.Id }, org);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Organization org)
    {
        if (id != org.Id) return BadRequest();
        _db.Entry(org).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var o = await _db.Organizations.FindAsync(id);
        if (o is null) return NotFound();
        _db.Organizations.Remove(o);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
