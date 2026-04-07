using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using HMS.Services.Abstraction;
using HMS.Shared.Moderation;
using Microsoft.Extensions.Configuration;

namespace HMS.Infrastructure.ExternalServices
{
    public class OpenAiModerationService : IOpenAiModerationService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public OpenAiModerationService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<ModerationResult> CheckContentAsync(string content)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                _configuration["OpenAiModeration:ApiKey"]
            );

            var response = await _httpClient.PostAsJsonAsync(
                _configuration["OpenAiModeration:BaseUrl"],
                new { model = _configuration["OpenAiModeration:ModerationModel"], input = content }
            );

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();

            var flagged = result.GetProperty("results")[0].GetProperty("flagged").GetBoolean();

            return new ModerationResult
            {
                IsSafe = !flagged,
                Reason = flagged ? "Content rejected by AI moderation" : "Approved",
            };
        }
    }
}
