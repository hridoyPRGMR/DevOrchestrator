using System;

namespace DevOrchestrator.Domain.Entities;

public class AiCache
{
    public Guid Id { get; set; }
    public string HashKey { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public string Response { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastAccessedAt { get; set; }
    public int AccessCount { get; set; } = 0;
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public string Model { get; set; } = string.Empty;
    public decimal Cost { get; set; }
}
