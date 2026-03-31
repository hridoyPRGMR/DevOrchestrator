using System;

namespace DevOrchestrator.Domain.Resume;
public sealed class CoverLetter
{
    private CoverLetter() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required Guid ResumeId { get; set; }
    public required Guid JobPostingId { get; set; }
    public required string Content { get; set; } = null!;
    public required DateTimeOffset GeneratedAt { get; set; }

    public Resume? Resume { get; set; }
    public JobPosting? JobPosting { get; set; }
}





