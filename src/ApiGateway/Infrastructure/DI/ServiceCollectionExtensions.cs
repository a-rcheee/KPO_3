using ApiGateway.Infrastructure.Http;
using ApiGateway.UseCases.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ApiGateway.Infrastructure.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string dataDir, string storageBaseUrl, string analysisBaseUrl)
    {
        Directory.CreateDirectory(dataDir);

        var dbPath = Path.Combine(dataDir, "gateway.db");
        services.AddDbContext<GatewayDbContext>(opt => opt.UseSqlite($"Data Source={dbPath}"));
        services.AddScoped<IPendingTasksRepository, EfPendingTasksRepository>();

        services.AddHttpClient<IFileStorageApi, HttpFileStorageApi>(c => c.BaseAddress = new Uri(storageBaseUrl));
        services.AddHttpClient<IFileAnalysisApi, HttpFileAnalysisApi>(c => c.BaseAddress = new Uri(analysisBaseUrl));

        return services;
    }
}