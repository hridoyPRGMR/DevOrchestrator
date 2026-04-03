using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOrchestrator.Domain.Services;

namespace DevOrchestrator.Tools.Implementations;

public class FindSimilarCodeTool : Core.IMcpTool
{
    private readonly IRepositoryFileChunkService _chunkService;
    private readonly IEmbeddingService _embeddingService;

    public string Name => "find_similar_code";

    public FindSimilarCodeTool(IRepositoryFileChunkService chunkService, IEmbeddingService embeddingService)
    {
        _chunkService = chunkService;
        _embeddingService = embeddingService;
    }

    public async Task<object> ExecuteAsync(Dictionary<string, object> args)
    {
        if (args == null || !args.TryGetValue("query", out var queryObj))
        {
            return new { error = "Missing required argument: query" };
        }

        var query = queryObj.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(query))
        {
            return new { error = "Query cannot be empty" };
        }

        var repositoryId = args.TryGetValue("repositoryId", out var repoIdObj) &&
                          Guid.TryParse(repoIdObj.ToString(), out var repoId) ? repoId : (Guid?)null;

        var limit = args.TryGetValue("limit", out var limitObj) &&
                   int.TryParse(limitObj.ToString(), out var limitVal) ? limitVal : 5;

        try
        {
            // Find similar chunks using the query text directly
            var similarChunks = await _chunkService.FindSimilarChunksAsync(repositoryId.Value, query, limit);

            return new
            {
                query,
                results = similarChunks.Select(chunk => new
                {
                    chunk.Id,
                    chunk.RepositoryFileId,
                    chunk.Content,
                    filePath = chunk.RepositoryFile?.FilePath ?? "unknown",
                    startLine = chunk.StartLineNumber,
                    endLine = chunk.EndLineNumber,
                    tokenCount = chunk.TokenCount
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            return new { error = $"Failed to find similar code: {ex.Message}" };
        }
    }
}