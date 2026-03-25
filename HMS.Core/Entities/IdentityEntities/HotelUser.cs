using Microsoft.AspNetCore.Identity;

namespace HMS.Core.Entities.IdentityEntities
{
    public class HotelUser : IdentityUser
    {
        public string FullName { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
