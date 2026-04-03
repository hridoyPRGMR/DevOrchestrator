using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevOrchestrator.Domain.Services;

public interface IGitHubService
{
    Task<IEnumerable<GitHubFile>> FetchRepositoryFilesAsync(string owner, string repo, string? path = null, string? branch = null);
    Task<(string Content, string Sha)> FetchFileContentAsync(string owner, string repo, string filePath, string? branch = null);
}

public class GitHubFile
{
    public string Path { get; set; } = string.Empty;
    public string Sha { get; set; } = string.Empty;
    public long Size { get; set; }
    public string Type { get; set; } = string.Empty; // "file" or "dir"
    public string? Content { get; set; }
    public DateTime? LastModified { get; set; }
}
