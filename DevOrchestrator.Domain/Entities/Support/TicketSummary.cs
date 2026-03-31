using System;

namespace DevOrchestrator.Domain.Support;
public sealed class TicketSummary
{
    private TicketSummary() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required Guid TicketId { get; set; }
    public required string SummaryText { get; set; } = null!;
    public required DateTimeOffset GeneratedAt { get; set; }

    public Ticket? Ticket { get; set; }
}





