namespace HMS.Infrastructure.ExternalServices
{
    public class PayMobSettings
    {
        public string BaseUrl { get; set; }
        public int IntegrationId { get; set; }
        public int IFrameId { get; set; }
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public string HMAC { get; set; }
    }
}

