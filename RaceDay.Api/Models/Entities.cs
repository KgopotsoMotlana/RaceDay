using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models;

public static class Roles
{
    public const string Organiser = "Organiser";
    public const string Participant = "Participant";
}

public class User
{
    public int Id { get; set; }
    [Required, MaxLength(100)] public string FullName { get; set; } = "";
    [Required, EmailAddress, MaxLength(200)] public string Email { get; set; } = "";
    [Required] public string PasswordHash { get; set; } = "";
    [Required, MaxLength(20)] public string Role { get; set; } = Roles.Participant;
    [MaxLength(30)] public string? Phone { get; set; }
}

public class RaceEvent
{
    public int Id { get; set; }
    [Required, MaxLength(150)] public string Name { get; set; } = "";
    [Required, MaxLength(1000)] public string Description { get; set; } = "";
    public DateTime Date { get; set; }
    [Required, MaxLength(200)] public string Location { get; set; } = "";
    [Range(0.1, 1000)] public decimal DistanceKm { get; set; }
    [Required, MaxLength(20)] public string EventType { get; set; } = "Run";
    public int OrganiserId { get; set; }
    public User? Organiser { get; set; }
    public List<EventCategory> Categories { get; set; } = [];
    public List<Enrolment> Enrolments { get; set; } = [];
}

public class EventCategory
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public RaceEvent? Event { get; set; }
    [Required, MaxLength(80)] public string Name { get; set; } = "";
    [MaxLength(200)] public string? Description { get; set; }
}

public class Enrolment
{
    public int Id { get; set; }
    public int ParticipantId { get; set; }
    public User? Participant { get; set; }
    public int EventId { get; set; }
    public RaceEvent? Event { get; set; }
    public int CategoryId { get; set; }
    public EventCategory? Category { get; set; }
    [Required, MaxLength(20)] public string Status { get; set; } = "Confirmed";
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public RaceResult? Result { get; set; }
}

public class RaceResult
{
    public int Id { get; set; }
    public int EnrolmentId { get; set; }
    public Enrolment? Enrolment { get; set; }
    public TimeSpan FinishTime { get; set; }
    public int FinishingPosition { get; set; }
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}
