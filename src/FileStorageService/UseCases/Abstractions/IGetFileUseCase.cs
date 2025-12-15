namespace FileStorageService.UseCases.Abstractions;

public interface IGetFileUseCase
{
    Task<(Stream Stream, string FileName)?> ExecuteAsync(string fileId, CancellationToken cancellationToken);
}