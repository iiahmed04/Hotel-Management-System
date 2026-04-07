namespace HMS.Shared.DTOs.FeedbackDTOs
{
    public class GuestFeedbacksDTO
    {
        public int Id { get; set; }
        public string Content { get; set; } = default!;
        public int? Rating { get; set; }
        public string? ServiceName { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
