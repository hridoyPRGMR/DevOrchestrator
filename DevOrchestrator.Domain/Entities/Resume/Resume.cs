using System;
using System.Collections.Generic;

namespace DevOrchestrator.Domain.Resume;
public sealed class Resume
{
    private Resume() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required Guid UserId { get; set; }
    public required string Content { get; set; } = null!;
    public required DateTimeOffset UploadedAt { get; set; }

    public User? User { get; set; }
    public ICollection<CoverLetter> CoverLetters { get; set; } = new List<CoverLetter>();
}





