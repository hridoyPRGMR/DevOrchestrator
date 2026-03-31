using System;

namespace DevOrchestrator.Domain.DocumentSummarizer;
public sealed class DocumentSummary
{
    private DocumentSummary() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required Guid DocumentId { get; set; }
    public required string SummaryText { get; set; } = null!;
    public required DateTimeOffset GeneratedAt { get; set; }

    public Document? Document { get; set; }
}





