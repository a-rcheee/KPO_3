using FileAnalysisService.Entities;

namespace FileAnalysisService.UseCases.Abstractions;

public interface IReportFileWriter
{
    Task SaveReportJsonAsync(Report report, Submission submission, CancellationToken ct);
}