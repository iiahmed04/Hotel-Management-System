using HMS.Shared.Moderation;

namespace HMS.Services.Abstraction
{
    public interface IOpenAiModerationService
    {
        Task<ModerationResult> CheckContentAsync(string content);
    }
}
