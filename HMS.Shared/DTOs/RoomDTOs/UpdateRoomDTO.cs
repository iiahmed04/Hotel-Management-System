using HMS.Shared.SharedEnums;

namespace HMS.Shared.DTOs.RoomDTOs
{
    public class UpdateRoomDTO
    {
        public RoomType RoomType { get; set; }
        public string Description { get; set; } = default!;
        public decimal PricePerNight { get; set; }
        public string Amenities { get; set; } = default!;
        public RoomStatus RoomStatus { get; set; }

    }
}
