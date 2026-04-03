using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DevOrchestrator.Domain.Services;
using Microsoft.Extensions.Logging;

namespace DevOrchestrator.Infrastructure.Services;

public class CodeChunker : ICodeChunker
{
    private readonly ILogger<CodeChunker> _logger;
    private const int EstimatedCharsPerToken = 4;
    private const int DefaultMaxTokensPerChunk = 512;

    public CodeChunker(ILogger<CodeChunker> logger)
    {
        _logger = logger;
    }

    public Task<List<CodeChunk>> ChunkCodeAsync(string content, string filePath, string? language = null, int maxTokensPerChunk = 512)
    {
        if (string.IsNullOrEmpty(content))
            return Task.FromResult(new List<CodeChunk>());

        try
        {
            var maxCharsPerChunk = maxTokensPerChunk * EstimatedCharsPerToken;
            var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            // Determine language if not provided
            language = language ?? DetectLanguage(filePath);

            var chunks = new List<CodeChunk>();
            var currentChunk = new List<string>();
            var startLine = 0;
            var chunkIndex = 0;
            var currentCharOffset = 0;

            foreach (var (line, lineIndex) in lines.Select((l, i) => (l, i)))
            {
                var lineWithNewline = line + Environment.NewLine;
                var lineLength = lineWithNewline.Length;

                // Check if adding this line would exceed chunk size
                if (currentChunk.Count > 0 && currentCharOffset + lineLength > maxCharsPerChunk)
                {
                    // Save current chunk
                    var chunkContent = string.Join("", currentChunk).TrimEnd();
                    if (!string.IsNullOrWhiteSpace(chunkContent))
                    {
                        chunks.Add(CreateChunk(
                            chunkIndex++,
                            chunkContent,
                            startLine,
                            lineIndex - 1,
                            currentCharOffset - chunkContent.Length,
                            currentCharOffset
                        ));

                        // Reset for next chunk
                        currentChunk.Clear();
                        startLine = lineIndex;
                    }
                }

                currentChunk.Add(lineWithNewline);
                currentCharOffset += lineLength;
            }

            // Add final chunk if content remains
            if (currentChunk.Any())
            {
                var chunkContent = string.Join("", currentChunk).TrimEnd();
                if (!string.IsNullOrWhiteSpace(chunkContent))
                {
                    chunks.Add(CreateChunk(
                        chunkIndex,
                        chunkContent,
                        startLine,
                        lines.Length - 1,
                        currentCharOffset - chunkContent.Length,
                        currentCharOffset
                    ));
                }
            }

            return Task.FromResult(chunks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error chunking code for {FilePath}", filePath);
            return Task.FromResult(new List<CodeChunk>());
        }
    }

    private CodeChunk CreateChunk(int index, string content, int startLine, int endLine, int startOffset, int endOffset)
    {
        return new CodeChunk(
            Index: index,
            Content: content,
            StartLineNumber: startLine,
            EndLineNumber: endLine,
            StartCharacterOffset: startOffset,
            EndCharacterOffset: endOffset,
            EstimatedTokenCount: (int)Math.Ceiling(content.Length / (double)EstimatedCharsPerToken)
        );
    }

    private string DetectLanguage(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();

        return extension switch
        {
            ".cs" => "csharp",
            ".java" => "java",
            ".py" => "python",
            ".js" => "javascript",
            ".ts" => "typescript",
            ".cpp" or ".cc" or ".cxx" => "cpp",
            ".c" => "c",
            ".h" or ".hpp" => "header",
            ".go" => "go",
            ".rs" => "rust",
            ".php" => "php",
            ".rb" => "ruby",
            ".json" => "json",
            ".xml" => "xml",
            ".yaml" or ".yml" => "yaml",
            ".sql" => "sql",
            _ => "text"
        };
    }
}