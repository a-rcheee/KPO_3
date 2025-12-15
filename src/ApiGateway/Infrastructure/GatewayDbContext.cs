using ApiGateway.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiGateway.Infrastructure;

public sealed class GatewayDbContext : DbContext
{
    public DbSet<PendingAnalysisTask> PendingTasks => Set<PendingAnalysisTask>();

    public GatewayDbContext(DbContextOptions<GatewayDbContext> options) : base(options) { }
}