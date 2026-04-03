using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOrchestrator.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Octokit;
using Polly;
using Polly.Retry;

namespace DevOrchestrator.Infrastructure.Services;

public class GitHubService : IGitHubService
{
    private readonly GitHubClient _client;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly ILogger<GitHubService> _logger;

    public GitHubService(IConfiguration configuration, ILogger<GitHubService> logger)
    {
        _client = new GitHubClient(new ProductHeaderValue("DevOrchestrator"));
        var token = configuration["GitHub:Token"];
        if (!string.IsNullOrWhiteSpace(token))
        {
            _client.Credentials = new Credentials(token);
        }

        _logger = logger;

        // Configure retry policy for rate limits and transient errors
        _retryPolicy = Policy
            .Handle<ApiException>(ex => ex.StatusCode == System.Net.HttpStatusCode.Forbidden) // Rate limit
            .Or<ApiException>(ex => ex.StatusCode >= System.Net.HttpStatusCode.InternalServerError) // Server errors
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // Exponential backoff
                (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(exception, "GitHub API retry {RetryCount} after {TimeSpan} due to: {Message}", retryCount, timeSpan, exception.Message);
                });
    }

    public async Task<IEnumerable<GitHubFile>> FetchRepositoryFilesAsync(string owner, string repo, string? path = null, string? branch = null)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                var contentPath = path ?? string.Empty;
                IReadOnlyList<RepositoryContent> result;
                if (!string.IsNullOrWhiteSpace(branch))
                {
                    result = await _client.Repository.Content.GetAllContentsByRef(owner, repo, contentPath, branch);
                }
                else
                {
                    result = await _client.Repository.Content.GetAllContents(owner, repo, contentPath);
                }

                var files = new List<GitHubFile>();
                foreach (var item in result)
                {
                    files.Add(new GitHubFile
                    {
                        Path = item.Path,
                        Sha = item.Sha,
                        Size = item.Size,
                        Type = item.Type.ToString().ToLower(),
                        LastModified = DateTime.UtcNow
                    });
                }

                return files;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Repository {Owner}/{Repo} not found", owner, repo);
                return Enumerable.Empty<GitHubFile>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GitHub API error fetching files for {Owner}/{Repo}", owner, repo);
                return Enumerable.Empty<GitHubFile>();
            }
        });
    }

    public async Task<(string Content, string Sha)> FetchFileContentAsync(string owner, string repo, string filePath, string? branch = null)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                IReadOnlyList<RepositoryContent> contents;
                if (!string.IsNullOrWhiteSpace(branch))
                {
                    contents = await _client.Repository.Content.GetAllContentsByRef(owner, repo, filePath, branch);
                }
                else
                {
                    contents = await _client.Repository.Content.GetAllContents(owner, repo, filePath);
                }

                var file = contents.FirstOrDefault();

                if (file != null && file.Content != null)
                {
                    return (file.Content, file.Sha);
                }

                return (string.Empty, string.Empty);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "File {FilePath} not found in {Owner}/{Repo}", filePath, owner, repo);
                return (string.Empty, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GitHub API error fetching file content for {Owner}/{Repo}/{FilePath}", owner, repo, filePath);
                return (string.Empty, string.Empty);
            }
        });
    }
}
