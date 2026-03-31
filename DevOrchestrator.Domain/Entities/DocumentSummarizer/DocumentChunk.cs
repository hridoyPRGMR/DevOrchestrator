using System;
using System.Collections.Generic;

namespace DevOrchestrator.Domain.DocumentSummarizer;
public sealed class DocumentChunk
{
    private DocumentChunk() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required Guid DocumentId { get; set; }
    public required string ChunkText { get; set; } = null!;
    public required string VectorEmbedding { get; set; } = null!;

    public Document? Document { get; set; }
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}





