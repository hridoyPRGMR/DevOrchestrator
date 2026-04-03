using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOrchestrator.Domain.Services;

namespace DevOrchestrator.Tools.Implementations;

public class GetGitHubFilesTool : Core.IMcpTool
{
    private readonly IGitHubApplicationService _gitHubApplicationService;

    public string Name => "get_github_files";

    public GetGitHubFilesTool(IGitHubApplicationService gitHubApplicationService)
    {
        _gitHubApplicationService = gitHubApplicationService;
    }

    public async Task<object> ExecuteAsync(Dictionary<string, object> args)
    {
        if (args == null || !args.TryGetValue("owner", out var ownerObj) || ownerObj == null)
        {
            return new { error = "Missing required argument: owner" };
        }

        if (!args.TryGetValue("repo", out var repoObj) || repoObj == null)
        {
            return new { error = "Missing required argument: repo" };
        }

        string owner = ownerObj.ToString() ?? string.Empty;
        string repo = repoObj.ToString() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(owner) || string.IsNullOrWhiteSpace(repo))
        {
            return new { error = "owner and repo cannot be empty" };
        }

        string? path = null;
        if (args.TryGetValue("path", out var pathObj) && pathObj != null)
            path = pathObj.ToString();

        string? branch = null;
        if (args.TryGetValue("branch", out var branchObj) && branchObj != null)
            branch = branchObj.ToString();

        try
        {
            var files = await _gitHubApplicationService.GetRepositoryFilesAsync(owner, repo, path, branch, forceRefresh: false);

            var fileList = files
                .Select(f => new
                {
                    f.Path,
                    f.Size,
                    f.Sha
                })
                .ToList();

            return new
            {
                success = true,
                owner,
                repo,
                path = path ?? "/",
                branch = branch ?? "main",
                fileCount = fileList.Count,
                files = fileList
            };
        }
        catch (Exception ex)
        {
            return new
            {
                success = false,
                error = "Failed to retrieve GitHub files",
                details = ex.Message
            };
        }
    }
}
