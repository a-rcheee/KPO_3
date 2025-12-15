namespace FileAnalysisService.Entities;

public class Report
{
    public Guid Id { get; set; }
    public Guid SubmissionId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public bool IsPlagiarism { get; set; }
    public Guid? MatchId { get; set; }
    public string Reason { get; set; } = "";
}