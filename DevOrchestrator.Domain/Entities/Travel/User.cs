using System;
using System.Collections.Generic;

namespace DevOrchestrator.Domain.Travel;
public sealed class User
{
    private User() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; } = null!;
    public required string Email { get; set; } = null!;
    public required string Preferences { get; set; } = null!;

    public ICollection<Trip> Trips { get; set; } = new List<Trip>();
}





