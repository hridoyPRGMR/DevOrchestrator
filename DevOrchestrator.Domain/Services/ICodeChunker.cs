using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevOrchestrator.Domain.Services;

public interface ICodeChunker
{
    /// <summary>
    /// Chunks code content into logical segments based on language-specific patterns.
    /// </summary>
    Task<List<CodeChunk>> ChunkCodeAsync(string content, string filePath, string? language = null, int maxTokensPerChunk = 512);
}

public record CodeChunk(
    int Index,
    string Content,
    int StartLineNumber,
    int EndLineNumber,
    int StartCharacterOffset,
    int EndCharacterOffset,
    int EstimatedTokenCount
);