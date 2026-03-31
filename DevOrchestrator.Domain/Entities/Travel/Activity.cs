using System;

namespace DevOrchestrator.Domain.Travel;
public sealed class Activity
{
    private Activity() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required Guid DayPlanId { get; set; }
    public required string Name { get; set; } = null!;
    public required string Type { get; set; } = null!;
    public required DateTimeOffset StartTime { get; set; }
    public required DateTimeOffset EndTime { get; set; }
    public required string Location { get; set; } = null!;

    public Guid? PlaceId { get; set; }
    public DayPlan? DayPlan { get; set; }
    public Place? Place { get; set; }
}





