using System.Threading.Tasks;
using DevOrchestrator.Domain.Entities;

namespace DevOrchestrator.Domain.Services;

public interface IEmbeddingService
{
    Task<float[]> GenerateEmbeddingAsync(string text, string? model = null);
    Task<float[]> GetOrCreateEmbeddingAsync(RepositoryFile repositoryFile, string? model = null);
}