using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(RaceDayDbContext db) : ControllerBase
{
    private readonly PasswordHasher<User> _hasher = new();

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (request.Role is not (Roles.Organiser or Roles.Participant))
            return BadRequest(new { message = "Role must be Organiser or Participant." });
        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(u => u.Email == email))
            return Conflict(new { message = "An account with this email already exists." });

        var user = new User { FullName = request.FullName.Trim(), Email = email, Role = request.Role, Phone = request.Phone };
        user.PasswordHash = _hasher.HashPassword(user, request.Password);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return Created($"/api/users/me", new UserProfileDto(user.Id, user.FullName, user.Email, user.Role, user.Phone));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user is null || _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            return Unauthorized(new { message = "Email or password is incorrect." });

        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("Role", user.Role);
        HttpContext.Session.SetString("FullName", user.FullName);
        return Ok(new { message = "Login successful.", user = new UserProfileDto(user.Id, user.FullName, user.Email, user.Role, user.Phone) });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Ok(new { message = "You have been logged out." });
    }
}
