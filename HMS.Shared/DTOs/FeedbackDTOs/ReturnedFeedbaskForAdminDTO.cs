namespace HMS.Shared.DTOs.FeedbackDTOs
{
    public class ReturnedFeedbaskForAdminDTO
    {
        public int Id { get; set; }
        public string GuestName { get; set; } = default!;
        public string Content { get; set; } = default!;
        public int? Rating { get; set; }
        public string? ServiceName { get; set; }
        public string ModerationStatus { get; set; } = default!;
        public string? ModerationReason { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
