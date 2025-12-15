namespace Contracts;

public sealed record AnalyzeWorkRequest(
    string WorkId,
    string StudentName,
    string FileId,
    string Sha256,
    string OriginalFileName
);