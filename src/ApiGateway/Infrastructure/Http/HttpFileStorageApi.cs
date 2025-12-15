using ApiGateway.UseCases.Abstractions;
using Contracts;

namespace ApiGateway.Infrastructure.Http;

public sealed class HttpFileStorageApi: IFileStorageApi
{
    private readonly HttpClient _http;
    public HttpFileStorageApi(HttpClient http)
    {
        _http = http;
    }

    public async Task<StoreFileResponse> StoreAsync(Stream fileStream, string fileName, CancellationToken ct)
    {
        using var content = new MultipartFormDataContent();
        content.Add(new StreamContent(fileStream), "file", fileName);
        var resp = await _http.PostAsync("/files", content, ct);
        resp.EnsureSuccessStatusCode();

        return (await resp.Content.ReadFromJsonAsync<StoreFileResponse>(cancellationToken: ct))!;
    }
}