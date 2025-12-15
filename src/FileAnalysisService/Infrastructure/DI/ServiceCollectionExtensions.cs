using FileAnalysisService.Infrastructure.Persistence;
using FileAnalysisService.Infrastructure.Reports;
using FileAnalysisService.Infrastructure.Repositories;
using FileAnalysisService.UseCases.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FileAnalysisService.Infrastructure.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string dataDir)
    {
        Directory.CreateDirectory(dataDir);

        var dbPath = Path.Combine(dataDir, "analysis.db");
        var reportsDir = Path.Combine(dataDir, "reports");

        services.AddDbContext<AppDbContext>(opt => opt.UseSqlite($"Data Source={dbPath}"));

        services.AddScoped<ISubmissionsRepository, EfSubmissionsRepository>();
        services.AddScoped<IReportsRepository, EfReportsRepository>();
        services.AddSingleton<IReportFileWriter>(new LocalReportFileWriter(reportsDir));

        return services;
    }
}