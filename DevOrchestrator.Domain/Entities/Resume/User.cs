using System;
using System.Collections.Generic;

namespace DevOrchestrator.Domain.Resume;
public sealed class User
{
    private User() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; } = null!;
    public required string Email { get; set; } = null!;
    public required string Role { get; set; } = null!;

    public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}





