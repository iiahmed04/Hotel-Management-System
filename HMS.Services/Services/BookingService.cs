using AutoMapper;
using HMS.Core.Contracts;
using HMS.Core.Entities.BookingEntities;
using HMS.Core.Entities.RoomEntities;
using HMS.Services.Abstraction;
using HMS.Shared.DTOs.BookingDTOs;
using HMS.Shared.Messages;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HMS.Services.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<Booking> _logger;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public BookingService(
            IUnitOfWork unitOfWork,
            ILogger<Booking> logger,
            IMapper mapper,
            IEmailService emailService
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<GenericResponse<bool>> CancelBookingAsync(Guid bookingId)
        {
            var genericResponse = new GenericResponse<bool>();

            try
            {
                var booking = await _unitOfWork
                    .GetRepository<Booking, Guid>()
                    .GetByIdAsync(bookingId, null, [B => B.HotelUser]);

                if (booking is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = "No Booking found to Cancel";

                    return genericResponse;
                }

                if (booking.Status == BookingStatus.Paid)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message =
                        "You can't cancel this booking , because it already Paid!";

                    return genericResponse;
                }

                booking.Status = BookingStatus.Canceled;
                booking.UpdatedAt = DateTime.Now;

                _unitOfWork.GetRepository<Booking, Guid>().Update(booking);

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (!result)
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Failed to cancel this booking";

                    return genericResponse;
                }

                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Success to cancel booking";
                genericResponse.Data = true;

                var email = new Email
                {
                    To = booking.HotelUser.Email!,
                    Subject = "Your booking has been Canceled",
                    Body =
                        "Sorry,we have cancel your booking because of emergency reasons , please contact us for more details",
                };

                await _emailService.SendEmailAsync(email);

                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel this Error");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed to cancel this booking";

                return genericResponse;
            }
        }

        public async Task<GenericResponse<Guid>> CreateBookingAsync(
            string userId,
            CreateBookingDTO createBooking
        )
        {
            var genericResponse = new GenericResponse<Guid>();

            try
            {
                if (createBooking is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "No Booking Data to Create booking";

                    return genericResponse;
                }

                if (
                    createBooking.CheckInDate < DateTime.Now
                    || createBooking.CheckInDate >= createBooking.CheckOutDate
                )
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "Invalid Booking Data";

                    return genericResponse;
                }

                var room = await _unitOfWork
                    .GetRepository<Room, int>()
                    .GetByIdAsync(createBooking.RoomId, null, [R => R.RoomBookings]);

                if (
                    room is null
                    || room.RoomStatus == RoomStatus.NotExist
                    || room.RoomStatus == RoomStatus.Maintenance
                )
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message =
                        $"Room with Id : {createBooking.RoomId} that you want to booking not exist";

                    return genericResponse;
                }

                var hasConflict = room.RoomBookings.Any(b =>
                    (b.Status == BookingStatus.PendingPayment || b.Status == BookingStatus.Paid)
                    && (b.CheckInDate < createBooking.CheckOutDate)
                    && //Existing : 10 15
                    (b.CheckOutDate > createBooking.CheckInDate)
                ); //New : 12 16

                if (hasConflict)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "Room not available to booking in this Dates";

                    return genericResponse;
                }

                var numOfNights = (createBooking.CheckOutDate - createBooking.CheckInDate).Days;
                var totalAmount = room.PricePerNight * numOfNights;

                var booking = new Booking
                {
                    Id = Guid.NewGuid(),
                    CheckInDate = createBooking.CheckInDate,
                    CheckOutDate = createBooking.CheckOutDate,
                    TotalAmount = totalAmount,
                    RoomId = createBooking.RoomId,
                    HotelUserId = userId,
                    CreatedAt = DateTime.Now,
                };

                await _unitOfWork.GetRepository<Booking, Guid>().AddAsync(booking);

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (!result)
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Failed to booking";
                }

                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Booking Created Successfully";
                genericResponse.Data = booking.Id;

                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to booking");
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Booking Created Successfully";

                return genericResponse;
            }
        }

        public async Task<GenericResponse<IEnumerable<MyBookingDTO>>> GetAllBookingForGuestAsync(
            string guestId
        )
        {
            var genericResponse = new GenericResponse<IEnumerable<MyBookingDTO>>();

            var bookings = await _unitOfWork
                .GetRepository<Booking, Guid>()
                .GetAllAsync(x => x.HotelUserId == guestId, null, x => x.CreatedAt);

            if (bookings is null || !bookings.Any())
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "No Bookings to show";

                return genericResponse;
            }

            var mappedBooking = _mapper.Map<IEnumerable<MyBookingDTO>>(bookings);

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Success to show all Guest Bookings";
            genericResponse.Data = mappedBooking;

            return genericResponse;
        }

        public async Task<GenericResponse<IEnumerable<BookingDTO>>> GetAllBookingsForAdminAsync()
        {
            var genericResponse = new GenericResponse<IEnumerable<BookingDTO>>();

            var bookings = await _unitOfWork
                .GetRepository<Booking, Guid>()
                .GetAllAsync(null, null, x => x.CreatedAt, [B => B.HotelUser]);

            if (bookings is null || !bookings.Any())
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "No Bookings founded to show";

                return genericResponse;
            }

            var mappedBooking = _mapper.Map<IEnumerable<BookingDTO>>(bookings);

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Success to retrieve all bookings";
            genericResponse.Data = mappedBooking;

            return genericResponse;
        }
    }
}
