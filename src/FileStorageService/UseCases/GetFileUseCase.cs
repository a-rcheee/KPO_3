using FileStorageService.UseCases.Abstractions;

namespace FileStorageService.UseCases;

public sealed class GetFileUseCase : IGetFileUseCase
{
    private readonly IFileStorage _storage;

    public GetFileUseCase(IFileStorage storage)
    {
        _storage = storage;
    }

    public Task<(Stream Stream, string FileName)?> ExecuteAsync(string fileId,
        CancellationToken cancellationToken) => _storage.GetAsync(fileId, cancellationToken);
    
    
}