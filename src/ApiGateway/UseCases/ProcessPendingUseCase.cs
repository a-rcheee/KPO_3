using ApiGateway.UseCases.Abstractions;
using Contracts;

namespace ApiGateway.UseCases;

public sealed class ProcessPendingUseCase : IProcessPendingUseCase
{
    private readonly IPendingTasksRepository _pending;
    private readonly IFileAnalysisApi _analysis;

    public ProcessPendingUseCase(IPendingTasksRepository pending, IFileAnalysisApi analysis)
    {
        _pending = pending;
        _analysis = analysis;
    }

    public async Task<int> ExecuteAsync(CancellationToken ct)
    {
        var processed = 0;
        while (true)
        {
            var task = await _pending.TakeNextAsync(ct);
            if (task is null) break;
            var req = new AnalyzeWorkRequest(task.WorkId, task.StudentName, task.FileId, task.Sha256,
                task.OriginalFileName);

            try
            {
                await _analysis.AnalyzeAsync(req, ct);
                await _pending.MarkDoneAsync(task.Id, ct);
                processed++;
            }
            catch
            {
                await _pending.IncrementAttemptsAsync(task.Id, ct);
                break;
            }
        }
        return processed;
    }
}