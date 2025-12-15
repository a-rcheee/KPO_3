using System.Security.Cryptography;
using Contracts;
using FileStorageService.UseCases.Abstractions;

namespace FileStorageService.Infrastructure;

public class LocalFileStorage : IFileStorage
{
    private readonly string _filesDir;

    public LocalFileStorage(string dataDir)
    {
        _filesDir = Path.Combine(dataDir, "files");
        Directory.CreateDirectory(_filesDir);
    }

    public async Task<StoreFileResponse> SaveAsync(Stream file, string originalFileName, CancellationToken ct)
    {
        var safeName = Path.GetFileName(originalFileName);
        var fileId = Guid.NewGuid().ToString("N");
        var savedPath = Path.Combine(_filesDir, $"{fileId}_{safeName}");

        await using (var outStream = File.Create(savedPath))
        {
            await file.CopyToAsync(outStream, ct);
        }

        await using var readStream = File.OpenRead(savedPath);
        var sha256 = await ComputeHashAsync(readStream, ct);

        return new StoreFileResponse(fileId, safeName, sha256);
    }

    public Task<(Stream Stream, string FileName)?> GetAsync(string fileId, CancellationToken ct)
    {
        var files = Directory.GetFiles(_filesDir, $"{fileId}_*");
        if (files.Length == 0)
        {
            return Task.FromResult<(Stream Stream, string FileName)?>(null);
        }

        var path = files[0];
        var parts = Path.GetFileName(path).Split('_', 2);
        var fileName = parts.Length == 2 ? parts[1] : "file";
        Stream stream = File.OpenRead(path);
        return Task.FromResult<(Stream Stream, string FileName)?>((stream, fileName));
    }

    private static async Task<string> ComputeHashAsync(Stream stream, CancellationToken ct)
    {
        using var sha256 = SHA256.Create();

        if (stream.CanSeek)
        {
            stream.Position = 0;
        }
        var bytes = await sha256.ComputeHashAsync(stream, ct);
        return Convert.ToHexString(bytes);
    }
}