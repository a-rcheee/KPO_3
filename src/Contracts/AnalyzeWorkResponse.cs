namespace Contracts;

public sealed record AnalyzeWorkResponse(
    Guid SubmissionId,
    Guid ReportId,
    bool IsPlagiarism,
    Guid? MatchId,
    string Reason
        
);