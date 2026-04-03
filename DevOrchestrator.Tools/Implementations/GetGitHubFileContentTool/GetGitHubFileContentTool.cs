using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevOrchestrator.Domain.Services;

namespace DevOrchestrator.Tools.Implementations;

public class GetGitHubFileContentTool : Core.IMcpTool
{
    private readonly IGitHubApplicationService _gitHubApplicationService;

    public string Name => "get_github_file_content";

    public GetGitHubFileContentTool(IGitHubApplicationService gitHubApplicationService)
    {
        _gitHubApplicationService = gitHubApplicationService;
    }

    public async Task<object> ExecuteAsync(Dictionary<string, object> args)
    {
        if (args == null || !args.TryGetValue("owner", out var ownerObj) || ownerObj == null)
            return new { error = "Missing required argument: owner" };

        if (!args.TryGetValue("repo", out var repoObj) || repoObj == null)
            return new { error = "Missing required argument: repo" };

        if (!args.TryGetValue("filePath", out var filePathObj) || filePathObj == null)
            return new { error = "Missing required argument: filePath" };

        string owner = ownerObj.ToString() ?? string.Empty;
        string repo = repoObj.ToString() ?? string.Empty;
        string filePath = filePathObj.ToString() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(owner) || string.IsNullOrWhiteSpace(repo) || string.IsNullOrWhiteSpace(filePath))
            return new { error = "owner, repo, and filePath cannot be empty" };

        string? branch = null;
        if (args.TryGetValue("branch", out var branchObj) && branchObj != null)
            branch = branchObj.ToString();

        try
        {
            var content = await _gitHubApplicationService.GetFileContentAsync(owner, repo, filePath, branch);

            return new
            {
                success = true,
                owner,
                repo,
                filePath,
                branch = branch ?? "main",
                content
            };
        }
        catch (Exception ex)
        {
            return new
            {
                success = false,
                error = "Failed to retrieve GitHub file content",
                details = ex.Message
            };
        }
    }
}
