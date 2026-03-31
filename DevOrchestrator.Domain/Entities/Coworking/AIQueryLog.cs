using System;

namespace DevOrchestrator.Domain.Coworking;
public sealed class AIQueryLog
{
    AIQueryLog (){}
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required Guid UserId { get; set; }
    public required string Query { get; set; } = null!;
    public required string Response { get; set; } = null!;
    public required DateTimeOffset Timestamp { get; set; }

    public Guid? ChatMessageId { get; set; }
    public ChatMessage? ChatMessage { get; set; }
    public User? User { get; set; }
}





