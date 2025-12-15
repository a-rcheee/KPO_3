using ApiGateway.UseCases.Abstractions;
using Contracts;

namespace ApiGateway.Infrastructure.Http;

public sealed class HttpFileAnalysisApi : IFileAnalysisApi
{
    private readonly HttpClient _http;
    public HttpFileAnalysisApi(HttpClient http)
    {
        _http = http;
    }
    public async Task<AnalyzeWorkResponse> AnalyzeAsync(AnalyzeWorkRequest request, CancellationToken ct)
    {
        var resp = await _http.PostAsJsonAsync("/submissions", request, ct);
        resp.EnsureSuccessStatusCode();

        return (await resp.Content.ReadFromJsonAsync<AnalyzeWorkResponse>(cancellationToken: ct))!;
    }
}