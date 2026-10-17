using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Security;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/results")]
[SessionAuthorize]
public class ResultsController(RaceDayDbContext db) : ControllerBase
{
    [HttpPost, SessionAuthorize(Roles.Organiser)]
    public async Task<IActionResult> Record(ResultRequest request)
    {
        var enrolment = await db.Enrolments.Include(e => e.Event).Include(e => e.Result).FirstOrDefaultAsync(e => e.Id == request.EnrolmentId);
        if (enrolment is null) return NotFound(new { message = "Enrolment not found." });
        if (enrolment.Event!.OrganiserId != HttpContext.Session.GetInt32("UserId")) return Forbid();
        if (request.FinishTime <= TimeSpan.Zero || request.FinishingPosition < 1)
            return BadRequest(new { message = "Finish time and position must be positive." });
        if (enrolment.Result is not null) return Conflict(new { message = "A result already exists for this enrolment." });
        var result = new RaceResult { EnrolmentId = enrolment.Id, FinishTime = request.FinishTime, FinishingPosition = request.FinishingPosition };
        db.Results.Add(result);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(MyResults), new { id = result.Id }, result);
    }

    [HttpGet("mine"), SessionAuthorize(Roles.Participant)]
    public async Task<IActionResult> MyResults()
    {
        var participantId = HttpContext.Session.GetInt32("UserId")!.Value;
        return Ok(await db.Results.Include(r => r.Enrolment!).ThenInclude(e => e.Event)
            .Include(r => r.Enrolment!).ThenInclude(e => e.Category)
            .Where(r => r.Enrolment!.ParticipantId == participantId)
            .Select(r => new { r.Id, Event = r.Enrolment!.Event!.Name, Date = r.Enrolment!.Event!.Date, Category = r.Enrolment!.Category!.Name, r.FinishTime, r.FinishingPosition })
            .ToListAsync());
    }
}
