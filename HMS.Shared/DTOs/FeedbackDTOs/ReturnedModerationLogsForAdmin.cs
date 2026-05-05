namespace HMS.Shared.DTOs.FeedbackDTOs
{
    public class ReturnedModerationLogsForAdmin
    {
        public int Id { get; set; }
        public string GuestName { get; set; } = default!;
        public string Content { get; set; } = default!;
        public string Verdict { get; set; } = default!;
        public string? RejectionReason { get; set; }
        public DateTime AttempetedAt { get; set; }
    }
}
