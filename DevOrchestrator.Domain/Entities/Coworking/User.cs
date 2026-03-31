using System;
using System.Collections.Generic;

namespace DevOrchestrator.Domain.Coworking;
public sealed class User
{
    private User() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; } = null!;
    public required string Email { get; set; } = null!;
    public required string Role { get; set; } = null!;
    public required string PasswordHash { get; set; } = null!;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}





