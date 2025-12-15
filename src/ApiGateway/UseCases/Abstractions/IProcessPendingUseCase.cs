namespace ApiGateway.UseCases.Abstractions;

public interface IProcessPendingUseCase
{
    Task<int> ExecuteAsync(CancellationToken ct);
}