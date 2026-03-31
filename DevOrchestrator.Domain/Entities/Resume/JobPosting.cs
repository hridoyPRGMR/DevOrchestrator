using System.Collections.Generic;

namespace DevOrchestrator.Domain.Resume;
public sealed class JobPosting
{
    private JobPosting() { }
    public required Guid Id { get; set; } = System.Guid.NewGuid();
    public required string Company { get; set; } = null!;
    public required string Position { get; set; } = null!;
    public required string Description { get; set; } = null!;
    public required string Requirements { get; set; } = null!;

    public ICollection<CoverLetter> CoverLetters { get; set; } = new List<CoverLetter>();
}





