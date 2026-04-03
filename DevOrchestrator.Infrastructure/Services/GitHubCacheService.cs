using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using DevOrchestrator.Domain.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;

namespace DevOrchestrator.Infrastructure.Services;

public class GitHubCacheService : IGitHubCacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConfiguration _configuration;

    public GitHubCacheService(IDistributedCache cache, IConfiguration configuration)
    {
        _cache = cache;
        _configuration = configuration;
    }

    public async Task<IEnumerable<GitHubFile>?> GetRepositoryFilesAsync(string owner, string repo, string? path, string? branch)
    {
        var key = GetFilesKey(owner, repo, path, branch);
        var cached = await _cache.GetStringAsync(key);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<IEnumerable<GitHubFile>>(cached);
        }
        return null;
    }

    public async Task SetRepositoryFilesAsync(string owner, string repo, string? path, string? branch, IEnumerable<GitHubFile> files)
    {
        var key = GetFilesKey(owner, repo, path, branch);
        var ttlMinutes = _configuration.GetValue<int>("Redis:FileCacheTTLMinutes", 60);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ttlMinutes)
        };
        var json = JsonSerializer.Serialize(files);
        await _cache.SetStringAsync(key, json, options);
    }

    public async Task<string?> GetFileContentAsync(string owner, string repo, string filePath, string? branch)
    {
        var key = GetContentKey(owner, repo, filePath, branch);
        return await _cache.GetStringAsync(key);
    }

    public async Task SetFileContentAsync(string owner, string repo, string filePath, string? branch, string content)
    {
        var key = GetContentKey(owner, repo, filePath, branch);
        var ttlMinutes = _configuration.GetValue<int>("Redis:FileCacheTTLMinutes", 60);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ttlMinutes)
        };
        await _cache.SetStringAsync(key, content, options);
    }

    private static string GetFilesKey(string owner, string repo, string? path, string? branch)
    {
        return $"github:files:{owner}:{repo}:{path ?? "/"}:{branch ?? "main"}";
    }

    private static string GetContentKey(string owner, string repo, string filePath, string? branch)
    {
        return $"github:content:{owner}:{repo}:{filePath}:{branch ?? "main"}";
    }
}