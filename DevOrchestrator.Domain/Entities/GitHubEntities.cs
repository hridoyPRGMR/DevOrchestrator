using System.Collections.Generic;

namespace DevOrchestrator.Domain.Entities;

public class GitHubRepository
{
    public Guid Id { get; set; }
    public string Owner { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string RepositoryUrl { get; set; } = string.Empty;
    public string DefaultBranch { get; set; } = "main";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastSyncedAt { get; set; }
    public DateTime? LastAccessedAt { get; set; }

    public ICollection<RepositoryFile> Files { get; set; } = new List<RepositoryFile>();
}

public class RepositoryFile
{
    public Guid Id { get; set; }
    public Guid RepositoryId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string FileHash { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public float[]? Embedding { get; set; }
    public DateTime? EmbeddingUpdatedAt { get; set; }

    public GitHubRepository Repository { get; set; } = null!;
}
