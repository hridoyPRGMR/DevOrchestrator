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

        services.AddHttpClient();

        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IAiService, AiService>();
        services.AddScoped<IUsageTracker, UsageTracker>();
        services.AddScoped<IPricingService, PricingService>();
        services.AddScoped<IAgentEngine, AgentEngine>();
        services.AddScoped<IEmbeddingService, EmbeddingService>();
        services.AddScoped<ICodeChunker, CodeChunker>();
        services.AddScoped<IOllamaEmbeddingService, OllamaEmbeddingService>();
        services.AddScoped<IRepositoryFileChunkService, RepositoryFileChunkService>();
        services.AddScoped<IContextBuilder, ContextBuilder>();
        services.AddScoped<IGitHubService, GitHubService>();
        services.AddScoped<IGitHubSyncService, GitHubSyncService>();
        services.AddScoped<IGitHubCacheService, GitHubCacheService>();

        services.AddToolServices();

        services.AddHostedService<RepoSyncWorker>();

        return services;
    }
}
