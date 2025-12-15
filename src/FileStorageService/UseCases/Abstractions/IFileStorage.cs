using Contracts;

namespace FileStorageService.UseCases.Abstractions;

public interface IFileStorage
{
    Task<StoreFileResponse> SaveAsync(Stream file, string originalFileName, CancellationToken ct);
    Task<(Stream Stream, string FileName)?> GetAsync(string fileId, CancellationToken ct);
}