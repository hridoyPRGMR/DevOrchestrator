using DevOrchestrator.Application.Services;
using DevOrchestrator.Contracts.Dto;
using DevOrchestrator.Domain.Coworking;
using Microsoft.AspNetCore.Mvc;

namespace DevOrchestrator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController(BookingService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await service.GetAllBookingsAsync();
            var dto = bookings.Select(b => b.ToReadDto());
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookingCreateDto bookingCreateDto)
        {
            var booking = bookingCreateDto.ToDomain();
            var created = await service.CreateBookingAsync(booking);
            return Ok(created.ToReadDto());
        }
    }
}