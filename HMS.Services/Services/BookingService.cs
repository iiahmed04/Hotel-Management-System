using HMS.Core.Contracts;
using HMS.Core.Entities.BookingEntities;
using HMS.Core.Entities.RoomEntities;
using HMS.Services.Abstraction;
using HMS.Shared.DTOs.BookingDTOs;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HMS.Services.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<Booking> _logger;

        public BookingService(IUnitOfWork unitOfWork, ILogger<Booking> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<GenericResponse<Guid>> CreateBookingAsync(string userId, CreateBookingDTO createBooking)
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

                if (createBooking.CheckInDate < DateTime.Now || createBooking.CheckInDate >= createBooking.CheckOutDate)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "Invalid Booking Data";

                    return genericResponse;
                }

                var room = await _unitOfWork.GetRepository<Room, int>()
                    .GetByIdAsync(createBooking.RoomId, null, [R => R.RoomBookings]);

                if (room is null || room.RoomStatus == RoomStatus.NotExist || room.RoomStatus == RoomStatus.Maintenance)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = $"Room with Id : {createBooking.RoomId} that you want to booking not exist";

                    return genericResponse;
                }


                var hasConflict = room.RoomBookings.Any(b =>
                    (b.Status == BookingStatus.PendingPayment || b.Status == BookingStatus.Paid) &&
                    (b.CheckInDate < createBooking.CheckOutDate) && //Existing : 10 15
                    (b.CheckOutDate > createBooking.CheckInDate));  //New : 12 16

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
                    CreatedAt = DateTime.Now
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
    }
}
