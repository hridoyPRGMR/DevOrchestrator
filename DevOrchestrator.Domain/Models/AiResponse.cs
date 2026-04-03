namespace DevOrchestrator.Domain.Models;

public record AiResponse(
    string Content,
    int InputTokens,
    int OutputTokens,
    decimal Cost,
    string Model,
    bool CacheHit
);