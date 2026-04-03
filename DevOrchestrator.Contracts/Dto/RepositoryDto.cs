using System.Collections.Generic;

namespace DevOrchestrator.Contracts.Dto;

// Repository DTOs
public class RepositoryDto
{
    public Guid Id { get; set; }
    public string Owner { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string RepositoryUrl { get; set; } = string.Empty;
    public string DefaultBranch { get; set; } = "main";
    public DateTime CreatedAt { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    public int FileCount { get; set; }
}

public class RepositoryFilesResponseDto
{
    public Guid RepositoryId { get; set; }
    public string? Path { get; set; }
    public List<FileDto> Files { get; set; } = new();
}

public class FileDto
{
    public string Path { get; set; } = string.Empty;
    public string Sha { get; set; } = string.Empty;
    public long Size { get; set; }
    public string Type { get; set; } = string.Empty; // "file" or "dir"
    public DateTime? LastModified { get; set; }
}

// File DTOs
public class FileContentDto
{
    public Guid RepositoryId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Sha { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime? LastModified { get; set; }
}

public class FileAnalysisDto
{
    public Guid RepositoryId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public int LineCount { get; set; }
    public Dictionary<string, int> LanguageStats { get; set; } = new();
    public List<string> Issues { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
}