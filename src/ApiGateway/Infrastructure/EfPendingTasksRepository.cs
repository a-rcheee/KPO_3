using ApiGateway.Entities;
using ApiGateway.UseCases.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ApiGateway.Infrastructure;

public sealed class EfPendingTasksRepository : IPendingTasksRepository
{
    private readonly GatewayDbContext _db;
    public EfPendingTasksRepository(GatewayDbContext db) => _db = db;

    public async Task AddAsync(PendingAnalysisTask task, CancellationToken ct)
    {
        _db.PendingTasks.Add(task);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<PendingAnalysisTask?> TakeNextAsync(CancellationToken ct)
    {
        return await _db.PendingTasks
            .OrderBy(t => t.CreatedAtUtc)
            .FirstOrDefaultAsync(ct);
    }
    
    public async Task MarkDoneAsync(Guid id, CancellationToken ct)
    {
        var entity = await _db.PendingTasks.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return;

        _db.PendingTasks.Remove(entity);
        await _db.SaveChangesAsync(ct);
    }
    
    public async Task IncrementAttemptsAsync(Guid id, CancellationToken ct)
    {
        var entity = await _db.PendingTasks.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return;

        entity.Attempts++;
        await _db.SaveChangesAsync(ct);
    }
}