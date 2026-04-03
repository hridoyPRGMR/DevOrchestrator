using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevOrchestrator.Domain.Entities;

namespace DevOrchestrator.Domain.Services;

public interface IRepositoryFileChunkService
{
    /// <summary>
    /// Creates chunks for a repository file and stores them in the database.
    /// </summary>
    Task<List<RepositoryFileChunk>> ChunkAndStoreFileAsync(RepositoryFile repositoryFile, int maxTokensPerChunk = 512);

    /// <summary>
    /// Generates embeddings for all unchunked files in a repository.
    /// </summary>
    Task<int> EmbedChunksForRepositoryAsync(Guid repositoryId, string embeddingModel = "nomic-embed-text");

    /// <summary>
    /// Finds chunks similar to the given query text using semantic search.
    /// </summary>
    Task<List<RepositoryFileChunk>> FindSimilarChunksAsync(Guid repositoryId, string query, int limit = 10, string embeddingModel = "nomic-embed-text");

    /// <summary>
    /// Gets all chunks for a specific file.
    /// </summary>
    Task<List<RepositoryFileChunk>> GetChunksForFileAsync(Guid repositoryFileId);

    /// <summary>
    /// Deletes all chunks for a specific file.
    /// </summary>
    Task DeleteChunksForFileAsync(Guid repositoryFileId);
}