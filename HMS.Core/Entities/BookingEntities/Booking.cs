using HMS.Core.Entities.IdentityEntities;
using HMS.Core.Entities.RoomEntities;

namespace HMS.Core.Entities.BookingEntities
{
    public class Booking : BaseEntity<Guid>
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "EGP";
        public BookingStatus Status { get; set; } = BookingStatus.PendingPayment;
        public string? PayMobOrderId { get; set; }
        public string? PayMobPaymentKey { get; set; }
        public DateTime? PaidDate { get; set; }
        public Room Room { get; set; } = default!;
        public int RoomId { get; set; }
        public HotelUser HotelUser { get; set; } = default!;
        public string HotelUserId { get; set; } = default!;

    }
}
