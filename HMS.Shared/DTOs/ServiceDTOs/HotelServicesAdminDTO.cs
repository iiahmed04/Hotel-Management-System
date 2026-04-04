namespace HMS.Shared.DTOs.ServiceDTOs
{
    public class HotelServicesAdminDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public string CreatedAt { get; set; } = default!;
        public string? UpdatedAt { get; set; }
    }
}
