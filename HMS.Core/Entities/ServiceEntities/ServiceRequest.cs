using HMS.Core.Entities.BookingEntities;
using HMS.Core.Entities.IdentityEntities;

namespace HMS.Core.Entities.ServiceEntities
{
    public class ServiceRequest : BaseEntity<int>
    {
        public Guid BookingId { get; set; }
        public Booking Booking { get; set; } = default!;

        public int ServiceId { get; set; }
        public Service Service { get; set; } = default!;

        public string? AssignedStaffId { get; set; }
        public StaffUser? AssignedStaff { get; set; } = default!;

        public Status Status { get; set; } = Status.Pending;
        public string? Notes { get; set; }
    }
}
