using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models;

public record RegisterRequest(
    [Required, MaxLength(100)] string FullName,
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password,
    [Required] string Role,
    string? Phone);

public record LoginRequest([Required, EmailAddress] string Email, [Required] string Password);
public record UserProfileDto(int Id, string FullName, string Email, string Role, string? Phone);
public record UpdateProfileRequest([Required, MaxLength(100)] string FullName, string? Phone);
public record EventRequest(
    [Required, MaxLength(150)] string Name,
    [Required, MaxLength(1000)] string Description,
    DateTime Date,
    [Required, MaxLength(200)] string Location,
    [Range(0.1, 1000)] decimal DistanceKm,
    [Required] string EventType);
public record CategoryRequest([Required, MaxLength(80)] string Name, string? Description);
public record EnrolRequest([Range(1, int.MaxValue)] int EventId, [Range(1, int.MaxValue)] int CategoryId);
public record ResultRequest([Range(1, int.MaxValue)] int EnrolmentId, TimeSpan FinishTime, [Range(1, int.MaxValue)] int FinishingPosition);
