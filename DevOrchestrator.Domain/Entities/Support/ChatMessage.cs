using System;

namespace DevOrchestrator.Domain.Support;
public sealed class ChatMessage
{
    private ChatMessage() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required Guid AgentId { get; set; }
    public required Guid? TicketId { get; set; }
    public required string UserQuery { get; set; } = null!;
    public required string AIResponse { get; set; } = null!;
    public required DateTimeOffset Timestamp { get; set; }

    public User? Agent { get; set; }
    public Ticket? Ticket { get; set; }
}





