using Microsoft.EntityFrameworkCore;
using JobSearchDashboard.Api.Models;

namespace JobSearchDashboard.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Outreach> Outreach => Set<Outreach>();
    public DbSet<ResearchNote> ResearchNotes => Set<ResearchNote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organization>()
            .HasIndex(o => o.Name)
            .IsUnique();

        modelBuilder.Entity<Job>()
            .Property(j => j.Status).HasDefaultValue("Saved");
        modelBuilder.Entity<Job>()
            .Property(j => j.Priority).HasDefaultValue("Medium");
    }
}
