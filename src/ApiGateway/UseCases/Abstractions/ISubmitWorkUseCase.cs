namespace ApiGateway.UseCases.Abstractions;

public interface ISubmitWorkUseCase
{
    Task<SubmitWorkResult> ExecuteAsync(string workId, string studentName, Stream file, string fileName, CancellationToken ct);
}