using HMS.Core.Entities.IdentityEntities;

namespace HMS.Core.Entities.FeedbackEntities
{
    public class ModerationLog : BaseEntity<int>
    {
        public string GuestId { get; set; } = default!;
        public string Content { get; set; } = default!;
        public Verdict Verdict { get; set; }
        public string? RejectionReason { get; set; }
        public int? FeedbackId { get; set; }
        public DateTime AttempetedAt { get; set; } = DateTime.Now;

        #region Relations
        public HotelUser Guest { get; set; } = default!;
        public Feedback? Feedback { get; set; }
        #endregion
    }
}
