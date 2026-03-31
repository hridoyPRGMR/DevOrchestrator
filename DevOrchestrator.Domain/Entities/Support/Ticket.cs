using System;
using System.Collections.Generic;

namespace DevOrchestrator.Domain.Support;
public sealed class Ticket
{
    private Ticket() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required Guid UserId { get; set; }
    public required string Title { get; set; } = null!;
    public required string Description { get; set; } = null!;
    public required string Status { get; set; } = null!;
    public required string Priority { get; set; } = null!;
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset UpdatedAt { get; set; }

    public User? User { get; set; }
    public TicketSummary? TicketSummary { get; set; }
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}





