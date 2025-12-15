using Contracts;
using FileAnalysisService.Entities;
using FileAnalysisService.UseCases.Abstractions;

namespace FileAnalysisService.UseCases;

public class AnalyzeWorkUseCase : IAnalyzeWorkUseCase
{
    private readonly ISubmissionsRepository _submissions;
    private readonly IReportsRepository _reports;
    private readonly IReportFileWriter _writer;

    public AnalyzeWorkUseCase(ISubmissionsRepository submissions, IReportsRepository reports, IReportFileWriter writer)
    {
        _submissions = submissions;
        _reports = reports;
        _writer = writer;
    }
    
    public async Task<AnalyzeWorkResponse> ExecuteAsync(AnalyzeWorkRequest request, CancellationToken ct)
    {
        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            WorkId = request.WorkId,
            StudentName = request.StudentName,
            FileId = request.FileId,
            Sha256 = request.Sha256,
            OriginalFileName = request.OriginalFileName,
            SubmittedAtUtc = DateTime.UtcNow
        };

        await _submissions.AddAsync(submission, ct);
        
        var earlier = await _submissions.FindEarlierDifferentStudentAsync(
            request.WorkId, request.Sha256, request.StudentName, ct);

        var isPlagiarism = earlier is not null;

        var report = new Report
        {
            Id = Guid.NewGuid(),
            SubmissionId = submission.Id,
            CreatedAtUtc = DateTime.UtcNow,
            IsPlagiarism = isPlagiarism,
            MatchId = earlier?.Id,
            Reason = isPlagiarism
                ? $"Одинаковый хеш файла с более ранней сдачей ({earlier!.Id}) by {earlier.StudentName}"
                : "Больше ранних и совпадающих сдач не обнаружено"
        };
        
        await _reports.AddAsync(report, ct);
        await _writer.SaveReportJsonAsync(report, submission, ct);
        var response = new AnalyzeWorkResponse(submission.Id, report.Id, report.IsPlagiarism, report.MatchId, report.Reason);
        return response;
    }
}