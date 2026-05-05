using HMS.Core.Entities.ServiceEntities;

namespace HMS.Core.Entities.IdentityEntities
{
    public class StaffUser : HotelUser
    {
        public StaffSpecialities Specialities { get; set; }
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = [];
    }
}
