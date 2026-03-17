using HMS.Shared.SharedEnums;
using System.ComponentModel.DataAnnotations;

namespace HMS.Shared.DTOs.RoomDTOs
{
    public class CreateRoomDTO
    {
        [Required(ErrorMessage = "Room type is required")]
        public RoomType RoomType { get; set; }

        [Required(ErrorMessage = "Room description is required")]
        [MaxLength(200)]
        public string Description { get; set; } = default!;

        [Required(ErrorMessage = "Room price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price Per Night must be positive value")]
        public decimal PricePerNight { get; set; }

        [Required(ErrorMessage = "Amenities are required")]
        public string Amenities { get; set; } = default!;
    }
}
