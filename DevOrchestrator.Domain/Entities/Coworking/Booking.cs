using System;

namespace DevOrchestrator.Domain.Coworking;
public sealed class Booking
{
    public Booking(Guid userId, Guid workspaceId, DateTimeOffset startTime, DateTimeOffset endTime, string status)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        WorkspaceId = workspaceId;
        StartTime = startTime;
        EndTime = endTime;
        Status = status;
    }

    public Guid Id { get; init; }
    public Guid UserId { get; set; }
    public Guid WorkspaceId { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public string Status { get; set; }

    public User? User { get; set; }
    public Workspace? Workspace { get; set; }
    public Payment? Payment { get; set; }
}





