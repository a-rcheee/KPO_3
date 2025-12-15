using FileAnalysisService.Entities;
using FileAnalysisService.Infrastructure.Persistence;
using FileAnalysisService.UseCases.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FileAnalysisService.Infrastructure.Repositories;

public sealed class EfSubmissionsRepository : ISubmissionsRepository
{
    private readonly AppDbContext _db;

    public EfSubmissionsRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Submission submission, CancellationToken ct)
    {
        _db.Submissions.Add(submission);
        await _db.SaveChangesAsync(ct);
    }
    public Task<Submission?> FindEarlierDifferentStudentAsync(string workId, string sha256, string studentName, CancellationToken ct)
    {
        return _db.Submissions
            .Where(s => s.WorkId == workId && s.Sha256 == sha256 && s.StudentName != studentName)
            .OrderBy(s => s.SubmittedAtUtc)
            .FirstOrDefaultAsync(ct);
    }
}