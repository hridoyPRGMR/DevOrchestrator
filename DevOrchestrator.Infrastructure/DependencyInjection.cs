using DevOrchestrator.Domain.Services;
using DevOrchestrator.Infrastructure.Services;
using DevOrchestrator.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevOrchestrator.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<Persistence.AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis:ConnectionString"] ?? "localhost:6379";
        });

        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IAiService, AiService>();
        services.AddScoped<IGitHubService, GitHubService>();
        services.AddScoped<IGitHubSyncService, GitHubSyncService>();
        services.AddScoped<IGitHubCacheService, GitHubCacheService>();

        services.AddToolServices();

        services.AddHostedService<RepoSyncWorker>();

        return services;
    }
}
