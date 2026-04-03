using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevOrchestrator.Domain.Entities;
using DevOrchestrator.Domain.Services;
using Microsoft.Extensions.Logging;

namespace DevOrchestrator.Infrastructure.Services;

public class ContextBuilder : IContextBuilder
{
    private readonly IRepositoryFileChunkService _chunkService;
    private readonly IOllamaEmbeddingService _embeddingService;
    private readonly ILogger<ContextBuilder> _logger;
    private const int EstimatedCharsPerToken = 4;

    public ContextBuilder(
        IRepositoryFileChunkService chunkService,
        IOllamaEmbeddingService embeddingService,
        ILogger<ContextBuilder> logger)
    {
        _chunkService = chunkService;
        _embeddingService = embeddingService;
        _logger = logger;
    }

    public async Task<string> BuildContextAsync(Guid repositoryId, string query, int maxTokens = 8000)
    {
        try
        {
            // Find similar chunks using semantic search
            var similarChunks = await _chunkService.FindSimilarChunksAsync(repositoryId, query, limit: 20);

            if (!similarChunks.Any())
            {
                _logger.LogWarning("No similar chunks found for query '{Query}' in repository {RepositoryId}", query, repositoryId);
                return string.Empty;
            }

            // Sort by relevance (assuming the SQL query already sorts by similarity)
            var selectedChunks = similarChunks
                .OrderBy(c => c.ChunkIndex) // Group by file, then by chunk order
                .ThenBy(c => c.RepositoryFileId)
                .ToList();

            // Build context with token budget
            var contextBuilder = new StringBuilder();
            var totalTokens = 0;
            var maxChars = maxTokens * EstimatedCharsPerToken;

            foreach (var chunk in selectedChunks)
            {
                var chunkText = FormatChunk(chunk);
                var chunkChars = chunkText.Length;

                if (contextBuilder.Length + chunkChars > maxChars)
                {
                    _logger.LogInformation("Reached token limit ({MaxTokens} tokens) for context building", maxTokens);
                    break;
                }

                contextBuilder.AppendLine(chunkText);
                contextBuilder.AppendLine(); // Add spacing between chunks
                totalTokens += chunk.TokenCount;
            }

            var finalContext = contextBuilder.ToString().Trim();

            _logger.LogInformation("Built context with {ChunkCount} chunks, {TotalTokens} tokens for query '{Query}'",
                selectedChunks.Count, totalTokens, query);

            return finalContext;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building context for query '{Query}' in repository {RepositoryId}", query, repositoryId);
            return string.Empty;
        }
    }

    private string FormatChunk(RepositoryFileChunk chunk)
    {
        var fileName = chunk.RepositoryFile?.FilePath ?? "unknown";
        return $"File: {fileName} (lines {chunk.StartLineNumber}-{chunk.EndLineNumber})\n{chunk.Content}";
    }
}