using System.Security.Claims;
using HMS.Services.Abstraction;
using HMS.Shared.DTOs.BookingDTOs;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers
{
    public class BookingsController : ApiBaseController
    {
        private readonly IBookingService _bookingService;
        private readonly IPaymentService _paymentService;

        public BookingsController(IBookingService bookingService, IPaymentService paymentService)
        {
            _bookingService = bookingService;
            _paymentService = paymentService;
        }

        //POST : BaseUrl/api/Bookings
        [Authorize(Roles = "Guest")]
        [HttpPost]
        public async Task<ActionResult<GenericResponse<Guid>>> CreateBooking(
            CreateBookingDTO createBooking
        )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bookingService.CreateBookingAsync(userId!, createBooking);

            return HandleResponse(result);
        }

        //POST : BaseUrl/api/Bookings/{id}/pay
        [HttpPost("{id}/pay")]
        public async Task<ActionResult<GenericResponse<string>>> CreatePaymentUrl(
            [FromRoute] Guid id
        )
        {
            var result = await _paymentService.CreatePaymentUrlAsync(id);
            return HandleResponse(result);
        }
    }
}
