using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Security;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/events/{eventId:int}/categories")]
public class CategoriesController(RaceDayDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetForEvent(int eventId)
    {
        if (!await db.Events.AnyAsync(e => e.Id == eventId)) return NotFound(new { message = "Event not found." });
        return Ok(await db.Categories.Where(c => c.EventId == eventId).ToListAsync());
    }

    [HttpPost, SessionAuthorize(Roles.Organiser)]
    public async Task<IActionResult> Create(int eventId, CategoryRequest request)
    {
        var evt = await db.Events.FindAsync(eventId);
        if (evt is null) return NotFound(new { message = "Event not found." });
        if (evt.OrganiserId != HttpContext.Session.GetInt32("UserId")) return Forbid();
        var category = new EventCategory { EventId = eventId, Name = request.Name.Trim(), Description = request.Description };
        db.Categories.Add(category);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetForEvent), new { eventId }, category);
    }

    [HttpDelete("{categoryId:int}"), SessionAuthorize(Roles.Organiser)]
    public async Task<IActionResult> Delete(int eventId, int categoryId)
    {
        var evt = await db.Events.FindAsync(eventId);
        if (evt is null) return NotFound();
        if (evt.OrganiserId != HttpContext.Session.GetInt32("UserId")) return Forbid();
        var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == categoryId && c.EventId == eventId);
        if (category is null) return NotFound();
        db.Categories.Remove(category);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
