using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevOrchestrator.Domain.Services;

public interface IOllamaEmbeddingService
{
    /// <summary>
    /// Generates embeddings using a local Ollama instance.
    /// </summary>
    Task<float[]> GenerateEmbeddingAsync(string text, string model = "nomic-embed-text");

    /// <summary>
    /// Generates embeddings for multiple texts in batch.
    /// </summary>
    Task<List<float[]>> GenerateEmbeddingsBatchAsync(List<string> texts, string model = "nomic-embed-text");

    /// <summary>
    /// Gets the embedding dimension for the specified model.
    /// </summary>
    Task<int> GetEmbeddingDimensionAsync(string model = "nomic-embed-text");
}