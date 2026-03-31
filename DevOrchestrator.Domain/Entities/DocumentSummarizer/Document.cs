using System;
using System.Collections.Generic;

namespace DevOrchestrator.Domain.DocumentSummarizer;
public sealed class Document
{
    private Document() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required Guid UserId { get; set; }
    public required string FileName { get; set; } = null!;
    public required string FileType { get; set; } = null!;
    public required string Content { get; set; } = null!;
    public required DateTimeOffset UploadedAt { get; set; }

    public User? User { get; set; }
    public DocumentSummary? DocumentSummary { get; set; }
    public ICollection<DocumentChunk> DocumentChunks { get; set; } = new List<DocumentChunk>();
}





