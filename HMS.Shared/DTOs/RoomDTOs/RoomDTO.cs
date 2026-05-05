namespace HMS.Shared.DTOs.RoomDTOs
{
    public class RoomDTO
    {
        public int Id { get; set; }
        public string RoomType { get; set; } = default!;
        public decimal PricePerNight { get; set; }
        public string Amenities { get; set; } = default!;
        public string RoomStatus { get; set; } = default!;

    }
}
