using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Security;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController(RaceDayDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await db.Events.Include(e => e.Categories).OrderBy(e => e.Date)
            .Select(e => new { e.Id, e.Name, e.Description, e.Date, e.Location, e.DistanceKm, e.EventType, e.OrganiserId, Categories = e.Categories.Select(c => new { c.Id, c.Name, c.Description }) })
            .ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var item = await db.Events.Include(e => e.Categories).Where(e => e.Id == id)
            .Select(e => new { e.Id, e.Name, e.Description, e.Date, e.Location, e.DistanceKm, e.EventType, e.OrganiserId, Categories = e.Categories.Select(c => new { c.Id, c.Name, c.Description }) })
            .SingleOrDefaultAsync();
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost, SessionAuthorize(Roles.Organiser)]
    public async Task<IActionResult> Create(EventRequest request)
    {
        if (!ValidType(request.EventType)) return BadRequest(new { message = "EventType must be Run, Walk, or Cycle." });
        var item = new RaceEvent { Name = request.Name.Trim(), Description = request.Description.Trim(), Date = request.Date, Location = request.Location.Trim(), DistanceKm = request.DistanceKm, EventType = request.EventType, OrganiserId = HttpContext.Session.GetInt32("UserId")!.Value };
        db.Events.Add(item);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}"), SessionAuthorize(Roles.Organiser)]
    public async Task<IActionResult> Update(int id, EventRequest request)
    {
        var item = await db.Events.FindAsync(id);
        if (item is null) return NotFound();
        if (item.OrganiserId != HttpContext.Session.GetInt32("UserId")) return Forbid();
        if (!ValidType(request.EventType)) return BadRequest(new { message = "EventType must be Run, Walk, or Cycle." });
        item.Name = request.Name.Trim(); item.Description = request.Description.Trim(); item.Date = request.Date;
        item.Location = request.Location.Trim(); item.DistanceKm = request.DistanceKm; item.EventType = request.EventType;
        await db.SaveChangesAsync();
        return Ok(item);
    }

    [HttpDelete("{id:int}"), SessionAuthorize(Roles.Organiser)]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await db.Events.FindAsync(id);
        if (item is null) return NotFound();
        if (item.OrganiserId != HttpContext.Session.GetInt32("UserId")) return Forbid();
        db.Events.Remove(item);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static bool ValidType(string type) => new[] { "Run", "Walk", "Cycle" }.Contains(type, StringComparer.OrdinalIgnoreCase);
}
