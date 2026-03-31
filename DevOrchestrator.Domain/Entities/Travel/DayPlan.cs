using System;
using System.Collections.Generic;

namespace DevOrchestrator.Domain.Travel;
public sealed class DayPlan
{
    private DayPlan() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required Guid TripId { get; set; }
    public required DateOnly Date { get; set; }
    public required string Activities { get; set; } = null!;

    public Trip? Trip { get; set; }
    public ICollection<Activity> ActivitiesList { get; set; } = new List<Activity>();
}





