using Contracts;

namespace ApiGateway.UseCases;

public sealed record SubmitWorkResult(
    string Status, StoreFileResponse StoredFile,
    AnalyzeWorkResponse? Analysis, string? Message);