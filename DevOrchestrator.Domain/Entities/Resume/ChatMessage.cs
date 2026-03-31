using System;

namespace DevOrchestrator.Domain.Resume;
public sealed class ChatMessage
{
    private ChatMessage() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required Guid UserId { get; set; }
    public required string Message { get; set; } = null!;
    public required string AIResponse { get; set; } = null!;
    public required DateTimeOffset Timestamp { get; set; }

    public User? User { get; set; }
}





