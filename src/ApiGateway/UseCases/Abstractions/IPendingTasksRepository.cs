using ApiGateway.Entities;

namespace ApiGateway.UseCases.Abstractions;

public interface IPendingTasksRepository
{
    Task AddAsync(PendingAnalysisTask task, CancellationToken ct);
    Task<PendingAnalysisTask?> TakeNextAsync(CancellationToken ct);
    Task MarkDoneAsync(Guid id, CancellationToken ct);
    Task IncrementAttemptsAsync(Guid id, CancellationToken ct);
}