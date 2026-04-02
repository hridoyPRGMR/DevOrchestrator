using DevOrchestrator.Domain.Coworking;
using DevOrchestrator.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevOrchestrator.Application.Services
{
    public class BookingService(
        AppDbContext _context
    )
    {
        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings.Include(b => b.User).Include(b => b.Workspace).ToListAsync();
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }
    }
}