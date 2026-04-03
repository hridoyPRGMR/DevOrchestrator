using DevOrchestrator.Domain.Entities;

namespace DevOrchestrator.Contracts.Dto;

public static class RepositoryExtensions
{
    public static RepositoryDto ToDto(this GitHubRepository repository)
    {
        return new RepositoryDto
        {
            Id = repository.Id,
            Owner = repository.Owner,
            Name = repository.Name,
            Description = repository.Description,
            RepositoryUrl = repository.RepositoryUrl,
            DefaultBranch = repository.DefaultBranch,
            CreatedAt = repository.CreatedAt,
            LastSyncedAt = repository.LastSyncedAt,
            LastAccessedAt = repository.LastAccessedAt,
            FileCount = repository.Files?.Count ?? 0
        };
    }

    public static FileDto ToDto(this RepositoryFile file)
    {
        return new FileDto
        {
            Path = file.FilePath,
            Sha = file.FileHash,
            Size = file.FileSize,
            Type = "file", // Assuming all RepositoryFile entities are files
            LastModified = file.UpdatedAt ?? file.CreatedAt
        };
    }

    public static FileContentDto ToContentDto(this RepositoryFile file)
    {
        return new FileContentDto
        {
            RepositoryId = file.RepositoryId,
            FilePath = file.FilePath,
            Content = file.Content,
            Sha = file.FileHash,
            Size = file.FileSize,
            LastModified = file.UpdatedAt ?? file.CreatedAt
        };
    }
}