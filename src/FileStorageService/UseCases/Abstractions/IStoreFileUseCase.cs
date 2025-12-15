using Contracts;

namespace FileStorageService.UseCases.Abstractions;

public interface IStoreFileUseCase
{
    Task<StoreFileResponse> ExecuteAsync(Stream file, string originalFileName, CancellationToken cancellationToken);
}