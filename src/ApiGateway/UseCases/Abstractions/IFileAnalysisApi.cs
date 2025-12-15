using Contracts;

namespace ApiGateway.UseCases.Abstractions;

public interface IFileAnalysisApi
{
    Task<AnalyzeWorkResponse> AnalyzeAsync(AnalyzeWorkRequest request, CancellationToken ct);
}