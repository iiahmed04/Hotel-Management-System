using HMS.Core.Entities.IdentityEntities;
using HMS.Core.Entities.ServiceEntities;

namespace HMS.Core.Entities.FeedbackEntities
{
    public class Feedback : BaseEntity<int>
    {
        public string GuestId { get; set; } = default!;
        public int? ServiceId { get; set; }
        public string Content { get; set; } = default!;
        public ModerationStatus ModerationStatus { get; set; } = ModerationStatus.Approved;
        public string? ModerationReason { get; set; }
        public int? Rating { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        #region Relations
        public HotelUser Guest { get; set; } = default!;
        public Service? Service { get; set; }
        public ModerationLog? ModerationLog { get; set; }
        #endregion
    }
}
