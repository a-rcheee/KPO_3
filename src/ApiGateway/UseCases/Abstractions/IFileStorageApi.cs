using Contracts;

namespace ApiGateway.UseCases.Abstractions;

public interface IFileStorageApi
{
    Task<StoreFileResponse> StoreAsync(Stream fileStream, string fileName, CancellationToken ct);
}