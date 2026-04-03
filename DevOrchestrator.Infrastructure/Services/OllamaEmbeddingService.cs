using System.Net.Http.Json;
using System.Text.Json;
using DevOrchestrator.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DevOrchestrator.Infrastructure.Services;

public class OllamaEmbeddingService : IOllamaEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OllamaEmbeddingService> _logger;
    private readonly string _ollamaBaseUrl;
    private readonly Dictionary<string, int> _modelDimensions = new();

    public OllamaEmbeddingService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<OllamaEmbeddingService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _ollamaBaseUrl = configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text, string model = "nomic-embed-text")
    {
        try
        {
            var request = new { prompt = text, model };
            var response = await _httpClient.PostAsJsonAsync($"{_ollamaBaseUrl}/api/embeddings", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaEmbeddingResponse>();
            return result?.Embedding ?? Array.Empty<float>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating embedding with Ollama for model {Model}", model);
            throw;
        }
    }

    public async Task<List<float[]>> GenerateEmbeddingsBatchAsync(List<string> texts, string model = "nomic-embed-text")
    {
        var embeddings = new List<float[]>();

        // Process in smaller batches to avoid overwhelming Ollama
        const int batchSize = 10;
        for (int i = 0; i < texts.Count; i += batchSize)
        {
            var batch = texts.Skip(i).Take(batchSize).ToList();
            var tasks = batch.Select(text => GenerateEmbeddingAsync(text, model));
            var batchResults = await Task.WhenAll(tasks);
            embeddings.AddRange(batchResults);
        }

        return embeddings;
    }

    public async Task<int> GetEmbeddingDimensionAsync(string model = "nomic-embed-text")
    {
        if (_modelDimensions.TryGetValue(model, out var dimension))
            return dimension;

        try
        {
            // Generate a dummy embedding to determine dimension
            var embedding = await GenerateEmbeddingAsync("test", model);
            var dim = embedding.Length;
            _modelDimensions[model] = dim;
            return dim;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error determining embedding dimension for model {Model}", model);
            throw;
        }
    }

    private class OllamaEmbeddingResponse
    {
        public float[] Embedding { get; set; } = Array.Empty<float>();
    }
}