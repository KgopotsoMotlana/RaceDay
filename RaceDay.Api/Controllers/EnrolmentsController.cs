using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Security;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/enrolments")]
[SessionAuthorize]
public class EnrolmentsController(RaceDayDbContext db) : ControllerBase
{
    [HttpPost, SessionAuthorize(Roles.Participant)]
    public async Task<IActionResult> Enrol(EnrolRequest request)
    {
        var participantId = HttpContext.Session.GetInt32("UserId")!.Value;
        var evt = await db.Events.FindAsync(request.EventId);
        if (evt is null) return NotFound(new { message = "Event not found." });
        var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.EventId == request.EventId);
        if (category is null) return BadRequest(new { message = "Choose a category belonging to this event." });
        if (await db.Enrolments.AnyAsync(e => e.ParticipantId == participantId && e.EventId == request.EventId))
            return Conflict(new { message = "You have already enrolled for this event." });
        var enrolment = new Enrolment { ParticipantId = participantId, EventId = request.EventId, CategoryId = request.CategoryId };
        db.Enrolments.Add(enrolment);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(MyEnrolments), new { id = enrolment.Id }, new { enrolment.Id, enrolment.EventId, enrolment.CategoryId, enrolment.Status, enrolment.EnrolledAt });
    }

    [HttpGet("mine"), SessionAuthorize(Roles.Participant)]
    public async Task<IActionResult> MyEnrolments()
    {
        var id = HttpContext.Session.GetInt32("UserId")!.Value;
        return Ok(await db.Enrolments.Where(e => e.ParticipantId == id)
            .Include(e => e.Event).Include(e => e.Category)
            .Select(e => new { e.Id, e.Status, e.EnrolledAt, Event = e.Event!.Name, EventDate = e.Event!.Date, Category = e.Category!.Name })
            .ToListAsync());
    }

    [HttpGet("event/{eventId:int}"), SessionAuthorize(Roles.Organiser)]
    public async Task<IActionResult> ForEvent(int eventId)
    {
        var userId = HttpContext.Session.GetInt32("UserId")!.Value;
        if (!await db.Events.AnyAsync(e => e.Id == eventId && e.OrganiserId == userId)) return NotFound();
        return Ok(await db.Enrolments.Where(e => e.EventId == eventId).Include(e => e.Participant).Include(e => e.Category)
            .Select(e => new { e.Id, Participant = e.Participant!.FullName, Email = e.Participant!.Email, Category = e.Category!.Name, e.Status, e.EnrolledAt })
            .ToListAsync());
    }
}
