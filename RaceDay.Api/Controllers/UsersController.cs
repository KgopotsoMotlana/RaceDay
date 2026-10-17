using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Security;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/users")]
[SessionAuthorize]
public class UsersController(RaceDayDbContext db) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var id = HttpContext.Session.GetInt32("UserId");
        var user = await db.Users.FindAsync(id);
        return user is null ? NotFound() : Ok(ToDto(user));
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UpdateProfileRequest request)
    {
        var id = HttpContext.Session.GetInt32("UserId");
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) return NotFound();
        user.FullName = request.FullName.Trim();
        user.Phone = request.Phone;
        await db.SaveChangesAsync();
        return Ok(ToDto(user));
    }

    private static UserProfileDto ToDto(User u) => new(u.Id, u.FullName, u.Email, u.Role, u.Phone);
}
