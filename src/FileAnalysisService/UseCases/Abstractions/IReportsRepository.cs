using FileAnalysisService.Entities;

namespace FileAnalysisService.UseCases.Abstractions;

public interface IReportsRepository
{
    Task AddAsync(Report report, CancellationToken ct);

    Task<IReadOnlyList<(Report report, Submission submission)>> GetReportsByWorkIdAsync(string workId, CancellationToken ct);
}