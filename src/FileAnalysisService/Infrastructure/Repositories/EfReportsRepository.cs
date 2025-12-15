using FileAnalysisService.Entities;
using FileAnalysisService.Infrastructure.Persistence;
using FileAnalysisService.UseCases.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FileAnalysisService.Infrastructure.Repositories;

public class EfReportsRepository : IReportsRepository
{
    private readonly AppDbContext _db;

    public EfReportsRepository(AppDbContext db)
    {
        _db = db;
    }
    public async Task AddAsync(Report report, CancellationToken ct)
    {
        _db.Reports.Add(report);
        await _db.SaveChangesAsync(ct);
    }
    
    public async Task<IReadOnlyList<(Report report, Submission submission)>> GetReportsByWorkIdAsync(string workId, CancellationToken ct)
    {
        var query =
            from r in _db.Reports
            join s in _db.Submissions on r.SubmissionId equals s.Id
            where s.WorkId == workId
            orderby r.CreatedAtUtc descending
            select new { r, s };

        var list = await query.ToListAsync(ct);
        return list.Select(x => (x.r, x.s)).ToList();
    }
}