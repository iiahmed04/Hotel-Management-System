using HMS.Shared.DTOs.BookingDTOs;
using HMS.Shared.Responses;

namespace HMS.Services.Abstraction
{
    public interface IBookingService
    {
        Task<GenericResponse<Guid>> CreateBookingAsync(
            string userId,
            CreateBookingDTO createBooking
        );

        Task<GenericResponse<IEnumerable<BookingDTO>>> GetAllBookingsForAdminAsync();
        Task<GenericResponse<bool>> CancelBookingAsync(Guid bookingId);
        Task<GenericResponse<IEnumerable<MyBookingDTO>>> GetAllBookingForGuestAsync(string guestId);
    }
}
