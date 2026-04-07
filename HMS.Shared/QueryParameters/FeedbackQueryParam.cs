namespace HMS.Shared.QueryParameters
{
    public class FeedbackQueryParam
    {
        public string Content { get; set; } = default!;
        public int? Rating { get; set; }
        public int? ServiceId { get; set; }
    }
}
