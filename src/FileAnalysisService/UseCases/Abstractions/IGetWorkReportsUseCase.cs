namespace FileAnalysisService.UseCases.Abstractions;

public interface IGetWorkReportsUseCase
{
    Task<IReadOnlyList<object>> ExecuteAsync(string workId, CancellationToken ct);
}