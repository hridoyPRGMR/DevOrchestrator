using System;

namespace DevOrchestrator.Contracts.Dto;

public sealed class BookingReadDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid WorkspaceId { get; init; }
    public DateTimeOffset StartTime { get; init; }
    public DateTimeOffset EndTime { get; init; }
    public string Status { get; init; } = null!;
}

public sealed class BookingCreateDto
{
    public Guid UserId { get; set; }
    public Guid WorkspaceId { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public string Status { get; set; } = null!;
}
