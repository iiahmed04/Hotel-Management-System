namespace HMS.Shared.DTOs.RoomDTOs
{
    public class RoomDetailsDTO
    {
        public string RoomType { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal PricePerNight { get; set; }
        public string Amenities { get; set; } = default!;
        public List<string> ImageUrls { get; set; } = [];
        public string RoomStatus { get; set; } = default!;

    }
}
