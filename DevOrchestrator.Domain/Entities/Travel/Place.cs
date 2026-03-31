using System;
using System.Collections.Generic;

namespace DevOrchestrator.Domain.Travel;
public sealed class Place
{
    private Place() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; } = null!;
    public required string Type { get; set; } = null!;
    public required string Coordinates { get; set; } = null!;
    public required string InfoUrl { get; set; } = null!;
    public required string ImageUrl { get; set; } = null!;

    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
}





