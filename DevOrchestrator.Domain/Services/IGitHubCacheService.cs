using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevOrchestrator.Domain.Services;

public interface IGitHubCacheService
{
    Task<IEnumerable<GitHubFile>?> GetRepositoryFilesAsync(string owner, string repo, string? path, string? branch);
    Task SetRepositoryFilesAsync(string owner, string repo, string? path, string? branch, IEnumerable<GitHubFile> files);
    Task<string?> GetFileContentAsync(string owner, string repo, string filePath, string? branch);
    Task SetFileContentAsync(string owner, string repo, string filePath, string? branch, string content);
}