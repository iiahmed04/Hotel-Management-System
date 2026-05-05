namespace HMS.Shared.DTOs.ServiceDTOs
{
    public class ServiceRequestForAdminDTO
    {
        public int Id { get; set; }
        public string GuestName { get; set; } = default!;
        public string ServiceName { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string? AssignedStaff { get; set; }
        public string? Notes { get; set; }
        public string CreatedAt { get; set; } = default!;
        public string? UpdatedAt { get; set; }
    }
}
