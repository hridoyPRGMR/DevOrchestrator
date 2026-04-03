using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOrchestrator.Domain.Services;

namespace DevOrchestrator.Application.Services;

public class GitHubApplicationService : IGitHubApplicationService
{
    private readonly IGitHubSyncService _gitHubSyncService;

    public GitHubApplicationService(IGitHubSyncService gitHubSyncService)
    {
        _gitHubSyncService = gitHubSyncService;
    }

    public async Task<IEnumerable<GitHubFile>> GetRepositoryFilesAsync(string owner, string repo, string? path = null, string? branch = null, bool forceRefresh = false)
    {
        var files = await _gitHubSyncService.GetRepositoryFilesAsync(owner, repo, path, branch, forceRefresh);

        // Business logic: filter to files only, limit to 50
        return files
            .Where(f => f.Type == "file")
            .Take(50)
            .ToList();
    }

    public async Task<string> GetFileContentAsync(string owner, string repo, string filePath, string? branch = null, bool forceRefresh = false)
    {
        return await _gitHubSyncService.GetFileContentAsync(owner, repo, filePath, branch, forceRefresh);
    }
}