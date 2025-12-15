using System.Text.Json;
using FileAnalysisService.Entities;
using FileAnalysisService.UseCases.Abstractions;

namespace FileAnalysisService.Infrastructure.Reports;

public sealed class LocalReportFileWriter : IReportFileWriter
{
    private readonly string _reportsDir;

    public LocalReportFileWriter(string reportsDir)
    {
        _reportsDir = reportsDir;
        Directory.CreateDirectory(_reportsDir);
    } 
    
    public async Task SaveReportJsonAsync(Report report, Submission submission, CancellationToken ct)
    {
        var path = Path.Combine(_reportsDir, $"{report.Id}.json");

        var json = JsonSerializer.Serialize(new
        {
            reportId = report.Id,
            createdAtUtc = report.CreatedAtUtc,
            isPlagiarism = report.IsPlagiarism,
            matchId = report.MatchId,
            reason = report.Reason,
            submission
        }, new JsonSerializerOptions { WriteIndented = true });

        await File.WriteAllTextAsync(path, json, ct);
    }
}