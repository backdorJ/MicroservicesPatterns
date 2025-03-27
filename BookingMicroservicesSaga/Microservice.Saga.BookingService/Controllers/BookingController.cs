using BookingService.Models.CreateBooking;
using BookingService.Services.BookingService;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers;

[ApiController]
[Route("[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task CreateBooking([FromBody] CreateBookingRequest request, CancellationToken cancellationToken)
        => await _bookingService.CreateBooking(request, cancellationToken);
}