using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;

namespace RaceDay.Api.Data;

public class RaceDayDbContext(DbContextOptions<RaceDayDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RaceEvent> Events => Set<RaceEvent>();
    public DbSet<EventCategory> Categories => Set<EventCategory>();
    public DbSet<Enrolment> Enrolments => Set<Enrolment>();
    public DbSet<RaceResult> Results => Set<RaceResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<User>().Property(x => x.Role).HasMaxLength(20).IsRequired();
        modelBuilder.Entity<RaceEvent>().Property(x => x.EventType).HasMaxLength(20).IsRequired();
        modelBuilder.Entity<RaceEvent>().HasOne(x => x.Organiser).WithMany()
            .HasForeignKey(x => x.OrganiserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EventCategory>().HasOne(x => x.Event).WithMany(x => x.Categories)
            .HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Enrolment>().HasIndex(x => new { x.ParticipantId, x.EventId }).IsUnique();
        modelBuilder.Entity<Enrolment>().HasOne(x => x.Participant).WithMany()
            .HasForeignKey(x => x.ParticipantId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Enrolment>().HasOne(x => x.Event).WithMany(x => x.Enrolments)
            .HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Enrolment>().HasOne(x => x.Category).WithMany()
            .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<RaceResult>().HasOne(x => x.Enrolment).WithOne(x => x.Result)
            .HasForeignKey<RaceResult>(x => x.EnrolmentId).OnDelete(DeleteBehavior.Cascade);
    }
}
