namespace HMS.Shared.DTOs.ServiceDTOs
{
    public class ServiceRequestDTO
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string? Notes { get; set; }
        public string RequestedAt { get; set; } = default!;
        public string? UpdatedAt { get; set; } = default!;
    }
}
