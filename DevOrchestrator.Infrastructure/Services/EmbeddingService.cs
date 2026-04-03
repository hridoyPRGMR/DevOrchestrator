using System;
using System.Text.Json;
using System.Threading.Tasks;
using DevOrchestrator.Domain.Entities;
using DevOrchestrator.Domain.Services;
using DevOrchestrator.Infrastructure.Persistence;

namespace DevOrchestrator.Infrastructure.Services;

public class EmbeddingService : IEmbeddingService
{
    private readonly AppDbContext _dbContext;
    private readonly IAiService _aiService;

    public EmbeddingService(AppDbContext dbContext, IAiService aiService)
    {
        _dbContext = dbContext;
        _aiService = aiService;
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text, string? model = null)
    {
        var response = await _aiService.EmbedAsync(text, model);
        return JsonSerializer.Deserialize<float[]>(response.Content) ?? Array.Empty<float>();
    }

    public async Task<float[]> GetOrCreateEmbeddingAsync(RepositoryFile repositoryFile, string? model = null)
    {
        if (repositoryFile == null) throw new ArgumentNullException(nameof(repositoryFile));

        if (repositoryFile.Embedding != null && repositoryFile.Embedding.Length > 0)
        {
            // If embedding is recent (24h), reuse
            if (repositoryFile.EmbeddingUpdatedAt.HasValue && DateTime.UtcNow - repositoryFile.EmbeddingUpdatedAt.Value < TimeSpan.FromHours(24))
            {
                return repositoryFile.Embedding;
            }
        }

        var response = await _aiService.EmbedAsync(repositoryFile.Content, model);
        var embedding = JsonSerializer.Deserialize<float[]>(response.Content) ?? Array.Empty<float>();

        repositoryFile.Embedding = embedding;
        repositoryFile.EmbeddingUpdatedAt = DateTime.UtcNow;

        _dbContext.RepositoryFiles.Update(repositoryFile);
        await _dbContext.SaveChangesAsync();

        return embedding;
    }
}
