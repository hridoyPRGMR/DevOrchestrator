using System;

namespace DevOrchestrator.Domain.Entities;

public class AiUsageLog
{
    public Guid Id { get; set; }
    public string RequestId { get; set; } = string.Empty;
    public Guid? RepositoryId { get; set; }
    public string OperationType { get; set; } = string.Empty; // e.g., "analyze_code", "generate_text"
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public decimal Cost { get; set; }
    public bool CacheHit { get; set; }
    public string Model { get; set; } = string.Empty; // e.g., "gpt-4", "claude-3"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}