using FileAnalysisService.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileAnalysisService.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<Report> Reports => Set<Report>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Submission>().HasKey(x => x.Id);
        modelBuilder.Entity<Report>().HasKey(x => x.Id);

        modelBuilder.Entity<Submission>().HasIndex(x => new { x.WorkId, x.Sha256 });
    }
}