namespace FileAnalysisService.Entities;

public class Submission
{
    public Guid Id { get; set; }
    public string WorkId { get; set; } = "";
    public string StudentName { get; set; } = "";
    public DateTime SubmittedAtUtc { get; set; }
    public string FileId { get; set; } = "";
    public string Sha256 { get; set; } = "";
    public string OriginalFileName { get; set; } = "";
}