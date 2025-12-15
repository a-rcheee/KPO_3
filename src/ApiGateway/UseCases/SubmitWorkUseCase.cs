using ApiGateway.Entities;
using ApiGateway.UseCases.Abstractions;
using Contracts;

namespace ApiGateway.UseCases;

public sealed class SubmitWorkUseCase : ISubmitWorkUseCase
{
    private readonly IFileStorageApi _storage;
    private readonly IFileAnalysisApi _analysis;
    private readonly IPendingTasksRepository _pending;
    public SubmitWorkUseCase(IFileStorageApi storage, IFileAnalysisApi analysis, IPendingTasksRepository pending)
    {
        _storage = storage;
        _analysis = analysis;
        _pending = pending;
    }

    public async Task<SubmitWorkResult> ExecuteAsync(string workId, string studentName, Stream file, string fileName,
        CancellationToken ct)
    {
        var stored = await _storage.StoreAsync(file, fileName, ct);

        var req = new AnalyzeWorkRequest(workId, studentName, stored.FileId, stored.Sha256, stored.OriginalFileName);

        try
        {
            var analyzed = await _analysis.AnalyzeAsync(req, ct);
            return new SubmitWorkResult("Analyzed", stored, analyzed, null);
        }
        catch
        {
            await _pending.AddAsync(new PendingAnalysisTask
            {
                Id = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow,
                WorkId = req.WorkId,
                StudentName = req.StudentName,
                FileId = req.FileId,
                Sha256 = req.Sha256,
                OriginalFileName = req.OriginalFileName,
                Attempts = 0
            }, ct);

            return new SubmitWorkResult("PendingAnalysis", stored, null,
                "Analysis service unavailable");
        }
    }
}