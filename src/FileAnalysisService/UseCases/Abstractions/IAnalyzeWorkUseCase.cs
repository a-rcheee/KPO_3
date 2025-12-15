using Contracts;

namespace FileAnalysisService.UseCases.Abstractions;

public interface IAnalyzeWorkUseCase
{
    Task<AnalyzeWorkResponse> ExecuteAsync(AnalyzeWorkRequest request, CancellationToken ct); 
}