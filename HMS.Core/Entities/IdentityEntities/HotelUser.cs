using HMS.Core.Entities.BookingEntities;
using HMS.Core.Entities.FeedbackEntities;
using Microsoft.AspNetCore.Identity;

namespace HMS.Core.Entities.IdentityEntities
{
    public class HotelUser : IdentityUser
    {
        public string FullName { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        #region Relations
        public ICollection<Booking> GuestBookings { get; set; } = [];
        public ICollection<Feedback> Feedbacks { get; set; } = [];
        public ICollection<ModerationLog> ModerationLogs { get; set; } = [];
        #endregion
    }
}
