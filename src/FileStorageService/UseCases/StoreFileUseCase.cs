using Contracts;
using FileStorageService.UseCases.Abstractions;

namespace FileStorageService.UseCases;

public sealed class StoreFileUseCase : IStoreFileUseCase
{
    private readonly IFileStorage _storage;

    public StoreFileUseCase(IFileStorage storage)
    {
        _storage = storage;
    }

    public Task<StoreFileResponse> ExecuteAsync(Stream file, string originalFileName,
        CancellationToken cancellationToken) => _storage.SaveAsync(file, originalFileName, cancellationToken);
}