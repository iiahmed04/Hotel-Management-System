namespace HMS.Shared.DTOs.BookingDTOs
{
    public class MyBookingDTO
    {
        public int RoomId { get; set; }
        public string CheckInDate { get; set; } = default!;
        public string CheckOutDate { get; set; } = default!;
        public string Status { get; set; } = default!;
        public decimal TotalAmount { get; set; }
    }
}
