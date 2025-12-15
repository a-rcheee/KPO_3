namespace ApiGateway.Entities;

public sealed class PendingAnalysisTask
{
    public Guid Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string WorkId { get; set; } = "";
    public string StudentName { get; set; } = "";
    public string FileId { get; set; } = "";
    public string Sha256 { get; set; } = "";
    public string OriginalFileName { get; set; } = "";
    public int Attempts { get; set; }
}