using System;
using System.Collections.Generic;

namespace DevOrchestrator.Domain.DocumentSummarizer;
public sealed class User
{
    private User() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; } = null!;
    public required string Email { get; set; } = null!;

    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}





