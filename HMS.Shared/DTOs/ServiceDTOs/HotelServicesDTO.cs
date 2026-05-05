namespace HMS.Shared.DTOs.ServiceDTOs
{
    public class HotelServicesDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
    }
}
