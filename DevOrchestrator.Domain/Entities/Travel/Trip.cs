using System;
using System.Collections.Generic;

namespace DevOrchestrator.Domain.Travel;
public sealed class Trip
{
    private Trip() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required Guid UserId { get; set; }
    public required DateTimeOffset StartDate { get; set; }
    public required DateTimeOffset EndDate { get; set; }
    public required string DestinationCity { get; set; } = null!;
    public string? Notes { get; set; }

    public User? User { get; set; }
    public ICollection<DayPlan> DayPlans { get; set; } = new List<DayPlan>();
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}





