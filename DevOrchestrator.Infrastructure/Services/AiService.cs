using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DevOrchestrator.Domain.Models;
using DevOrchestrator.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DevOrchestrator.Infrastructure.Services;

public class AiService : IAiService
{
    private readonly ICacheService _cacheService;
    private readonly HttpClient _httpClient;
    private readonly ILogger<AiService> _logger;
    private readonly string _openAiApiKey;
    private readonly IUsageTracker _usageTracker;
    private readonly IPricingService _pricingService;

    public AiService(
        ICacheService cacheService,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<AiService> logger,
        IUsageTracker usageTracker,
        IPricingService pricingService)
    {
        _cacheService = cacheService;
        _logger = logger;
        _openAiApiKey = configuration["OpenAI:ApiKey"] ?? string.Empty;
        _usageTracker = usageTracker;
        _pricingService = pricingService;

        _httpClient = httpClientFactory.CreateClient("OpenAI");
        _httpClient.BaseAddress = new Uri(configuration["OpenAI:BaseUrl"]?.TrimEnd('/') ?? "https://api.openai.com/v1");

        if (!string.IsNullOrEmpty(_openAiApiKey))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _openAiApiKey);
        }
    }

    public async Task<string> AnalyzeCodeAsync(string code, string? language = null)
    {
        var prompt = new StringBuilder();
        prompt.AppendLine("You are a code review assistant.");
        if (!string.IsNullOrWhiteSpace(language))
        {
            prompt.AppendLine($"Language: {language}");
        }

        prompt.AppendLine("Analyze the following code and provide bug risks, clean-up suggestions, and potential improvements:");
        prompt.AppendLine("---");
        prompt.AppendLine(code);
        prompt.AppendLine("---");

        var response = await GenerateAsync(prompt.ToString(), null);
        return response.Content;
    }

    public async Task<AiResponse> GenerateAsync(string prompt, string? model = null)
    {
        var actualModel = model ?? "gpt-4o-mini";
        string hashKey = GenerateHashKey("generate:" + actualModel + ":" + prompt);

        var cached = await _cacheService.GetAsync(hashKey);
        if (!string.IsNullOrEmpty(cached))
        {
            // For cached responses, we don't have token info, so return minimal response
            return new AiResponse(cached, 0, 0, 0, actualModel, true);
        }

        if (string.IsNullOrEmpty(_openAiApiKey))
        {
            _logger.LogWarning("OpenAI API key is missing. Using deterministic fallback for generate.");
            var fallback = $"[FALLBACK] {prompt[..Math.Min(prompt.Length, 128)]}";
            await _cacheService.SetAsync(hashKey, prompt, fallback);

            return new AiResponse(fallback, EstimateTokens(prompt), EstimateTokens(fallback), 0, actualModel, false);
        }

        var requestBody = new
        {
            model = actualModel,
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            temperature = 0.2,
            max_tokens = 512
        };

        var httpResponse = await _httpClient.PostAsJsonAsync("/chat/completions", requestBody);
        httpResponse.EnsureSuccessStatusCode();

        using var jsonDoc = await JsonDocument.ParseAsync(await httpResponse.Content.ReadAsStreamAsync());
        var choice = jsonDoc.RootElement.GetProperty("choices")[0];
        var content = choice.GetProperty("message").GetProperty("content").GetString() ?? string.Empty;

        // Extract token usage from response
        var usage = jsonDoc.RootElement.GetProperty("usage");
        var inputTokens = usage.GetProperty("prompt_tokens").GetInt32();
        var outputTokens = usage.GetProperty("completion_tokens").GetInt32();

        // Calculate cost
        var cost = await _pricingService.CalculateCostAsync(actualModel, inputTokens, outputTokens);

        var aiResponse = new AiResponse(content.Trim(), inputTokens, outputTokens, cost, actualModel, false);

        // Log usage
        await _usageTracker.LogUsageAsync(aiResponse, Guid.NewGuid().ToString(), null, "generate");

        await _cacheService.SetAsync(hashKey, prompt, content);

        return aiResponse;
    }

    public async Task<AiResponse> EmbedAsync(string text, string? model = null)
    {
        var actualModel = model ?? "text-embedding-3-small";
        string hashKey = GenerateHashKey("embed:" + actualModel + ":" + text);

        var cached = await _cacheService.GetAsync(hashKey);
        if (!string.IsNullOrEmpty(cached))
        {
            // For cached responses, we don't have token info, so return minimal response
            return new AiResponse(cached, 0, 0, 0, actualModel, true);
        }

        if (string.IsNullOrEmpty(_openAiApiKey))
        {
            _logger.LogWarning("OpenAI API key is missing. Using deterministic fallback for embedding.");
            var fallback = GenerateDeterministicVector(text, 1536);
            var fallbackEmbeddingJson = JsonSerializer.Serialize(fallback);
            await _cacheService.SetAsync(hashKey, text, fallbackEmbeddingJson);

            return new AiResponse(fallbackEmbeddingJson, EstimateTokens(text), 0, 0, actualModel, false);
        }

        var requestBody = new
        {
            model = actualModel,
            input = text
        };

        var httpResponse = await _httpClient.PostAsJsonAsync("/embeddings", requestBody);
        httpResponse.EnsureSuccessStatusCode();

        using var jsonDoc = await JsonDocument.ParseAsync(await httpResponse.Content.ReadAsStreamAsync());
        var embeddingArray = jsonDoc.RootElement.GetProperty("data")[0].GetProperty("embedding").EnumerateArray();

        var embedding = embeddingArray.Select(x => x.GetSingle()).ToArray();
        var serializedEmbedding = JsonSerializer.Serialize(embedding);

        // Extract token usage from response (embeddings only have prompt_tokens)
        var usage = jsonDoc.RootElement.GetProperty("usage");
        var inputTokens = usage.GetProperty("prompt_tokens").GetInt32();

        // Calculate cost (embeddings only have input cost)
        var cost = await _pricingService.CalculateCostAsync(actualModel, inputTokens, 0);

        var aiResponse = new AiResponse(serializedEmbedding, inputTokens, 0, cost, actualModel, false);

        // Log usage
        await _usageTracker.LogUsageAsync(aiResponse, Guid.NewGuid().ToString(), null, "embed");

        await _cacheService.SetAsync(hashKey, text, serializedEmbedding);

        return aiResponse;
    }

    private static int EstimateTokens(string text)
    {
        // Rough estimation: ~4 characters per token for English text
        // This is a simplified approximation for fallback scenarios
        return Math.Max(1, text.Length / 4);
    }

    private static float[] GenerateDeterministicVector(string text, int dimension)
    {
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
        var vector = new float[dimension];

        for (var i = 0; i < dimension; i++)
        {
            vector[i] = (hash[i % hash.Length] / 255f) * 2f - 1f;
        }

        return vector;
    }

    private static string GenerateHashKey(string input)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(hash);
    }
}


