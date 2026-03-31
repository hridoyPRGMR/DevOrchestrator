using System;

namespace DevOrchestrator.Domain.Support;
public sealed class KnowledgeBaseArticle
{
    private KnowledgeBaseArticle() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required string Title { get; set; } = null!;
    public required string Content { get; set; } = null!;
    public string? Tags { get; set; }
    public required string VectorEmbedding { get; set; } = null!;
}





