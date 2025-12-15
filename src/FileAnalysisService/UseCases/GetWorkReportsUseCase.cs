using FileAnalysisService.UseCases.Abstractions;

namespace FileAnalysisService.UseCases;

public sealed class GetWorkReportsUseCase : IGetWorkReportsUseCase
{
    private readonly IReportsRepository _repo;
    public GetWorkReportsUseCase(IReportsRepository repo) => _repo = repo;
    
    public async Task<IReadOnlyList<object>> ExecuteAsync(string workId, CancellationToken ct)
    {
        var items = await _repo.GetReportsByWorkIdAsync(workId, ct);
        return items.Select(x => (object)new {x.report.Id, x.report.CreatedAtUtc, x.report.IsPlagiarism,
            x.report.MatchId, x.report.Reason, submission = x.submission }).ToList();
    }
}