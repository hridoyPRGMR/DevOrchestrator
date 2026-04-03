using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOrchestrator.Domain.Entities;
using DevOrchestrator.Domain.Services;
using DevOrchestrator.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;

namespace DevOrchestrator.Infrastructure.Services;

public class GitHubSyncService : IGitHubSyncService
{
    private readonly AppDbContext _dbContext;
    private readonly IGitHubService _gitHubService;
    private readonly IGitHubCacheService _cacheService;
    private readonly IConfiguration _configuration;

    public GitHubSyncService(AppDbContext dbContext, IGitHubService gitHubService, IGitHubCacheService cacheService, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _gitHubService = gitHubService;
        _cacheService = cacheService;
        _configuration = configuration;
    }

    public async Task<IEnumerable<GitHubFile>> GetRepositoryFilesAsync(string owner, string repo, string? path = null, string? branch = null, bool forceRefresh = false)
    {
        var repository = _dbContext.GitHubRepositories
            .FirstOrDefault(r => r.Owner.ToLower() == owner.ToLower() && 
                                r.Name.ToLower() == repo.ToLower());
                                
        // Check Redis cache first
        if (!forceRefresh)
        {
            var cachedFiles = await _cacheService.GetRepositoryFilesAsync(owner, repo, path, branch);
            if (cachedFiles != null)
            {
                // Update last accessed
                if (repository != null)
                {
                    repository.LastAccessedAt = DateTime.UtcNow;
                    _dbContext.GitHubRepositories.Update(repository);
                    await _dbContext.SaveChangesAsync();
                }
                return cachedFiles;
            }
        }

        var stalenessIntervalMinutes = _configuration.GetValue<int>("GitHub:StalenessCheckIntervalMinutes", 10);
        var isStale = repository == null || 
                     !repository.LastSyncedAt.HasValue || 
                     (DateTime.UtcNow - repository.LastSyncedAt.Value).TotalMinutes > stalenessIntervalMinutes;

        if (!forceRefresh && repository != null && !isStale)
        {
            // 1. Normalize the path first to handle nulls/empty consistently
            var normalizedPath = (path == "/" || string.IsNullOrWhiteSpace(path)) ? "" : path;

            var cachedFiles = _dbContext.RepositoryFiles
                .Where(f => f.RepositoryId == repository.Id)
                .Where(f => string.IsNullOrEmpty(normalizedPath) || f.FilePath.ToLower().StartsWith(normalizedPath.ToLower()))
                .ToList();

            if (cachedFiles.Any())
            {
                var result = cachedFiles
                    .Select(f => new GitHubFile
                    {
                        Path = f.FilePath,
                        Sha = f.FileHash,
                        Size = f.FileSize,
                        Type = "file",
                        LastModified = f.UpdatedAt ?? f.CreatedAt
                    })
                    .ToList();

                // Update last accessed and cache in Redis
                repository.LastAccessedAt = DateTime.UtcNow;
                _dbContext.GitHubRepositories.Update(repository);
                await _dbContext.SaveChangesAsync();
                await _cacheService.SetRepositoryFilesAsync(owner, repo, path, branch, result);
                return result;
            }
        }

        // Sync from GitHub
        var files = await _gitHubService.FetchRepositoryFilesAsync(owner, repo, path, branch);
        var fetchedFileList = files.ToList();

        if (repository == null)
        {
            repository = new GitHubRepository
            {
                Id = Guid.NewGuid(),
                Owner = owner,
                Name = repo,
                RepositoryUrl = $"https://github.com/{owner}/{repo}",
                DefaultBranch = branch ?? "main",
                CreatedAt = DateTime.UtcNow,
                LastSyncedAt = DateTime.UtcNow
            };
            _dbContext.GitHubRepositories.Add(repository);
        }
        else
        {
            repository.LastSyncedAt = DateTime.UtcNow;
            _dbContext.GitHubRepositories.Update(repository);
        }

        foreach (var file in fetchedFileList)
        {
            if (file.Type != "file")
                continue;

            var existing = _dbContext.RepositoryFiles
                .FirstOrDefault(f => f.RepositoryId == repository.Id && f.FilePath == file.Path);

            if (existing == null)
            {
                _dbContext.RepositoryFiles.Add(new RepositoryFile
                {
                    Id = Guid.NewGuid(),
                    RepositoryId = repository.Id,
                    FilePath = file.Path,
                    Content = file.Content ?? string.Empty,
                    FileHash = file.Sha,
                    FileSize = file.Size,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            else if (existing.FileHash != file.Sha)
            {
                // SHA changed, update metadata and content if available
                existing.FileHash = file.Sha;
                existing.FileSize = file.Size;
                if (!string.IsNullOrEmpty(file.Content))
                {
                    existing.Content = file.Content;
                }
                existing.UpdatedAt = DateTime.UtcNow;
                _dbContext.RepositoryFiles.Update(existing);
            }
            // If SHA same, no update needed
        }

        await _dbContext.SaveChangesAsync();

        // Cache in Redis
        await _cacheService.SetRepositoryFilesAsync(owner, repo, path, branch, fetchedFileList);

        // Update last accessed
        if (repository != null)
        {
            repository.LastAccessedAt = DateTime.UtcNow;
            _dbContext.GitHubRepositories.Update(repository);
            await _dbContext.SaveChangesAsync();
        }

        return fetchedFileList;
    }

    public async Task<string> GetFileContentAsync(string owner, string repo, string filePath, string? branch = null, bool forceRefresh = false)
    {
        var repository = _dbContext.GitHubRepositories
            .FirstOrDefault(r => r.Owner.Equals(owner, StringComparison.OrdinalIgnoreCase) && r.Name.Equals(repo, StringComparison.OrdinalIgnoreCase));

        // Check Redis cache first
        if (!forceRefresh)
        {
            var cachedContent = await _cacheService.GetFileContentAsync(owner, repo, filePath, branch);
            if (cachedContent != null)
            {
                // Update last accessed
                if (repository != null)
                {
                    repository.LastAccessedAt = DateTime.UtcNow;
                    _dbContext.GitHubRepositories.Update(repository);
                    await _dbContext.SaveChangesAsync();
                }
                return cachedContent;
            }
        }

        var stalenessIntervalMinutes = _configuration.GetValue<int>("GitHub:StalenessCheckIntervalMinutes", 10);
        var isStale = repository == null || 
                     !repository.LastSyncedAt.HasValue || 
                     (DateTime.UtcNow - repository.LastSyncedAt.Value).TotalMinutes > stalenessIntervalMinutes;

        if (!forceRefresh && repository != null && !isStale)
        {
            var cached = _dbContext.RepositoryFiles
                .FirstOrDefault(f => f.RepositoryId == repository.Id && f.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase));
            if (cached != null)
            {
                // Update last accessed and cache in Redis
                repository.LastAccessedAt = DateTime.UtcNow;
                _dbContext.GitHubRepositories.Update(repository);
                await _dbContext.SaveChangesAsync();
                await _cacheService.SetFileContentAsync(owner, repo, filePath, branch, cached.Content);
                return cached.Content;
            }
        }

        // fallback to remote fetch and save to DB
        var (content, sha) = await _gitHubService.FetchFileContentAsync(owner, repo, filePath, branch);

        if (string.IsNullOrEmpty(content))
            return string.Empty;

        if (repository == null)
        {
            repository = new GitHubRepository
            {
                Id = Guid.NewGuid(),
                Owner = owner,
                Name = repo,
                RepositoryUrl = $"https://github.com/{owner}/{repo}",
                DefaultBranch = branch ?? "main",
                CreatedAt = DateTime.UtcNow,
                LastSyncedAt = DateTime.UtcNow
            };
            _dbContext.GitHubRepositories.Add(repository);
            await _dbContext.SaveChangesAsync();
        }

        var file = _dbContext.RepositoryFiles
            .FirstOrDefault(f => f.RepositoryId == repository.Id && f.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase));

        if (file == null)
        {
            _dbContext.RepositoryFiles.Add(new RepositoryFile
            {
                Id = Guid.NewGuid(),
                RepositoryId = repository.Id,
                FilePath = filePath,
                Content = content,
                FileHash = sha,
                FileSize = content.Length,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
        else if (file.FileHash != sha)
        {
            file.Content = content;
            file.FileHash = sha;
            file.FileSize = content.Length;
            file.UpdatedAt = DateTime.UtcNow;
            _dbContext.RepositoryFiles.Update(file);
        }

        repository.LastSyncedAt = DateTime.UtcNow;
        _dbContext.GitHubRepositories.Update(repository);

        await _dbContext.SaveChangesAsync();

        // Cache in Redis
        await _cacheService.SetFileContentAsync(owner, repo, filePath, branch, content);

        // Update last accessed
        if (repository != null)
        {
            repository.LastAccessedAt = DateTime.UtcNow;
            _dbContext.GitHubRepositories.Update(repository);
            await _dbContext.SaveChangesAsync();
        }

        return content;
    }
}
