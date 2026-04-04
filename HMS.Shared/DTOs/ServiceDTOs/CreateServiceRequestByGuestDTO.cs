namespace HMS.Shared.DTOs.ServiceDTOs
{
    public class CreateServiceRequestByGuestDTO
    {
        public int ServiceId { get; set; }
        public Guid BookingId { get; set; }
        public string? Notes { get; set; }
    }
}
