using System;

namespace DevOrchestrator.Domain.Entities;

public class RepositoryFileChunk
{
    public Guid Id { get; set; }
    public Guid RepositoryFileId { get; set; }
    public Guid RepositoryId { get; set; }
    public int ChunkIndex { get; set; }
    public string Content { get; set; } = string.Empty;
    public string ContentHash { get; set; } = string.Empty;
    public int StartLineNumber { get; set; }
    public int EndLineNumber { get; set; }
    public int StartCharacterOffset { get; set; }
    public int EndCharacterOffset { get; set; }
    public string? EmbeddingData { get; set; }
    public DateTime? EmbeddingUpdatedAt { get; set; }
    public int TokenCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public RepositoryFile RepositoryFile { get; set; } = null!;
}