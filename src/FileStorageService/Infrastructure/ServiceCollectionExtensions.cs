using FileStorageService.UseCases.Abstractions;

namespace FileStorageService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string dataDir)
    {
        Directory.CreateDirectory(dataDir);

        services.AddSingleton<IFileStorage>(new LocalFileStorage(dataDir));

        return services;
    }
}