using DevOrchestrator.Domain.Coworking;

namespace DevOrchestrator.Contracts.Dto;

public static class BookingExtensions
{
    public static BookingReadDto ToReadDto(this Booking booking) => new BookingReadDto
    {
        Id = booking.Id,
        UserId = booking.UserId,
        WorkspaceId = booking.WorkspaceId,
        StartTime = booking.StartTime,
        EndTime = booking.EndTime,
        Status = booking.Status
    };

    public static Booking ToDomain(this BookingCreateDto createDto)
    {
        return new Booking(
            createDto.UserId,
            createDto.WorkspaceId,
            createDto.StartTime,
            createDto.EndTime,
            createDto.Status);
    }
}
