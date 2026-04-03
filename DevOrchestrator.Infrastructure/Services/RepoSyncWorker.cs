using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevOrchestrator.Domain.Services;
using DevOrchestrator.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DevOrchestrator.Infrastructure.Services;

public class RepoSyncWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RepoSyncWorker> _logger;

    public RepoSyncWorker(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<RepoSyncWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var syncIntervalMinutes = _configuration.GetValue<int>("BackgroundSync:IntervalMinutes", 5);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SyncActiveRepositoriesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                // Log error - for now, just continue
                _logger.LogError(ex, "Background sync error");
            }

            await Task.Delay(TimeSpan.FromMinutes(syncIntervalMinutes), stoppingToken);
        }
    }

    private async Task SyncActiveRepositoriesAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var syncService = scope.ServiceProvider.GetRequiredService<IGitHubSyncService>();

        // Get repositories accessed in the last 24 hours
        var activeSince = DateTime.UtcNow.AddHours(-24);
        var activeRepos = await dbContext.GitHubRepositories
            .Where(r => r.LastAccessedAt.HasValue && r.LastAccessedAt.Value > activeSince)
            .ToListAsync(stoppingToken);

        foreach (var repo in activeRepos)
        {
            if (stoppingToken.IsCancellationRequested)
                break;

            try
            {
                // Force refresh to ensure data is up to date
                await syncService.GetRepositoryFilesAsync(repo.Owner, repo.Name, forceRefresh: true);
            }
            catch (Exception ex)
            {
                // Log error for this repo, but continue with others
                _logger.LogError(ex, "Failed to sync repository {Owner}/{Name}", repo.Owner, repo.Name);
            }
        }
    }
}