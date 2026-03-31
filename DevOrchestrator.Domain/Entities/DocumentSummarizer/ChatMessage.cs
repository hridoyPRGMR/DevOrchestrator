using System;
using System.Collections.Generic;

namespace DevOrchestrator.Domain.DocumentSummarizer;
public sealed class ChatMessage
{
    private ChatMessage() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required Guid UserId { get; set; }
    public required string Question { get; set; } = null!;
    public required string AIAnswer { get; set; } = null!;
    public required DateTimeOffset Timestamp { get; set; }

    public User? User { get; set; }
    public ICollection<DocumentChunk> RetrievedChunks { get; set; } = new List<DocumentChunk>();
}





