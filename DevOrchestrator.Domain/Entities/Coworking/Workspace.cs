using System;
using System.Collections.Generic;

namespace DevOrchestrator.Domain.Coworking;
public sealed class Workspace
{
    private Workspace() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; } = null!;
    public required string Location { get; set; } = null!;
    public int Capacity { get; set; }
    public required string Type { get; set; } = null!;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}





