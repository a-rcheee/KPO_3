using FileAnalysisService.Entities;

namespace FileAnalysisService.UseCases.Abstractions;

public interface ISubmissionsRepository
{
    Task AddAsync(Submission submission, CancellationToken ct);

    Task<Submission?> FindEarlierDifferentStudentAsync(string workId, string sha256, string studentName, CancellationToken ct);
}