using System;

namespace DevOrchestrator.Domain.Coworking;
public sealed class Payment
{
    private Payment() { }
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required Guid BookingId { get; set; }
    public required decimal Amount { get; set; }
    public required string PaymentStatus { get; set; } = null!;
    public required DateTimeOffset PaymentDate { get; set; }

    public Booking? Booking { get; set; }
}





