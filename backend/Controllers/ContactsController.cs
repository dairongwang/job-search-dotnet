using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobSearchDashboard.Api.Data;
using JobSearchDashboard.Api.Models;

namespace JobSearchDashboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ContactsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Contact>>> GetAll() =>
        await _db.Contacts.OrderBy(c => c.Name).ToListAsync();

    [HttpPost]
    public async Task<ActionResult<Contact>> Create(Contact contact)
    {
        contact.CreatedAt = DateTime.UtcNow;
        _db.Contacts.Add(contact);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = contact.Id }, contact);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Contact contact)
    {
        if (id != contact.Id) return BadRequest();
        _db.Entry(contact).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await _db.Contacts.FindAsync(id);
        if (c is null) return NotFound();
        _db.Contacts.Remove(c);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
