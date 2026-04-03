using System;

namespace DevOrchestrator.Domain.Entities;

public class AiCache
{
    public Guid Id { get; set; }
    public string HashKey { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public string Response { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
