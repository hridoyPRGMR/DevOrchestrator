using System;

namespace DevOrchestrator.Domain.Coworking;
public sealed class Booking
{
    private Booking() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required Guid UserId { get; set; }
    public required Guid WorkspaceId { get; set; }
    public required DateTimeOffset StartTime { get; set; }
    public required DateTimeOffset EndTime { get; set; }
    public required string Status { get; set; } = null!;

    public User? User { get; set; }
    public Workspace? Workspace { get; set; }
    public Payment? Payment { get; set; }
}





