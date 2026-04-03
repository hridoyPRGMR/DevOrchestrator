using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOrchestrator.Domain.Services;

namespace DevOrchestrator.Application.Services;

public interface IGitHubApplicationService
{
    Task<IEnumerable<GitHubFile>> GetRepositoryFilesAsync(string owner, string repo, string? path = null, string? branch = null, bool forceRefresh = false);
    Task<string> GetFileContentAsync(string owner, string repo, string filePath, string? branch = null, bool forceRefresh = false);
}