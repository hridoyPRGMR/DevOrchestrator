using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DevOrchestrator.Domain.Entities;
using DevOrchestrator.Domain.Services;
using DevOrchestrator.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DevOrchestrator.Infrastructure.Services;

public class RepositoryFileChunkService : IRepositoryFileChunkService
{
    private readonly AppDbContext _dbContext;
    private readonly ICodeChunker _codeChunker;
    private readonly IOllamaEmbeddingService _embeddingService;
    private readonly ILogger<RepositoryFileChunkService> _logger;

    public RepositoryFileChunkService(
        AppDbContext dbContext,
        ICodeChunker codeChunker,
        IOllamaEmbeddingService embeddingService,
        ILogger<RepositoryFileChunkService> logger)
    {
        _dbContext = dbContext;
        _codeChunker = codeChunker;
        _embeddingService = embeddingService;
        _logger = logger;
    }

    public async Task<List<RepositoryFileChunk>> ChunkAndStoreFileAsync(RepositoryFile repositoryFile, int maxTokensPerChunk = 512)
    {
        if (repositoryFile == null)
            throw new ArgumentNullException(nameof(repositoryFile));

        // Delete existing chunks for this file
        await DeleteChunksForFileAsync(repositoryFile.Id);

        // Chunk the file
        var chunks = await _codeChunker.ChunkCodeAsync(
            repositoryFile.Content,
            repositoryFile.FilePath,
            maxTokensPerChunk: maxTokensPerChunk
        );

        var fileChunks = new List<RepositoryFileChunk>();

        foreach (var chunk in chunks)
        {
            var contentHash = ComputeHash(chunk.Content);

            // Check if chunk already exists (deduplication)
            var existingChunk = await _dbContext.RepositoryFileChunks
                .FirstOrDefaultAsync(c => c.RepositoryId == repositoryFile.RepositoryId && c.ContentHash == contentHash);

            if (existingChunk != null)
            {
                // Reuse existing chunk
                fileChunks.Add(existingChunk);
                continue;
            }

            var fileChunk = new RepositoryFileChunk
            {
                Id = Guid.NewGuid(),
                RepositoryFileId = repositoryFile.Id,
                RepositoryId = repositoryFile.RepositoryId,
                ChunkIndex = chunk.Index,
                Content = chunk.Content,
                StartLineNumber = chunk.StartLineNumber,
                EndLineNumber = chunk.EndLineNumber,
                StartCharacterOffset = chunk.StartCharacterOffset,
                EndCharacterOffset = chunk.EndCharacterOffset,
                TokenCount = chunk.EstimatedTokenCount,
                ContentHash = contentHash,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.RepositoryFileChunks.Add(fileChunk);
            fileChunks.Add(fileChunk);
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Created {Count} chunks for file {FilePath}", fileChunks.Count, repositoryFile.FilePath);

        return fileChunks;
    }

    public async Task<int> EmbedChunksForRepositoryAsync(Guid repositoryId, string embeddingModel = "nomic-embed-text")
    {
        var unembeddedChunks = await _dbContext.RepositoryFileChunks
            .Where(c => c.RepositoryId == repositoryId && c.EmbeddingData == null)
            .ToListAsync();

        if (!unembeddedChunks.Any())
        {
            _logger.LogInformation("No unembedded chunks found for repository {RepositoryId}", repositoryId);
            return 0;
        }

        var texts = unembeddedChunks.Select(c => c.Content).ToList();
        var embeddings = await _embeddingService.GenerateEmbeddingsBatchAsync(texts, embeddingModel);

        for (int i = 0; i < unembeddedChunks.Count; i++)
        {
            unembeddedChunks[i].EmbeddingData = JsonSerializer.Serialize(embeddings[i]);
            unembeddedChunks[i].EmbeddingUpdatedAt = DateTime.UtcNow;
            unembeddedChunks[i].UpdatedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Generated embeddings for {Count} chunks in repository {RepositoryId}", unembeddedChunks.Count, repositoryId);

        return unembeddedChunks.Count;
    }

    public async Task<List<RepositoryFileChunk>> FindSimilarChunksAsync(Guid repositoryId, string query, int limit = 10, string embeddingModel = "nomic-embed-text")
    {
        var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(query, embeddingModel);

        // Get chunks with embeddings from database
        var chunksWithEmbeddings = await _dbContext.RepositoryFileChunks
            .Where(c => c.RepositoryId == repositoryId && c.EmbeddingData != null)
            .Include(c => c.RepositoryFile)
            .ToListAsync();

        // Calculate cosine similarity in memory
        var similarities = chunksWithEmbeddings
            .Select(chunk =>
            {
                try
                {
                    var chunkEmbedding = JsonSerializer.Deserialize<float[]>(chunk.EmbeddingData!);
                    var similarity = CosineSimilarity(queryEmbedding, chunkEmbedding);
                    return new { Chunk = chunk, Similarity = similarity };
                }
                catch
                {
                    return new { Chunk = chunk, Similarity = 0.0 };
                }
            })
            .OrderByDescending(x => x.Similarity)
            .Take(limit)
            .Select(x => x.Chunk)
            .ToList();

        return similarities;
    }

    public async Task<List<RepositoryFileChunk>> GetChunksForFileAsync(Guid repositoryFileId)
    {
        return await _dbContext.RepositoryFileChunks
            .Where(c => c.RepositoryFileId == repositoryFileId)
            .OrderBy(c => c.ChunkIndex)
            .ToListAsync();
    }

    public async Task DeleteChunksForFileAsync(Guid repositoryFileId)
    {
        var chunks = await _dbContext.RepositoryFileChunks
            .Where(c => c.RepositoryFileId == repositoryFileId)
            .ToListAsync();

        if (chunks.Any())
        {
            _dbContext.RepositoryFileChunks.RemoveRange(chunks);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Deleted {Count} chunks for file {FileId}", chunks.Count, repositoryFileId);
        }
    }

    private static string ComputeHash(string content)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(content);
        return Convert.ToBase64String(sha.ComputeHash(bytes));
    }

    private static double CosineSimilarity(float[] a, float[] b)
    {
        if (a == null || b == null || a.Length != b.Length)
            return 0.0;

        double dotProduct = 0.0;
        double normA = 0.0;
        double normB = 0.0;

        for (int i = 0; i < a.Length; i++)
        {
            dotProduct += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }

        if (normA == 0.0 || normB == 0.0)
            return 0.0;

        return dotProduct / (Math.Sqrt(normA) * Math.Sqrt(normB));
    }
}