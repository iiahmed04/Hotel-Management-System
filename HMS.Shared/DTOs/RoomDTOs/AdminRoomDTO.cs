namespace HMS.Shared.DTOs.RoomDTOs
{
    public class AdminRoomDTO : RoomDTO
    {
        public DateTime CreatedAt { get; set; } = default!;
        public DateTime? UpdatedAt { get; set; }
    }
}
