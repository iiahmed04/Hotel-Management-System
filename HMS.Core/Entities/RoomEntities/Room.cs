namespace HMS.Core.Entities.RoomEntities
{
    public class Room : BaseEntity<int>
    {
        public RoomType RoomType { get; set; }
        public string Description { get; set; } = default!;
        public decimal PricePerNight { get; set; }
        public string Amenities { get; set; } = default!;
        public ICollection<RoomImage> RoomImages { get; set; } = [];
        public RoomStatus RoomStatus { get; set; }
    }
}
