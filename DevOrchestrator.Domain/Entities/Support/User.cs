using System;
using System.Collections.Generic;

namespace DevOrchestrator.Domain.Support;
public sealed class User
{
    private User() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; } = null!;
    public required string Email { get; set; } = null!;
    public required string Role { get; set; } = null!;

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}





